using app.Domain.Entities;
using app.Domain.Enums;
using app.Domain.Interfaces;
using app.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace app.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Verificar se seed do admin está habilitado
            var adminEnabled = _configuration.GetValue<bool>("AdminUser:Enabled", false);
            
            if (!adminEnabled)
            {
                _logger.LogInformation("ℹ️  Seed desabilitado (AdminUser:Enabled=false)");
                return;
            }

            _logger.LogInformation("🌱 Iniciando seed do banco de dados...");

            // Seed Admin User
            await SeedAdminUserAsync();

            // Seed Test Users (dev only)
            await SeedTestUsersAsync();

            // Seed Specialties
            await SeedSpecialtiesAsync();

            // Seed User Specialties
            await SeedUserSpecialtiesAsync();

            // Seed Schedules
            await SeedSchedulesAsync();

            // Seed Test Appointments
            await SeedTestAppointmentsAsync();

            _logger.LogInformation("✅ Seed concluído com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar seed do banco de dados");
            throw;
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = _configuration["AdminUser:Email"] ?? "adm@adm.com";
        
        var adminExists = await _context.Users.AnyAsync(u => u.Email == adminEmail);
        if (adminExists)
        {
            _logger.LogInformation($"⏭️  Admin já existe: {adminEmail}");
            return;
        }

        var adminPassword = _configuration["AdminUser:Password"] ?? "zxcasd";
        var adminFirstName = _configuration["AdminUser:FirstName"] ?? "adm";
        var adminLastName = _configuration["AdminUser:LastName"] ?? "adm";

        var adminUser = new User
        {
            FirstName = adminFirstName,
            LastName = adminLastName,
            Email = adminEmail,
            PasswordHash = _passwordHasher.HashPassword(adminPassword),
            Role = UserRole.Administrador,
            EmailConfirmed = true,
            IsActive = true
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("========================================");
        _logger.LogInformation("✅ Usuário administrador criado:");
        _logger.LogInformation($"   Email: {adminEmail}");
        _logger.LogInformation($"   ⚠️  ATENÇÃO: Altere a senha padrão!");
        _logger.LogInformation("========================================");
    }

    private async Task SeedTestUsersAsync()
    {
        var usersToSeed = new[]
        {
            new { FirstName = "gui", LastName = "gui", Email = "gui@gui.com", Password = "zxcasd", Role = UserRole.Paciente },
            new { FirstName = "med", LastName = "med", Email = "med@med.com", Password = "zxcasd", Role = UserRole.Profissional }
        };

        foreach (var userSeed in usersToSeed)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == userSeed.Email);
            if (exists)
            {
                _logger.LogInformation($"⏭️  Usuário já existe: {userSeed.Email}");
                continue;
            }

            var user = new User
            {
                FirstName = userSeed.FirstName,
                LastName = userSeed.LastName,
                Email = userSeed.Email,
                PasswordHash = _passwordHasher.HashPassword(userSeed.Password),
                Role = userSeed.Role,
                EmailConfirmed = true,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"✅ Usuário criado: {user.Email}");
        }
    }

    private async Task SeedSpecialtiesAsync()
    {
        var cardiologyName = "Cardiologia";
        var cardiology = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == cardiologyName);
        
        if (cardiology != null)
        {
            _logger.LogInformation($"⏭️  Especialidade já existe: {cardiologyName}");
            return;
        }

        cardiology = new Specialty
        {
            Name = cardiologyName,
            Description = "Especialidade focada em doenças cardiovasculares",
            Icon = "fas fa-heartbeat"
        };

        _context.Specialties.Add(cardiology);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Especialidade criada: {cardiology.Name}");

        // Criar campos personalizados para Cardiologia
        var cardiologyFields = new[]
        {
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "pressao_arterial",
                Label = "Pressão Arterial",
                Description = "Pressão arterial sistólica/diastólica (ex: 120/80)",
                FieldType = "text",
                IsRequired = true,
                DisplayOrder = 1,
                Placeholder = "120/80 mmHg",
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "frequencia_cardiaca",
                Label = "Frequência Cardíaca",
                Description = "Batimentos por minuto (bpm)",
                FieldType = "number",
                IsRequired = true,
                DisplayOrder = 2,
                Placeholder = "75",
                ValidationRules = "{\"min\": 40, \"max\": 200}",
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "ritmo_cardiaco",
                Label = "Ritmo Cardíaco",
                Description = "Avaliação do ritmo cardíaco",
                FieldType = "select",
                Options = "[\"Regular\", \"Irregular\", \"Arritmia\"]",
                IsRequired = true,
                DisplayOrder = 3,
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "ausculta_cardiaca",
                Label = "Ausculta Cardíaca",
                Description = "Descrição dos sons cardíacos auscultados",
                FieldType = "textarea",
                IsRequired = false,
                DisplayOrder = 4,
                Placeholder = "Descreva os sons cardíacos...",
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "dor_toracica",
                Label = "Dor Torácica",
                Description = "Paciente relata dor torácica?",
                FieldType = "radio",
                Options = "[\"Sim\", \"Não\"]",
                IsRequired = true,
                DisplayOrder = 5,
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "dispneia",
                Label = "Dispneia",
                Description = "Presença de falta de ar",
                FieldType = "checkbox",
                IsRequired = false,
                DisplayOrder = 6,
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "edema_perifericos",
                Label = "Edema em Membros",
                Description = "Presença de edema nos membros inferiores",
                FieldType = "checkbox",
                IsRequired = false,
                DisplayOrder = 7,
                IsActive = true
            },
            new SpecialtyField
            {
                SpecialtyId = cardiology.Id,
                FieldName = "classificacao_nyha",
                Label = "Classificação NYHA",
                Description = "Classificação funcional da insuficiência cardíaca",
                FieldType = "select",
                Options = "[\"Classe I - Sem limitação\", \"Classe II - Limitação leve\", \"Classe III - Limitação moderada\", \"Classe IV - Limitação grave\"]",
                IsRequired = false,
                DisplayOrder = 8,
                IsActive = true
            }
        };

        foreach (var field in cardiologyFields)
        {
            _context.SpecialtyFields.Add(field);
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ {cardiologyFields.Length} campos personalizados criados para Cardiologia");
    }

    private async Task SeedUserSpecialtiesAsync()
    {
        var medUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "med@med.com");
        var cardiology = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == "Cardiologia");

        if (medUser == null || cardiology == null)
        {
            _logger.LogInformation("⏭️  Médico ou especialidade não encontrados para associação");
            return;
        }

        var hasSpecialty = await _context.UserSpecialties
            .AnyAsync(us => us.UserId == medUser.Id && us.SpecialtyId == cardiology.Id);

        if (hasSpecialty)
        {
            _logger.LogInformation($"⏭️  Especialidade já atribuída ao médico: {medUser.Email}");
            return;
        }

        var userSpecialty = new UserSpecialty { UserId = medUser.Id, SpecialtyId = cardiology.Id };
        _context.UserSpecialties.Add(userSpecialty);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Especialidade atribuída ao médico: {medUser.Email}");
    }

    private async Task SeedSchedulesAsync()
    {
        var medUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "med@med.com");

        if (medUser == null)
        {
            _logger.LogInformation("⏭️  Médico não encontrado para criar agenda");
            return;
        }

        var hasSchedule = await _context.Schedules.AnyAsync(s => s.ProfessionalId == medUser.Id);
        if (hasSchedule)
        {
            _logger.LogInformation($"⏭️  Agenda já existe para o médico: {medUser.Email}");
            return;
        }

        var yesterday = DateTime.Now.Date.AddDays(-1);
        var schedule = new Schedule
        {
            ProfessionalId = medUser.Id,
            StartDate = yesterday,
            EndDate = null,
            CreatedByUserId = medUser.Id,
            IsActive = true
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        // Cria dias da semana (domingo a sábado)
        for (int day = 0; day <= 6; day++)
        {
            var scheduleDay = new ScheduleDay
            {
                ScheduleId = schedule.Id,
                DayOfWeek = day,
                StartTime = new TimeSpan(0, 0, 0), // 00:00
                EndTime = new TimeSpan(23, 59, 0), // 23:59
                AppointmentDuration = 30,
                IntervalBetweenAppointments = 0
            };
            _context.ScheduleDays.Add(scheduleDay);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Agenda criada para o cardiologista: {medUser.Email}");
    }

    private async Task SeedTestAppointmentsAsync()
    {
        var guiUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "gui@gui.com");
        var medUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "med@med.com");
        var cardiology = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == "Cardiologia");

        if (guiUser == null || medUser == null || cardiology == null)
        {
            _logger.LogInformation("⏭️  Usuários ou especialidade não encontrados para criar consulta de teste");
            return;
        }

        var testAppointmentExists = await _context.Appointments.AnyAsync(a =>
            a.PatientId == guiUser.Id &&
            a.ProfessionalId == medUser.Id &&
            a.SpecialtyId == cardiology.Id &&
            a.AppointmentDate.Year == 2026);

        if (testAppointmentExists)
        {
            _logger.LogInformation("⏭️  Consulta de teste já existe");
            return;
        }

        var appointmentDate = new DateTime(2026, 3, 15); // 15 de março de 2026
        var appointmentTime = new TimeSpan(14, 30, 0); // 14:30

        var dateStr = appointmentDate.ToString("yyyyMMdd");
        var timeStr = appointmentTime.ToString(@"hhmm");
        var meetingRoomId = $"TC-{guiUser.Id}-{medUser.Id}-{dateStr}-{timeStr}";

        var testAppointment = new Appointment
        {
            PatientId = guiUser.Id,
            ProfessionalId = medUser.Id,
            SpecialtyId = cardiology.Id,
            AppointmentDate = appointmentDate,
            AppointmentTime = appointmentTime,
            DurationMinutes = 30,
            Status = "Agendado",
            MeetingRoomId = meetingRoomId,
            Notes = "Consulta de teste - Cardiologia"
        };

        _context.Appointments.Add(testAppointment);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Consulta criada para {guiUser.Email} em Cardiologia - {appointmentDate:dd/MM/yyyy} {appointmentTime:hh\\:mm} (Sala: {meetingRoomId})");
    }
}
