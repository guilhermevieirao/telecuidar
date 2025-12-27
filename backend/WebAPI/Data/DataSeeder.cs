using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar se já existem usuários
        if (await context.Usuarios.AnyAsync())
        {
            Console.WriteLine("[SEEDER] Usuários já existem. Pulando seed.");
            return;
        }

        Console.WriteLine("[SEEDER] Iniciando seed de dados...");

        var hashSenha = new HashSenhaService();
        var adminEmail = Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL") ?? "adm@adm.com";
        var adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD") ?? "zxcasd12";
        var adminName = Environment.GetEnvironmentVariable("SEED_ADMIN_NAME") ?? "Admin";
        var adminLastName = Environment.GetEnvironmentVariable("SEED_ADMIN_LASTNAME") ?? "Sistema";
        var adminCpf = Environment.GetEnvironmentVariable("SEED_ADMIN_CPF") ?? "11111111111";
        const string senhaDefault = "zxcasd12";

        // Criar especialidade de Cardiologia com campos personalizados
        var camposPersonalizadosJson = @"[
            {""nome"":""Histórico de Infarto"",""tipo"":""checkbox"",""obrigatorio"":true,""descricao"":""Paciente já teve infarto do miocárdio?"",""ordem"":1},
            {""nome"":""Pressão Arterial Sistólica"",""tipo"":""number"",""obrigatorio"":true,""descricao"":""Pressão arterial sistólica em mmHg"",""valorPadrao"":""120"",""ordem"":2},
            {""nome"":""Pressão Arterial Diastólica"",""tipo"":""number"",""obrigatorio"":true,""descricao"":""Pressão arterial diastólica em mmHg"",""valorPadrao"":""80"",""ordem"":3},
            {""nome"":""Frequência Cardíaca"",""tipo"":""number"",""obrigatorio"":true,""descricao"":""Batimentos por minuto em repouso"",""ordem"":4},
            {""nome"":""Uso de Marca-passo"",""tipo"":""radio"",""obrigatorio"":true,""descricao"":""Paciente faz uso de marca-passo?"",""opcoes"":[""Sim"",""Não""],""ordem"":5},
            {""nome"":""Tipo de Dor Torácica"",""tipo"":""select"",""obrigatorio"":false,""descricao"":""Caso apresente dor torácica, qual o tipo?"",""opcoes"":[""Não apresenta"",""Dor em aperto"",""Dor em queimação"",""Dor em pontada"",""Dor irradiada""],""ordem"":6},
            {""nome"":""Medicamentos Cardiovasculares"",""tipo"":""textarea"",""obrigatorio"":false,""descricao"":""Liste os medicamentos em uso para o coração"",""ordem"":7},
            {""nome"":""Data Último ECG"",""tipo"":""date"",""obrigatorio"":false,""descricao"":""Data do último eletrocardiograma realizado"",""ordem"":8},
            {""nome"":""Histórico Familiar"",""tipo"":""textarea"",""obrigatorio"":false,""descricao"":""Histórico familiar de doenças cardiovasculares"",""ordem"":9},
            {""nome"":""Nível de Colesterol"",""tipo"":""select"",""obrigatorio"":false,""descricao"":""Último exame de colesterol"",""opcoes"":[""Normal"",""Borderline"",""Alto"",""Não sabe""],""ordem"":10}
        ]";

        var cardiologiaEspecialidade = new Especialidade
        {
            Nome = "Cardiologia",
            Descricao = "Especialidade médica dedicada ao diagnóstico e tratamento de doenças do coração e do sistema circulatório, incluindo hipertensão, insuficiência cardíaca, arritmias e doenças coronarianas.",
            Status = StatusEspecialidade.Ativa,
            CamposPersonalizadosJson = camposPersonalizadosJson
        };

        context.Especialidades.Add(cardiologiaEspecialidade);
        await context.SaveChangesAsync();

        Console.WriteLine("[SEEDER] Especialidade criada:");
        Console.WriteLine("  - Cardiologia (com 10 campos personalizados)");

        var usuarios = new List<Usuario>
        {
            new Usuario
            {
                Nome = adminName,
                Sobrenome = adminLastName,
                Email = adminEmail,
                Cpf = adminCpf,
                Telefone = "11911111111",
                SenhaHash = hashSenha.GerarHash(adminPassword),
                Tipo = TipoUsuario.Administrador,
                Status = StatusUsuario.Ativo,
                EmailVerificado = true
            },
            new Usuario
            {
                Nome = "Médico",
                Sobrenome = "Profissional",
                Email = "med@med.com",
                Cpf = "22222222222",
                Telefone = "11922222222",
                SenhaHash = hashSenha.GerarHash(senhaDefault),
                Tipo = TipoUsuario.Profissional,
                Status = StatusUsuario.Ativo,
                EmailVerificado = true
            },
            new Usuario
            {
                Nome = "Paciente",
                Sobrenome = "Dev",
                Email = "pac@pac.com",
                Cpf = "33333333333",
                Telefone = "11933333333",
                SenhaHash = hashSenha.GerarHash(senhaDefault),
                Tipo = TipoUsuario.Paciente,
                Status = StatusUsuario.Ativo,
                EmailVerificado = true
            }
        };

        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();

        // Criar PerfilProfissional para o médico com a especialidade
        var profissional = usuarios.First(u => u.Tipo == TipoUsuario.Profissional);
        var perfilProfissional = new PerfilProfissional
        {
            UsuarioId = profissional.Id,
            EspecialidadeId = cardiologiaEspecialidade.Id,
            Crm = "123456-SP"
        };
        context.PerfisProfissional.Add(perfilProfissional);
        await context.SaveChangesAsync();

        // Criar agenda para o profissional
        var configuracaoGlobalJson = @"{
            ""HorarioInicio"": ""00:00"",
            ""HorarioFim"": ""23:00"",
            ""DuracaoConsulta"": 30,
            ""IntervaloEntreConsultas"": 0
        }";

        var configuracaoDiasJson = @"[
            {""DiaSemana"": ""Segunda"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Terca"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Quarta"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Quinta"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Sexta"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Sabado"", ""Trabalha"": true, ""Personalizado"": false},
            {""DiaSemana"": ""Domingo"", ""Trabalha"": true, ""Personalizado"": false}
        ]";

        var agenda = new Agenda
        {
            ProfissionalId = profissional.Id,
            ConfiguracaoGlobalJson = configuracaoGlobalJson,
            ConfiguracaoDiasJson = configuracaoDiasJson,
            DataInicioValidade = DateTime.Now.AddDays(-2).Date,
            DataFimValidade = null,
            Ativa = true
        };

        context.Agendas.Add(agenda);
        await context.SaveChangesAsync();

        // Criar consulta para o próximo horário disponível
        var paciente = usuarios.First(u => u.Tipo == TipoUsuario.Paciente);
        var agora = DateTime.Now;
        
        // Calcular o próximo horário disponível (arredondar para a próxima meia hora)
        var minutos = agora.Minute;
        var proximoSlotMinutos = minutos < 30 ? 30 : 60;
        var dataHoraConsulta = agora.AddMinutes(proximoSlotMinutos - minutos);
        if (proximoSlotMinutos == 60)
        {
            dataHoraConsulta = dataHoraConsulta.AddHours(1);
            dataHoraConsulta = new DateTime(
                dataHoraConsulta.Year,
                dataHoraConsulta.Month,
                dataHoraConsulta.Day,
                dataHoraConsulta.Hour,
                0,
                0
            );
        }
        else
        {
            dataHoraConsulta = new DateTime(
                dataHoraConsulta.Year,
                dataHoraConsulta.Month,
                dataHoraConsulta.Day,
                dataHoraConsulta.Hour,
                30,
                0
            );
        }

        var consulta = new Consulta
        {
            PacienteId = paciente.Id,
            ProfissionalId = profissional.Id,
            EspecialidadeId = cardiologiaEspecialidade.Id,
            Data = dataHoraConsulta.Date,
            HoraInicio = dataHoraConsulta.TimeOfDay,
            HoraFim = dataHoraConsulta.AddMinutes(30).TimeOfDay,
            Tipo = TipoConsulta.Comum,
            Status = StatusConsulta.Agendada,
            Observacao = "Consulta de exemplo criada pelo seeder"
        };

        context.Consultas.Add(consulta);
        await context.SaveChangesAsync();

        Console.WriteLine("[SEEDER] Seed concluído!");
        Console.WriteLine("[SEEDER] Usuários criados:");
        Console.WriteLine($"  - {adminEmail} (ADMIN) - senha: {adminPassword}");
        Console.WriteLine("  - med@med.com (PROFISSIONAL) - senha: zxcasd12");
        Console.WriteLine("  - pac@pac.com (PACIENTE) - senha: zxcasd12");
        Console.WriteLine("[SEEDER] Agenda criada:");
        Console.WriteLine($"  - Profissional: {profissional.Nome} {profissional.Sobrenome}");
        Console.WriteLine("  - Horário: 00:00 - 23:00 (todos os dias)");
        Console.WriteLine("  - Consultas: 30min, sem intervalo, sem pausa");
        Console.WriteLine($"  - Validade: {agenda.DataInicioValidade:dd/MM/yyyy} - indeterminado");
        Console.WriteLine("[SEEDER] Consulta criada:");
        Console.WriteLine($"  - Paciente: {paciente.Nome} {paciente.Sobrenome}");
        Console.WriteLine($"  - Profissional: {profissional.Nome} {profissional.Sobrenome}");
        Console.WriteLine($"  - Especialidade: {cardiologiaEspecialidade.Nome}");
        Console.WriteLine($"  - Data/Hora: {dataHoraConsulta:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"  - Tipo: Videochamada");
        Console.WriteLine($"  - Status: Agendada");
    }
}
