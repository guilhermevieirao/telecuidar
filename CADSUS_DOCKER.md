# CADSUS no Docker - Solução Implementada

## Problema
O CADSUS funcionava quando executado via task (Windows) mas não funcionava no Docker devido a:
- Erro SSL: `error:0A000416:SSL routines::sslv3 alert certificate unknown`
- Incompatibilidade do OpenSSL no Linux com certificados digitais A1

## Solução Implementada

### 1. Dockerfile - Configuração do OpenSSL
```dockerfile
# Install ca-certificates and OpenSSL
RUN apt-get update && apt-get install -y \
    ca-certificates \
    openssl \
    && rm -rf /var/lib/apt/lists/*

# Configure OpenSSL para melhor compatibilidade com certificados
RUN echo "[default_conf]" > /etc/ssl/openssl_custom.cnf && \
    echo "ssl_conf = ssl_sect" >> /etc/ssl/openssl_custom.cnf && \
    echo "[ssl_sect]" >> /etc/ssl/openssl_custom.cnf && \
    echo "system_default = system_default_sect" >> /etc/ssl/openssl_custom.cnf && \
    echo "[system_default_sect]" >> /etc/ssl/openssl_custom.cnf && \
    echo "MinProtocol = TLSv1.2" >> /etc/ssl/openssl_custom.cnf && \
    echo "CipherString = DEFAULT@SECLEVEL=1" >> /etc/ssl/openssl_custom.cnf

ENV OPENSSL_CONF=/etc/ssl/openssl_custom.cnf
```

**Chave**: `SECLEVEL=1` permite certificados mais antigos/governamentais que não passariam na validação SECLEVEL=2 padrão.

### 2. CadsusService.cs - Carregamento da Cadeia Completa

```csharp
// Carregar TODA a coleção PKCS12 (certificado + intermediários)
var certificates = X509CertificateLoader.LoadPkcs12CollectionFromFile(
    certPath, 
    certPassword,
    X509KeyStorageFlags.Exportable | 
    X509KeyStorageFlags.MachineKeySet | 
    X509KeyStorageFlags.PersistKeySet);

// Adicionar o certificado cliente E toda a cadeia ao handler
handler.ClientCertificates.Add(clientCertificate);
foreach (var cert in certificates)
{
    if (cert != clientCertificate)
    {
        handler.ClientCertificates.Add(cert);
    }
}
```

**Chave**: Enviar a cadeia completa de certificados, não apenas o certificado principal.

### 3. docker-compose.yml - Montar .env e Certificado

```yaml
volumes:
  - db-data:/data
  - ./backend/api/certificado.pfx:/app/certificado.pfx:ro
  - ./backend/api/.env:/app/.env:ro
```

## Por que funcionou?

1. **OpenSSL SECLEVEL=1**: Permite certificados governamentais que usam algoritmos/chaves considerados "fracos" pelo padrão SECLEVEL=2
2. **Cadeia completa**: O servidor do governo precisa validar toda a cadeia de certificados, não apenas o certificado final
3. **Flags corretas**: `MachineKeySet` + `PersistKeySet` garantem que a chave privada seja acessível no ambiente Linux

## Data da Solução
03 de dezembro de 2025
