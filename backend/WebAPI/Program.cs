using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<Application.Interfaces.IJwtService, Infrastructure.Services.JwtService>();
builder.Services.AddScoped<Application.Interfaces.IPasswordHasher, Infrastructure.Services.PasswordHasher>();
builder.Services.AddScoped<Application.Interfaces.IAuthService, Infrastructure.Services.AuthService>();
builder.Services.AddScoped<Application.Interfaces.IUserService, Infrastructure.Services.UserService>();
builder.Services.AddScoped<Application.Interfaces.ISpecialtyService, Infrastructure.Services.SpecialtyService>();
builder.Services.AddScoped<Application.Interfaces.IAppointmentService, Infrastructure.Services.AppointmentService>();
builder.Services.AddScoped<Application.Interfaces.INotificationService, Infrastructure.Services.NotificationService>();
builder.Services.AddScoped<Application.Interfaces.IScheduleService, Infrastructure.Services.ScheduleService>();
builder.Services.AddScoped<Application.Interfaces.IReportService, Infrastructure.Services.ReportService>();
builder.Services.AddScoped<Application.Interfaces.IAuditLogService, Infrastructure.Services.AuditLogService>();
builder.Services.AddScoped<Application.Interfaces.IAttachmentService, Infrastructure.Services.AttachmentService>();
builder.Services.AddScoped<Application.Interfaces.IInviteService, Infrastructure.Services.InviteService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    await WebAPI.Data.DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
