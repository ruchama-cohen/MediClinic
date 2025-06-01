// ===== Program.cs המלא והמתוקן =====

using BL;
using BLL.API;
using BLL.Services;
using DAL.Models;
using DAL.API;
using DAL.Services;
using WebAPI.Services;
using WebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== רישום כל השירותים =====

// רישום DbContext
builder.Services.AddDbContext<DB_Manager>();

// רישום שירותי DAL
builder.Services.AddScoped<IPatientsManagement, PatientsManagement>();
builder.Services.AddScoped<IAppointmentManagement, AppointmentManagement>();
builder.Services.AddScoped<IAppointmentsSlotManagement, AppointmentsSlotManagement>();
builder.Services.AddScoped<IServiceProviderManagement, ServiceProviderManagement>();
builder.Services.AddScoped<IAddressManagement, AddressManagement>();
builder.Services.AddScoped<IBranchManagement, BranchManagement>();
builder.Services.AddScoped<IClinicServiceManagement, ClinicServiceManagement>();
builder.Services.AddScoped<IWorkHourManagement, WorkHourManagement>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IBL, BlManager>();
builder.Services.AddScoped<IJwtService, JwtService>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings not found in configuration");
}
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();