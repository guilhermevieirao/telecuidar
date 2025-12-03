using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using app.Application.Common.Interfaces;
using app.Application.Cadsus.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace app.Infrastructure.Services;

public class CadsusService : ICadsusService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CadsusService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    
    private string? _cachedToken;
    private DateTime? _tokenExpiry;
    
    public CadsusService(
        IConfiguration configuration, 
        ILogger<CadsusService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CadsusTokenStatus> GetTokenStatusAsync()
    {
        if (string.IsNullOrEmpty(_cachedToken) || _tokenExpiry == null)
        {
            return new CadsusTokenStatus
            {
                HasToken = false,
                Message = "No token available"
            };
        }

        var now = DateTime.Now;
        var isValid = now < _tokenExpiry.Value;
        var expiresIn = _tokenExpiry.Value - now;

        return new CadsusTokenStatus
        {
            HasToken = true,
            IsValid = isValid,
            ExpiresAt = _tokenExpiry.Value.ToString("dd/MM/yyyy HH:mm:ss"),
            ExpiresIn = $"{(int)expiresIn.TotalMinutes} minutos",
            ExpiresInMs = (long)expiresIn.TotalMilliseconds
        };
    }

    public async Task<string> RenewTokenAsync()
    {
        _logger.LogInformation("🔄 Forcing token renewal...");
        _cachedToken = null;
        _tokenExpiry = null;
        return await GetTokenAsync();
    }

    public async Task<CadsusCidadao> ConsultarCpfAsync(string cpf)
    {
        // Limpar CPF (remover formatação)
        var cleanCpf = Regex.Replace(cpf, @"\D", "");

        if (cleanCpf.Length != 11)
        {
            throw new ArgumentException("CPF deve conter 11 dígitos");
        }

        // Obter token
        var token = await GetTokenAsync();

        // Construir requisição SOAP
        var soapRequest = BuildSoapRequest(cleanCpf);

        _logger.LogInformation($"🔍 Querying CADSUS for CPF: {cleanCpf}");

        // Executar requisição via PowerShell com certificado
        var responseXml = await ExecuteSoapRequestAsync(soapRequest, token);

        // Parsear resposta XML
        return await ParseResponseAsync(responseXml);
    }

    private string Sanitize(string? value)
    {
        return (value ?? string.Empty).Trim().Trim('"').Trim('\'');
    }

    private async Task<string> GetTokenAsync()
    {
        // Verificar se token em cache ainda é válido (com buffer de 5 minutos)
        if (!string.IsNullOrEmpty(_cachedToken) && 
            _tokenExpiry != null && 
            DateTime.Now < _tokenExpiry.Value.AddMinutes(-5))
        {
            _logger.LogInformation("✅ Using cached token");
            return _cachedToken;
        }

        _logger.LogInformation("🔑 Fetching new token using HttpClient with certificate...");

        var ambiente = Sanitize(_configuration["CADSUS__Ambiente"] ?? Environment.GetEnvironmentVariable("CADSUS__Ambiente") ?? "producao");
        var isProd = ambiente.Equals("producao", StringComparison.OrdinalIgnoreCase);
        var authUrl = Sanitize(isProd 
            ? (_configuration["CADSUS__AuthUrl_Prod"] ?? Environment.GetEnvironmentVariable("CADSUS__AuthUrl_Prod") ?? "https://ehr-auth.saude.gov.br/api/osb/token")
            : (_configuration["CADSUS__AuthUrl_Hmg"] ?? Environment.GetEnvironmentVariable("CADSUS__AuthUrl_Hmg") ?? "https://ehr-auth-hmg.saude.gov.br/api/osb/token"));
        var certPath = Sanitize(_configuration["CADSUS__CertPath"] ?? Environment.GetEnvironmentVariable("CADSUS__CertPath"));
        var certPassword = Sanitize(_configuration["CADSUS__CertPassword"] ?? Environment.GetEnvironmentVariable("CADSUS__CertPassword"));

        if (string.IsNullOrWhiteSpace(certPath))
        {
            throw new InvalidOperationException("CADSUS__CertPath não está configurado no .env");
        }

        if (!File.Exists(certPath))
        {
            throw new FileNotFoundException($"Certificado não encontrado: {certPath}");
        }

        _logger.LogInformation($"📜 URL: {authUrl}");
        _logger.LogInformation($"📜 Certificate: {certPath}");

        try
        {
            // Carregar certificado usando X509CertificateLoader (recomendado para .NET 9+)
            var certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);
            _logger.LogInformation($"✅ Certificate loaded: {certificate.Subject}");

            // Criar handler com certificado
            using var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            
            // Configurar SSL/TLS
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(2); // Aumentar timeout para 2 minutos
            
            // Adicionar headers que o PowerShell envia automaticamente
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.ConnectionClose = false;

            // Fazer requisição GET para obter token
            _logger.LogInformation($"🔄 Sending GET request to auth endpoint...");
            var response = await client.GetAsync(authUrl);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"📡 HTTP Status: {response.StatusCode}");
            _logger.LogInformation($"📄 Response length: {content.Length}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"❌ HTTP Error: {response.StatusCode}");
                _logger.LogError($"Response: {content}");
                throw new Exception($"HTTP {response.StatusCode}: {content}");
            }

            // Parse JSON response
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _logger.LogError($"❌ Invalid token response: {content}");
                throw new Exception("No access_token in response");
            }

            _cachedToken = tokenResponse.AccessToken;
            // Tokens expiram em 30 minutos
            _tokenExpiry = DateTime.Now.AddMinutes(30);

            _logger.LogInformation("✅ Token obtained successfully");
            _logger.LogInformation($"⏰ Token expires at: {_tokenExpiry:dd/MM/yyyy HH:mm:ss}");

            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting token");
            throw new Exception($"Failed to obtain authentication token: {ex.Message}", ex);
        }
    }

    private string BuildSoapRequest(string cpf)
    {
        return $@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:urn=""urn:ihe:iti:xds-b:2007"" xmlns:urn1=""urn:oasis:names:tc:ebxml-regrep:xsd:lcm:3.0"" xmlns:urn2=""urn:oasis:names:tc:ebxml-regrep:xsd:rim:3.0"" xmlns:urn3=""urn:ihe:iti:xds-b:2007"">
  <soap:Body>
    <PRPA_IN201305UV02 xsi:schemaLocation=""urn:hl7-org:v3 ./schema/HL7V3/NE2008/multicacheschemas/PRPA_IN201305UV02.xsd"" ITSVersion=""XML_1.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:hl7-org:v3"">
      <id root=""2.16.840.1.113883.4.714"" extension=""123456""/>
      <creationTime value=""20070428150301""/>
      <interactionId root=""2.16.840.1.113883.1.6"" extension=""PRPA_IN201305UV02""/>
      <processingCode code=""T""/>
      <processingModeCode code=""T""/>
      <acceptAckCode code=""AL""/>
      <receiver typeCode=""RCV"">
        <device classCode=""DEV"" determinerCode=""INSTANCE"">
          <id root=""2.16.840.1.113883.3.72.6.5.100.85""/>
        </device>
      </receiver>
      <sender typeCode=""SND"">
        <device classCode=""DEV"" determinerCode=""INSTANCE"">
          <id root=""2.16.840.1.113883.3.72.6.2""/>
          <name>CADSUS</name>
        </device>
      </sender>
      <controlActProcess classCode=""CACT"" moodCode=""EVN"">
        <code code=""PRPA_TE201305UV02"" codeSystem=""2.16.840.1.113883.1.6""/>
        <queryByParameter>
          <queryId root=""1.2.840.114350.1.13.28.1.18.5.999"" extension=""1840997084""/>
          <statusCode code=""new""/>
          <responseModalityCode code=""R""/>
          <responsePriorityCode code=""I""/>
          <parameterList>
            <livingSubjectId>
              <value root=""2.16.840.1.113883.13.237"" extension=""{cpf}""/>
              <semanticsText>LivingSubject.id</semanticsText>
            </livingSubjectId>
          </parameterList>
        </queryByParameter>
      </controlActProcess>
    </PRPA_IN201305UV02>
  </soap:Body>
</soap:Envelope>";
    }

    private async Task<string> ExecuteSoapRequestAsync(string soapRequest, string token)
    {
        var ambiente = Sanitize(_configuration["CADSUS__Ambiente"] ?? Environment.GetEnvironmentVariable("CADSUS__Ambiente") ?? "producao");
        var isProd = ambiente.Equals("producao", StringComparison.OrdinalIgnoreCase);
        var queryUrl = Sanitize(isProd
            ? (_configuration["CADSUS__QueryUrl_Prod"] ?? Environment.GetEnvironmentVariable("CADSUS__QueryUrl_Prod") ?? "https://servicos.saude.gov.br/cadsus/v2/PDQSupplierJWT")
            : (_configuration["CADSUS__QueryUrl_Hmg"] ?? Environment.GetEnvironmentVariable("CADSUS__QueryUrl_Hmg") ?? "https://servicoshm.saude.gov.br/cadsus/v2/PDQSupplierJWT"));
        var certPath = Sanitize(_configuration["CADSUS__CertPath"] ?? Environment.GetEnvironmentVariable("CADSUS__CertPath"));
        var certPassword = Sanitize(_configuration["CADSUS__CertPassword"] ?? Environment.GetEnvironmentVariable("CADSUS__CertPassword"));

        _logger.LogInformation("📡 Executing SOAP request with HttpClient...");

        try
        {
            // Carregar certificado usando X509CertificateLoader (recomendado para .NET 9+)
            var certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);

            // Criar handler com certificado
            using var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            
            // Configurar SSL/TLS
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(2); // Aumentar timeout para 2 minutos
            
            // Adicionar headers que o PowerShell envia automaticamente
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.ConnectionClose = false;

            // Preparar requisição SOAP
            _logger.LogInformation($"🔄 Sending POST request to query endpoint...");
            var request = new HttpRequestMessage(HttpMethod.Post, queryUrl);
            request.Headers.Add("Authorization", $"jwt {token}");
            request.Content = new StringContent(soapRequest, Encoding.UTF8, "application/soap+xml");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"📡 HTTP Status: {response.StatusCode}");
            _logger.LogInformation($"📄 Response length: {responseContent.Length}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"❌ SOAP Error: {response.StatusCode}");
                _logger.LogError($"Response: {responseContent}");
                throw new Exception($"SOAP HTTP {response.StatusCode}: {responseContent}");
            }

            // Salvar XML para debug
            var xmlDebugPath = Path.Combine(Path.GetTempPath(), $"cadsus_response_{DateTime.Now:yyyyMMddHHmmss}.xml");
            await File.WriteAllTextAsync(xmlDebugPath, responseContent, Encoding.UTF8);
            _logger.LogInformation($"💾 XML saved to: {xmlDebugPath}");

        return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error executing SOAP request");
            throw;
        }
    }

    private async Task<CadsusCidadao> ParseResponseAsync(string xml)
    {
        _logger.LogInformation("🔍 Parsing XML response...");
        _logger.LogInformation($"📄 XML Length: {xml.Length}");

        var doc = XDocument.Parse(xml);
        
        // O XML usa namespace HL7 dentro do SOAP Body
        XNamespace hl7 = "urn:hl7-org:v3";
        
        // Buscar o patient element que contém todos os dados
        var patientElement = doc.Descendants(hl7 + "patient").FirstOrDefault();
        
        if (patientElement == null)
        {
            _logger.LogError("❌ Patient element not found in XML");
            throw new Exception("Patient element not found in CADSUS response");
        }

        _logger.LogInformation("✅ Patient element found");

        // Extrair IDs (buscar em patientElement e patientPerson)
        var patientPerson = patientElement.Descendants(hl7 + "patientPerson").FirstOrDefault();
        var ids = patientElement.Descendants(hl7 + "id")
            .Select(e => (
                Root: e.Attribute("root")?.Value,
                Extension: e.Attribute("extension")?.Value
            ))
            .Where(x => !string.IsNullOrEmpty(x.Root) && !string.IsNullOrEmpty(x.Extension))
            .ToList();

        _logger.LogInformation($"📋 Found {ids.Count} IDs");

        // CPF
        var cpfObj = ids.FirstOrDefault(x => x.Root == "2.16.840.1.113883.13.237" || x.Root == "2.16.840.1.113883.3.4594.100.4");
        var cpfRaw = cpfObj.Extension ?? "";
        var cpf = cpfRaw.Length == 11 
            ? $"{cpfRaw.Substring(0, 3)}.{cpfRaw.Substring(3, 3)}.{cpfRaw.Substring(6, 3)}-{cpfRaw.Substring(9)}"
            : cpfRaw;

        _logger.LogInformation($"📋 CPF: {cpf}");

        // CNS (dentro de asOtherIDs)
        var cnsList = new List<string>();
        var asOtherIDsElements = patientPerson?.Descendants(hl7 + "asOtherIDs") ?? Enumerable.Empty<XElement>();
        
        foreach (var asOtherIDs in asOtherIDsElements)
        {
            var cnsNumber = "";
            var cnsType = "";
            
            foreach (var idEl in asOtherIDs.Descendants(hl7 + "id"))
            {
                var root = idEl.Attribute("root")?.Value;
                var ext = idEl.Attribute("extension")?.Value;
                
                if (root == "2.16.840.1.113883.13.236" && !string.IsNullOrEmpty(ext))
                {
                    cnsNumber = ext;
                }
                if (root == "2.16.840.1.113883.13.236.1" && !string.IsNullOrEmpty(ext))
                {
                    cnsType = ext == "P" ? " (Principal)" : ext == "D" ? " (Definitivo)" : $" ({ext})";
                }
            }
            
            if (!string.IsNullOrEmpty(cnsNumber))
            {
                cnsList.Add(cnsNumber + cnsType);
            }
        }
        
        var cns = cnsList.Count > 0 ? string.Join(", ", cnsList) : "";

        // Nome
        var nome = "";
        var nameElements = patientPerson?.Descendants(hl7 + "name") ?? Enumerable.Empty<XElement>();
        foreach (var nameEl in nameElements)
        {
            var use = nameEl.Attribute("use")?.Value;
            if (use == "L")
            {
                nome = nameEl.Descendants(hl7 + "given").FirstOrDefault()?.Value?.Trim() ?? "";
                break;
            }
        }

        _logger.LogInformation($"📋 Nome: {nome}");

        // Data de Nascimento
        var dataNascimento = "";
        var birthTimeValue = patientPerson?.Descendants(hl7 + "birthTime").FirstOrDefault()?.Attribute("value")?.Value ?? "";
        if (birthTimeValue.Length >= 8)
        {
            dataNascimento = $"{birthTimeValue.Substring(6, 2)}/{birthTimeValue.Substring(4, 2)}/{birthTimeValue.Substring(0, 4)}";
        }

        // Sexo
        var sexoCode = patientPerson?.Descendants(hl7 + "administrativeGenderCode").FirstOrDefault()?.Attribute("code")?.Value ?? "";
        var sexo = sexoCode == "M" ? "Masculino" : sexoCode == "F" ? "Feminino" : sexoCode;

        // Raça/Cor
        var racaCode = patientPerson?.Descendants(hl7 + "raceCode").FirstOrDefault()?.Attribute("code")?.Value ?? "";
        var racaMap = new Dictionary<string, string>
        {
            {"01", "Branca"}, {"02", "Preta"}, {"03", "Parda"}, 
            {"04", "Amarela"}, {"05", "Indígena"}, {"99", "Sem informação"}
        };
        var racaCor = racaMap.ContainsKey(racaCode) ? racaMap[racaCode] : racaCode;

        // Filiação
        var nomeMae = "";
        var nomePai = "";
        var relationElements = patientPerson?.Descendants(hl7 + "personalRelationship") ?? Enumerable.Empty<XElement>();
        foreach (var rel in relationElements)
        {
            var code = rel.Descendants(hl7 + "code").FirstOrDefault()?.Attribute("code")?.Value;
            var name = rel.Descendants(hl7 + "given").FirstOrDefault()?.Value?.Trim();
            
            if (code == "PRN") nomeMae = name ?? "";
            if (code == "NPRN") nomePai = name ?? "";
        }

        // Endereço
        var tipoLogradouro = "";
        var logradouro = "";
        var numero = "";
        var complemento = "";
        var codigoCidade = "";
        var cidade = "";
        var cep = "";
        var paisEnderecoAtual = "";
        
        var addrElements = patientPerson?.Descendants(hl7 + "addr") ?? Enumerable.Empty<XElement>();
        foreach (var addr in addrElements)
        {
            var use = addr.Attribute("use")?.Value;
            if (use == "H")
            {
                logradouro = addr.Descendants(hl7 + "streetName").FirstOrDefault()?.Value?.Trim() ?? "";
                
                var tipoLogradouroCod = addr.Descendants(hl7 + "streetNameType").FirstOrDefault()?.Value?.Trim() ?? "";
                var tiposLogradouro = new Dictionary<string, string>
                {
                    {"001", "Rua"}, {"002", "Avenida"}, {"003", "Travessa"}, {"004", "Alameda"},
                    {"005", "Praça"}, {"006", "Largo"}, {"007", "Rodovia"}, {"008", "Estrada"},
                    {"081", "Rua"}
                };
                tipoLogradouro = tiposLogradouro.ContainsKey(tipoLogradouroCod) ? tiposLogradouro[tipoLogradouroCod] : tipoLogradouroCod;
                
                numero = addr.Descendants(hl7 + "houseNumber").FirstOrDefault()?.Value?.Trim() ?? "";
                complemento = addr.Descendants(hl7 + "additionalLocator").FirstOrDefault()?.Value?.Trim() ?? "";
                codigoCidade = addr.Descendants(hl7 + "city").FirstOrDefault()?.Value?.Trim() ?? "";
                cidade = await GetCityNameAsync(codigoCidade);
                
                var cepRaw = addr.Descendants(hl7 + "postalCode").FirstOrDefault()?.Value?.Trim() ?? "";
                cep = cepRaw.Length == 8 ? $"{cepRaw.Substring(0, 5)}-{cepRaw.Substring(5)}" : cepRaw;
                
                var paisCod = addr.Descendants(hl7 + "country").FirstOrDefault()?.Value?.Trim() ?? "";
                paisEnderecoAtual = GetCountryName(paisCod);
                
                break;
            }
        }

        // Naturalidade
        var codigoCidadeNascimento = "";
        var cidadeNascimento = "";
        var codigoPaisNascimento = "";
        var paisNascimento = "";
        var birthPlaceElements = patientPerson?.Descendants(hl7 + "birthPlace") ?? Enumerable.Empty<XElement>();
        if (birthPlaceElements.Any())
        {
            var birthAddr = birthPlaceElements.First().Descendants(hl7 + "addr").FirstOrDefault();
            if (birthAddr != null)
            {
                codigoCidadeNascimento = birthAddr.Descendants(hl7 + "city").FirstOrDefault()?.Value?.Trim() ?? "";
                cidadeNascimento = await GetCityNameAsync(codigoCidadeNascimento);
                
                codigoPaisNascimento = birthAddr.Descendants(hl7 + "country").FirstOrDefault()?.Value?.Trim() ?? "";
                paisNascimento = GetCountryName(codigoPaisNascimento);
            }
        }

        // Telefones e E-mails
        var telefones = new List<string>();
        var emails = new List<string>();
        var telecomElements = patientPerson?.Descendants(hl7 + "telecom") ?? Enumerable.Empty<XElement>();
        foreach (var telecom in telecomElements)
        {
            var value = telecom.Attribute("value")?.Value ?? "";
            if (value.StartsWith("+") || value.StartsWith("tel:"))
            {
                telefones.Add(FormatPhone(value.Replace("tel:", "")));
            }
            else if (value.Contains("@") || value.StartsWith("mailto:"))
            {
                emails.Add(value.Replace("mailto:", ""));
            }
        }

        // Status
        var statusCadastro = "";
        var statusElements = patientElement?.Descendants(hl7 + "statusCode") ?? Enumerable.Empty<XElement>();
        foreach (var status in statusElements)
        {
            var parent = status.Parent;
            if (parent?.Name.LocalName == "patient")
            {
                var statusCode = status.Attribute("code")?.Value;
                statusCadastro = statusCode == "active" ? "Ativo" : statusCode == "inactive" ? "Inativo" : statusCode ?? "";
                break;
            }
        }

        // Endereço completo
        var enderecoCompleto = string.Join(", ", new[] { tipoLogradouro, logradouro, numero, complemento, cidade, cep }.Where(s => !string.IsNullOrEmpty(s)));

        _logger.LogInformation("✅ Parsed data successfully");
        _logger.LogInformation($"📋 CPF: {cpf}, Nome: {nome}, CNS: {cns}");
        _logger.LogInformation($"📋 Data Nasc: {dataNascimento}, Sexo: {sexo}, Raça: {racaCor}");
        _logger.LogInformation($"📋 Mãe: {nomeMae}, Pai: {nomePai}");
        _logger.LogInformation($"📋 Endereço: {enderecoCompleto}");
        _logger.LogInformation($"📋 Telefones: {string.Join(", ", telefones)}, Emails: {string.Join(", ", emails)}");

        return new CadsusCidadao
        {
            Cns = cns,
            Cpf = cpf,
            Nome = nome,
            DataNascimento = dataNascimento,
            StatusCadastro = statusCadastro,
            NomeMae = nomeMae,
            NomePai = nomePai,
            Sexo = sexo,
            RacaCor = racaCor,
            TipoLogradouro = tipoLogradouro,
            Logradouro = logradouro,
            Numero = numero,
            Complemento = complemento,
            Cidade = cidade,
            CodigoCidade = codigoCidade,
            PaisEnderecoAtual = paisEnderecoAtual,
            Cep = cep,
            EnderecoCompleto = enderecoCompleto,
            CidadeNascimento = cidadeNascimento,
            CodigoCidadeNascimento = codigoCidadeNascimento,
            PaisNascimento = paisNascimento,
            CodigoPaisNascimento = codigoPaisNascimento,
            Telefones = telefones,
            Emails = emails
        };
    }

    private async Task<string> GetCityNameAsync(string codigoIBGE)
    {
        if (string.IsNullOrEmpty(codigoIBGE)) return "";

        try
        {
            // A API do IBGE precisa do código com 7 dígitos
            var codigoCompleto = codigoIBGE.Length == 6 ? codigoIBGE + "0" : codigoIBGE;

            var client = _httpClientFactory.CreateClient();
            var url = $"https://servicodados.ibge.gov.br/api/v1/localidades/municipios/{codigoCompleto}";
            
            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Trim() != "[]")
                {
                    var municipio = JsonSerializer.Deserialize<IbgeMunicipioResponse>(content);
                    if (municipio != null && municipio.Nome != null)
                    {
                        var cidade = municipio.Nome;
                        var uf = municipio.Microrregiao?.Mesorregiao?.UF?.Sigla ?? "";
                        return !string.IsNullOrEmpty(uf) ? $"{cidade}/{uf}" : cidade;
                    }
                }
            }

            _logger.LogWarning($"⚠️ Cidade não encontrada para código {codigoIBGE}");
            return $"Código IBGE: {codigoIBGE}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar cidade para código {codigoIBGE}");
            return $"Código IBGE: {codigoIBGE}";
        }
    }

    private string GetCountryName(string codigo)
    {
        var paisMap = new Dictionary<string, string>
        {
            {"010", "Brasil"}, {"020", "Argentina"}, {"023", "Bolívia"}, {"031", "Chile"},
            {"035", "Colômbia"}, {"039", "Equador"}, {"045", "Guiana Francesa"}, {"047", "Guiana"},
            {"053", "Paraguai"}, {"055", "Peru"}, {"063", "Suriname"}, {"067", "Uruguai"},
            {"071", "Venezuela"}, {"110", "Estados Unidos"}, {"190", "Alemanha"}, {"205", "Espanha"},
            {"208", "França"}, {"218", "Itália"}, {"239", "Portugal"}, {"240", "Reino Unido"},
            {"385", "Japão"}, {"355", "China"}, {"419", "Austrália"}
        };

        return paisMap.ContainsKey(codigo) ? paisMap[codigo] : codigo;
    }

    private string FormatPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone)) return phone;

        // Remove +55 e não-dígitos
        var cleaned = Regex.Replace(phone.Replace("+55", ""), @"\D", "");

        if (cleaned.Length == 11)
        {
            // Celular: (XX) 9XXXX-XXXX
            return $"({cleaned.Substring(0, 2)}) {cleaned.Substring(2, 5)}-{cleaned.Substring(7)}";
        }
        else if (cleaned.Length == 10)
        {
            // Fixo: (XX) XXXX-XXXX
            return $"({cleaned.Substring(0, 2)}) {cleaned.Substring(2, 4)}-{cleaned.Substring(6)}";
        }

        return phone;
    }

    // Classes auxiliares
    private class TokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";
    }

    private class IbgeMunicipioResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("microrregiao")]
        public IbgeMicrorregiao? Microrregiao { get; set; }
    }

    private class IbgeMicrorregiao
    {
        [System.Text.Json.Serialization.JsonPropertyName("mesorregiao")]
        public IbgeMesorregiao? Mesorregiao { get; set; }
    }

    private class IbgeMesorregiao
    {
        [System.Text.Json.Serialization.JsonPropertyName("UF")]
        public IbgeUF? UF { get; set; }
    }

    private class IbgeUF
    {
        [System.Text.Json.Serialization.JsonPropertyName("sigla")]
        public string? Sigla { get; set; }
    }
}
