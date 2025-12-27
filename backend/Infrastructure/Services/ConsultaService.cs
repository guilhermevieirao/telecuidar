using Application.DTOs.Consultas;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de consultas
/// </summary>
public class ConsultaService : IConsultaService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<ConsultaService> _logger;

    public ConsultaService(
        ApplicationDbContext context,
        INotificacaoService notificacaoService,
        ILogger<ConsultaService> logger)
    {
        _context = context;
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task<ConsultasPaginadasDto> ObterConsultasAsync(int pagina, int tamanhoPagina, string? busca, string? status, DateTime? dataInicio, DateTime? dataFim, Guid? pacienteId = null, Guid? profissionalId = null)
    {
        var query = _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Profissional)
            .Include(c => c.Especialidade)
            .Include(c => c.PreConsulta)
            .Include(c => c.Anamnese)
            .Include(c => c.Soap)
            .Include(c => c.DadosBiometricos)
            .AsQueryable();

        if (!string.IsNullOrEmpty(busca))
        {
            busca = busca.ToLower();
            query = query.Where(c =>
                c.Paciente!.Nome.ToLower().Contains(busca) ||
                c.Profissional!.Nome.ToLower().Contains(busca));
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusConsulta>(status, true, out var statusEnum))
        {
            query = query.Where(c => c.Status == statusEnum);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(c => c.Data >= dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(c => c.Data <= dataFim.Value);
        }

        if (pacienteId.HasValue)
        {
            query = query.Where(c => c.PacienteId == pacienteId);
        }

        if (profissionalId.HasValue)
        {
            query = query.Where(c => c.ProfissionalId == profissionalId);
        }

        var total = await query.CountAsync();
        var consultas = await query
            .OrderByDescending(c => c.Data)
            .ThenByDescending(c => c.Horario)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new ConsultasPaginadasDto
        {
            Dados = consultas.Select(MapearParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<ConsultaDto?> ObterConsultaPorIdAsync(Guid id)
    {
        var consulta = await _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Profissional)
            .Include(c => c.Especialidade)
            .Include(c => c.PreConsulta)
            .Include(c => c.Anamnese)
            .Include(c => c.Soap)
            .Include(c => c.DadosBiometricos)
            .Include(c => c.Anexos)
            .FirstOrDefaultAsync(c => c.Id == id);

        return consulta != null ? MapearParaDto(consulta) : null;
    }

    public async Task<ConsultaDto> CriarConsultaAsync(CriarConsultaDto dto)
    {
        if (!DateTime.TryParse(dto.Data, out var data))
        {
            throw new InvalidOperationException("Data inválida.");
        }

        var consulta = new Consulta
        {
            PacienteId = dto.PacienteId,
            ProfissionalId = dto.ProfissionalId,
            EspecialidadeId = dto.EspecialidadeId,
            Data = data,
            Horario = TimeSpan.Parse(dto.Horario),
            Tipo = string.IsNullOrEmpty(dto.Tipo) ? TipoConsulta.Teleconsulta : Enum.Parse<TipoConsulta>(dto.Tipo, true),
            Status = StatusConsulta.Agendada,
            Observacao = dto.Observacao,
            CriadoEm = DateTime.UtcNow
        };

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        // Enviar notificações
        await _notificacaoService.EnviarNotificacaoNovaConsultaAsync(
            dto.PacienteId,
            dto.ProfissionalId,
            data,
            dto.Horario);

        return await ObterConsultaPorIdAsync(consulta.Id) ?? throw new InvalidOperationException("Erro ao criar consulta.");
    }

    public async Task<ConsultaDto?> AtualizarConsultaAsync(Guid id, AtualizarConsultaDto dto)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Data) && DateTime.TryParse(dto.Data, out var data))
        {
            consulta.Data = data;
        }

        if (!string.IsNullOrEmpty(dto.Horario))
        {
            consulta.Horario = TimeSpan.Parse(dto.Horario);
        }

        if (!string.IsNullOrEmpty(dto.HorarioFim))
        {
            consulta.HorarioFim = TimeSpan.Parse(dto.HorarioFim);
        }

        if (!string.IsNullOrEmpty(dto.Tipo) && Enum.TryParse<TipoConsulta>(dto.Tipo, true, out var tipoEnum))
        {
            consulta.Tipo = tipoEnum;
        }

        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<StatusConsulta>(dto.Status, true, out var statusEnum))
        {
            consulta.Status = statusEnum;
        }

        if (dto.Observacao != null)
        {
            consulta.Observacao = dto.Observacao;
        }

        if (dto.CamposEspecialidadeJson != null)
        {
            consulta.CamposEspecialidadeJson = dto.CamposEspecialidadeJson;
        }

        consulta.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await ObterConsultaPorIdAsync(id);
    }

    public async Task<bool> CancelarConsultaAsync(Guid id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null)
        {
            return false;
        }

        consulta.Status = StatusConsulta.Cancelada;
        consulta.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Enviar notificações
        await _notificacaoService.EnviarNotificacaoConsultaCanceladaAsync(
            consulta.PacienteId,
            consulta.ProfissionalId,
            consulta.Data,
            consulta.Horario.ToString(@"hh\:mm"));

        return true;
    }

    public async Task<bool> FinalizarConsultaAsync(Guid id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null)
        {
            return false;
        }

        consulta.Status = StatusConsulta.Realizada;
        consulta.HorarioFim = TimeSpan.Parse(DateTime.UtcNow.ToString("HH:mm"));
        consulta.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IniciarConsultaAsync(Guid id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null)
        {
            return false;
        }

        consulta.Status = StatusConsulta.EmAndamento;
        consulta.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<SlotDisponivelDto>> ObterHorariosDisponiveisAsync(Guid profissionalId, DateTime data)
    {
        var slots = new List<SlotDisponivelDto>();

        var agenda = await _context.Agendas
            .FirstOrDefaultAsync(a => a.ProfissionalId == profissionalId && a.Status == Domain.Enums.StatusEspecialidade.Ativa);

        if (agenda == null || string.IsNullOrEmpty(agenda.ConfiguracaoDiasJson))
        {
            return slots;
        }

        try
        {
            var configuracaoDias = JsonSerializer.Deserialize<List<ConfiguracaoDiaJson>>(agenda.ConfiguracaoDiasJson);
            var diaSemana = (int)data.DayOfWeek;
            var configDia = configuracaoDias?.FirstOrDefault(d => d.DiaSemana == diaSemana && d.Ativo);

            if (configDia?.Horarios == null)
            {
                return slots;
            }

            // Obter consultas já agendadas para o dia
            var consultasExistentes = await _context.Consultas
                .Where(c => c.ProfissionalId == profissionalId &&
                            c.Data.Date == data.Date &&
                            c.Status != StatusConsulta.Cancelada)
                .Select(c => c.Horario.ToString(@"hh\:mm"))
                .ToListAsync();

            // Obter bloqueios
            var bloqueios = await _context.BloqueiosAgenda
                .Where(b => b.ProfissionalId == profissionalId &&
                            b.Status == StatusBloqueioAgenda.Aprovado &&
                            ((b.Data.HasValue && b.Data.Value.Date == data.Date) ||
                             (b.DataInicio.HasValue && b.DataFim.HasValue && 
                              data.Date >= b.DataInicio.Value.Date && data.Date <= b.DataFim.Value.Date)))
                .ToListAsync();

            var duracaoConsulta = 30; // Minutos padrão
            if (!string.IsNullOrEmpty(agenda.ConfiguracaoGlobalJson))
            {
                var configGlobal = JsonSerializer.Deserialize<ConfiguracaoGlobalJson>(agenda.ConfiguracaoGlobalJson);
                duracaoConsulta = configGlobal?.DuracaoConsulta ?? 30;
            }

            foreach (var faixa in configDia.Horarios)
            {
                if (TimeSpan.TryParse(faixa.Inicio, out var inicio) && TimeSpan.TryParse(faixa.Fim, out var fim))
                {
                    var horaAtual = inicio;
                    while (horaAtual.Add(TimeSpan.FromMinutes(duracaoConsulta)) <= fim)
                    {
                        var horarioStr = horaAtual.ToString(@"hh\:mm");
                        var horaFimSlot = horaAtual.Add(TimeSpan.FromMinutes(duracaoConsulta));

                        var disponivel = !consultasExistentes.Contains(horarioStr) && bloqueios.Count == 0;

                        slots.Add(new SlotDisponivelDto
                        {
                            Horario = horarioStr,
                            HorarioFim = horaFimSlot.ToString(@"hh\:mm"),
                            Disponivel = disponivel
                        });

                        horaAtual = horaFimSlot;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter horários disponíveis");
        }

        return slots;
    }

    public async Task<PreConsultaDto?> ObterPreConsultaAsync(Guid consultaId)
    {
        var preConsulta = await _context.PreConsultas.FirstOrDefaultAsync(p => p.ConsultaId == consultaId);
        return preConsulta != null ? MapearPreConsultaParaDto(preConsulta) : null;
    }

    public async Task<PreConsultaDto> SalvarPreConsultaAsync(Guid consultaId, PreConsultaDto dto)
    {
        var preConsulta = await _context.PreConsultas.FirstOrDefaultAsync(p => p.ConsultaId == consultaId);

        if (preConsulta == null)
        {
            preConsulta = new PreConsulta
            {
                ConsultaId = consultaId,
                CriadoEm = DateTime.UtcNow
            };
            _context.PreConsultas.Add(preConsulta);
        }

        preConsulta.NomeCompleto = dto.NomeCompleto;
        preConsulta.DataNascimento = dto.DataNascimento;
        preConsulta.Peso = dto.Peso;
        preConsulta.Altura = dto.Altura;
        preConsulta.CondicoesCronicas = dto.CondicoesCronicas;
        preConsulta.Medicamentos = dto.Medicamentos;
        preConsulta.Alergias = dto.Alergias;
        preConsulta.Cirurgias = dto.Cirurgias;
        preConsulta.ObservacoesHistorico = dto.ObservacoesHistorico;
        preConsulta.Tabagismo = dto.Tabagismo;
        preConsulta.ConsumoAlcool = dto.ConsumoAlcool;
        preConsulta.AtividadeFisica = dto.AtividadeFisica;
        preConsulta.ObservacoesHabitos = dto.ObservacoesHabitos;
        preConsulta.PressaoArterial = dto.PressaoArterial;
        preConsulta.FrequenciaCardiaca = dto.FrequenciaCardiaca;
        preConsulta.Temperatura = dto.Temperatura;
        preConsulta.SaturacaoOxigenio = dto.SaturacaoOxigenio;
        preConsulta.ObservacoesSinaisVitais = dto.ObservacoesSinaisVitais;
        preConsulta.SintomasPrincipais = dto.SintomasPrincipais;
        preConsulta.InicioSintomas = dto.InicioSintomas;
        preConsulta.IntensidadeDor = dto.IntensidadeDor;
        preConsulta.ObservacoesSintomas = dto.ObservacoesSintomas;
        preConsulta.ObservacoesAdicionais = dto.ObservacoesAdicionais;
        preConsulta.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapearPreConsultaParaDto(preConsulta);
    }

    public async Task<AnamneseDto?> ObterAnamneseAsync(Guid consultaId)
    {
        var anamnese = await _context.Anamneses.FirstOrDefaultAsync(a => a.ConsultaId == consultaId);
        return anamnese != null ? MapearAnamneseParaDto(anamnese) : null;
    }

    public async Task<AnamneseDto> SalvarAnamneseAsync(Guid consultaId, AnamneseDto dto)
    {
        var anamnese = await _context.Anamneses.FirstOrDefaultAsync(a => a.ConsultaId == consultaId);

        if (anamnese == null)
        {
            anamnese = new Anamnese
            {
                ConsultaId = consultaId,
                CriadoEm = DateTime.UtcNow
            };
            _context.Anamneses.Add(anamnese);
        }

        anamnese.QueixaPrincipal = dto.QueixaPrincipal;
        anamnese.HistoriaDoencaAtual = dto.HistoriaDoencaAtual;
        anamnese.HistoriaPatologicaPregressa = dto.HistoriaPatologicaPregressa;
        anamnese.HistoriaFamiliar = dto.HistoriaFamiliar;
        anamnese.HabitosVida = dto.HabitosVida;
        anamnese.RevisaoSistemas = dto.RevisaoSistemas;
        anamnese.MedicamentosEmUso = dto.MedicamentosEmUso;
        anamnese.Alergias = dto.Alergias;
        anamnese.Observacoes = dto.Observacoes;
        anamnese.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapearAnamneseParaDto(anamnese);
    }

    public async Task<RegistroSoapDto?> ObterSoapAsync(Guid consultaId)
    {
        var soap = await _context.RegistrosSoap.FirstOrDefaultAsync(s => s.ConsultaId == consultaId);
        return soap != null ? MapearSoapParaDto(soap) : null;
    }

    public async Task<RegistroSoapDto> SalvarSoapAsync(Guid consultaId, RegistroSoapDto dto)
    {
        var soap = await _context.RegistrosSoap.FirstOrDefaultAsync(s => s.ConsultaId == consultaId);

        if (soap == null)
        {
            soap = new RegistroSoap
            {
                ConsultaId = consultaId,
                CriadoEm = DateTime.UtcNow
            };
            _context.RegistrosSoap.Add(soap);
        }

        soap.Subjetivo = dto.Subjetivo;
        soap.Objetivo = dto.Objetivo;
        soap.Avaliacao = dto.Avaliacao;
        soap.Plano = dto.Plano;
        soap.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapearSoapParaDto(soap);
    }

    public async Task<DadosBiometricosDto?> ObterDadosBiometricosAsync(Guid consultaId)
    {
        var dados = await _context.DadosBiometricos.FirstOrDefaultAsync(d => d.ConsultaId == consultaId);
        return dados != null ? MapearDadosBiometricosParaDto(dados) : null;
    }

    public async Task<DadosBiometricosDto> SalvarDadosBiometricosAsync(Guid consultaId, DadosBiometricosDto dto)
    {
        var dados = await _context.DadosBiometricos.FirstOrDefaultAsync(d => d.ConsultaId == consultaId);

        if (dados == null)
        {
            dados = new DadosBiometricos
            {
                ConsultaId = consultaId,
                CriadoEm = DateTime.UtcNow
            };
            _context.DadosBiometricos.Add(dados);
        }

        dados.PressaoArterial = dto.PressaoArterial;
        dados.FrequenciaCardiaca = dto.FrequenciaCardiaca;
        dados.FrequenciaRespiratoria = dto.FrequenciaRespiratoria;
        dados.Temperatura = dto.Temperatura;
        dados.SaturacaoOxigenio = dto.SaturacaoOxigenio;
        dados.Peso = dto.Peso;
        dados.Altura = dto.Altura;
        dados.Imc = dto.Imc;
        dados.CircunferenciaAbdominal = dto.CircunferenciaAbdominal;
        dados.Glicemia = dto.Glicemia;
        dados.TipoGlicemia = dto.TipoGlicemia;
        dados.Observacoes = dto.Observacoes;
        dados.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapearDadosBiometricosParaDto(dados);
    }

    #region Mapeadores

    private static ConsultaDto MapearParaDto(Consulta consulta)
    {
        return new ConsultaDto
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            NomePaciente = consulta.Paciente != null ? $"{consulta.Paciente.Nome} {consulta.Paciente.Sobrenome}".Trim() : null,
            ProfissionalId = consulta.ProfissionalId,
            NomeProfissional = consulta.Profissional != null ? $"{consulta.Profissional.Nome} {consulta.Profissional.Sobrenome}".Trim() : null,
            EspecialidadeId = consulta.EspecialidadeId,
            NomeEspecialidade = consulta.Especialidade?.Nome,
            Data = consulta.Data,
            Horario = consulta.Horario.ToString(@"hh\:mm"),
            HorarioFim = consulta.HorarioFim?.ToString(@"hh\:mm"),
            Tipo = consulta.Tipo.ToString(),
            Status = consulta.Status.ToString(),
            Observacao = consulta.Observacao,
            LinkVideo = consulta.LinkVideo,
            CamposEspecialidadeJson = consulta.CamposEspecialidadeJson,
            ResumoIA = consulta.ResumoIA,
            ResumoIAGeradoEm = consulta.ResumoIAGeradoEm,
            HipoteseDiagnosticaIA = consulta.HipoteseDiagnosticaIA,
            HipoteseDiagnosticaIAGeradaEm = consulta.HipoteseDiagnosticaIAGeradaEm,
            CriadoEm = consulta.CriadoEm,
            AtualizadoEm = consulta.AtualizadoEm,
            PreConsulta = consulta.PreConsulta != null ? MapearPreConsultaParaDto(consulta.PreConsulta) : null,
            Anamnese = consulta.Anamnese != null ? MapearAnamneseParaDto(consulta.Anamnese) : null,
            Soap = consulta.Soap != null ? MapearSoapParaDto(consulta.Soap) : null,
            DadosBiometricos = consulta.DadosBiometricos != null ? MapearDadosBiometricosParaDto(consulta.DadosBiometricos) : null,
            Anexos = consulta.Anexos?.Select(a => new AnexoDto
            {
                Id = a.Id,
                ConsultaId = a.ConsultaId,
                Titulo = a.Titulo,
                NomeArquivo = a.NomeArquivo,
                TipoArquivo = a.TipoArquivo,
                TamanhoArquivo = a.TamanhoArquivo,
                CriadoEm = a.CriadoEm
            }).ToList()
        };
    }

    private static PreConsultaDto MapearPreConsultaParaDto(PreConsulta preConsulta)
    {
        return new PreConsultaDto
        {
            Id = preConsulta.Id,
            ConsultaId = preConsulta.ConsultaId,
            NomeCompleto = preConsulta.NomeCompleto,
            DataNascimento = preConsulta.DataNascimento,
            Peso = preConsulta.Peso,
            Altura = preConsulta.Altura,
            CondicoesCronicas = preConsulta.CondicoesCronicas,
            Medicamentos = preConsulta.Medicamentos,
            Alergias = preConsulta.Alergias,
            Cirurgias = preConsulta.Cirurgias,
            ObservacoesHistorico = preConsulta.ObservacoesHistorico,
            Tabagismo = preConsulta.Tabagismo,
            ConsumoAlcool = preConsulta.ConsumoAlcool,
            AtividadeFisica = preConsulta.AtividadeFisica,
            ObservacoesHabitos = preConsulta.ObservacoesHabitos,
            PressaoArterial = preConsulta.PressaoArterial,
            FrequenciaCardiaca = preConsulta.FrequenciaCardiaca,
            Temperatura = preConsulta.Temperatura,
            SaturacaoOxigenio = preConsulta.SaturacaoOxigenio,
            ObservacoesSinaisVitais = preConsulta.ObservacoesSinaisVitais,
            SintomasPrincipais = preConsulta.SintomasPrincipais,
            InicioSintomas = preConsulta.InicioSintomas,
            IntensidadeDor = preConsulta.IntensidadeDor,
            ObservacoesSintomas = preConsulta.ObservacoesSintomas,
            ObservacoesAdicionais = preConsulta.ObservacoesAdicionais
        };
    }

    private static AnamneseDto MapearAnamneseParaDto(Anamnese anamnese)
    {
        return new AnamneseDto
        {
            Id = anamnese.Id,
            ConsultaId = anamnese.ConsultaId,
            QueixaPrincipal = anamnese.QueixaPrincipal,
            HistoriaDoencaAtual = anamnese.HistoriaDoencaAtual,
            HistoriaPatologicaPregressa = anamnese.HistoriaPatologicaPregressa,
            HistoriaFamiliar = anamnese.HistoriaFamiliar,
            HabitosVida = anamnese.HabitosVida,
            RevisaoSistemas = anamnese.RevisaoSistemas,
            MedicamentosEmUso = anamnese.MedicamentosEmUso,
            Alergias = anamnese.Alergias,
            Observacoes = anamnese.Observacoes
        };
    }

    private static RegistroSoapDto MapearSoapParaDto(RegistroSoap soap)
    {
        return new RegistroSoapDto
        {
            Id = soap.Id,
            ConsultaId = soap.ConsultaId,
            Subjetivo = soap.Subjetivo,
            Objetivo = soap.Objetivo,
            Avaliacao = soap.Avaliacao,
            Plano = soap.Plano
        };
    }

    private static DadosBiometricosDto MapearDadosBiometricosParaDto(DadosBiometricos dados)
    {
        return new DadosBiometricosDto
        {
            Id = dados.Id,
            ConsultaId = dados.ConsultaId,
            PressaoArterial = dados.PressaoArterial,
            FrequenciaCardiaca = dados.FrequenciaCardiaca,
            FrequenciaRespiratoria = dados.FrequenciaRespiratoria,
            Temperatura = dados.Temperatura,
            SaturacaoOxigenio = dados.SaturacaoOxigenio,
            Peso = dados.Peso,
            Altura = dados.Altura,
            Imc = dados.Imc,
            CircunferenciaAbdominal = dados.CircunferenciaAbdominal,
            Glicemia = dados.Glicemia,
            TipoGlicemia = dados.TipoGlicemia,
            Observacoes = dados.Observacoes
        };
    }

    #endregion

    // Classes auxiliares para deserialização JSON
    private class ConfiguracaoDiaJson
    {
        public int DiaSemana { get; set; }
        public bool Ativo { get; set; }
        public List<FaixaHorarioJson>? Horarios { get; set; }
    }

    private class FaixaHorarioJson
    {
        public string Inicio { get; set; } = string.Empty;
        public string Fim { get; set; } = string.Empty;
    }

    private class ConfiguracaoGlobalJson
    {
        public int DuracaoConsulta { get; set; } = 30;
    }
}
