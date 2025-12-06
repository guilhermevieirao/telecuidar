using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using app.Api.Behaviors;
using app.Application.Common.Behaviors;
using app.Application.Common.Interfaces;
using app.Application.Reports.Services;
using app.Domain.Entities;
using app.Domain.Enums;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using app.Infrastructure.Repositories;
using app.Infrastructure.Services;
using MediatR;
using System.Reflection;
using DotNetEnv;

// Carrega variáveis de ambiente do arquivo .env
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine("✅ Arquivo .env carregado com sucesso");
}
else
{
    Console.WriteLine("⚠️  Arquivo .env não encontrado, usando apenas appsettings.json");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Garantir que variáveis de ambiente sejam lidas
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TeleCuidar API",
        Version = "v1",
        Description = "API do sistema TeleCuidar - Plataforma de telemedicina híbrida",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe TeleCuidar",
            Email = "contato@telecuidar.com"
        }
    });

    // JWT Authentication
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Entity Framework

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("app.Infrastructure")));
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

// Register services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDateTimeService, DateTimeService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfExportService, PdfExportService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<ICadsusService, CadsusService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); // Adicionar HttpClientFactory

// Register MediatR with Behaviors
var applicationAssembly = AppDomain.CurrentDomain.Load("app.Application");
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Environment.GetEnvironmentVariable("Jwt__Secret") 
    ?? jwtSettings["Secret"] 
    ?? throw new InvalidOperationException("JWT Secret não configurado. Configure via variável de ambiente Jwt__Secret ou appsettings.json");

// Validar tamanho mínimo da chave
if (secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT Secret deve ter no mínimo 32 caracteres");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS
var frontendUrl = builder.Configuration["FrontendUrl"] 
    ?? Environment.GetEnvironmentVariable("FrontendUrl")
    ?? Environment.GetEnvironmentVariable("FRONTEND_URL")
    ?? "http://localhost:4200";

var allowedOrigins = new List<string> 
{ 
    "http://localhost:4200",
    "http://localhost:4200",
};

Console.WriteLine($"🌐 FrontendUrl configurado: {frontendUrl}");

// Adicionar FrontendUrl se for diferente de localhost
if (!string.IsNullOrEmpty(frontendUrl) && !frontendUrl.Contains("localhost"))
{
    allowedOrigins.Add(frontendUrl);
    
    Console.WriteLine($"✅ Adicionado origin: {frontendUrl}");
    
    // Adicionar variações do domínio
    if (frontendUrl.StartsWith("https://www."))
    {
        // Adicionar sem www
        var withoutWww = frontendUrl.Replace("https://www.", "https://");
        allowedOrigins.Add(withoutWww);
        Console.WriteLine($"✅ Adicionado origin (sem www): {withoutWww}");
    }
    else if (frontendUrl.StartsWith("https://"))
    {
        // Adicionar com www
        var domain = frontendUrl.Replace("https://", "");
        var withWww = $"https://www.{domain}";
        allowedOrigins.Add(withWww);
        Console.WriteLine($"✅ Adicionado origin (com www): {withWww}");
    }
    
    // Adicionar também versão HTTP para desenvolvimento
    if (frontendUrl.StartsWith("https://"))
    {
        var httpVersion = frontendUrl.Replace("https://", "http://");
        allowedOrigins.Add(httpVersion);
        Console.WriteLine($"✅ Adicionado origin (http): {httpVersion}");
    }
}

Console.WriteLine($"🔓 CORS configurado com {allowedOrigins.Count} origins permitidos");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins(allowedOrigins.ToArray())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); // Permite credenciais para cookies CSRF
});

// Add Antiforgery for CSRF protection
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.HttpOnly = false; // Allow client to read for SPA
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Apply database migrations and seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Aplicar migrations pendentes
        context.Database.Migrate();
        logger.LogInformation("✅ Migrations aplicadas com sucesso");
        
        // Executar seed
        var seeder = new DatabaseSeeder(context, passwordHasher, configuration, services.GetRequiredService<ILogger<DatabaseSeeder>>());
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao aplicar migrations ou seed do banco de dados");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeleCuidar API v1");
        c.RoutePrefix = "api-docs"; // Swagger UI em /api-docs
        c.DocumentTitle = "TeleCuidar API Documentation";
    });
}

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});

var enableHttpsRedirect = (Environment.GetEnvironmentVariable("ENABLE_HTTPS_REDIRECT")
    ?? builder.Configuration["ENABLE_HTTPS_REDIRECT"] ?? "true").ToLower() == "true";
if (enableHttpsRedirect)
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

app.Run();
