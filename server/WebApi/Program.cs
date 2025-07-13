using BL;
using BLL.API;
using BLL.Services;
using DAL.API;
using DAL.Models;
using DAL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Models;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<DB_Manager>();
builder.Services.AddScoped<IBL, BlManager>();
builder.Services.AddScoped<IJwtService, JwtService>();

// כל השירותי DAL שהקונטרולרים צריכים ישירות
builder.Services.AddScoped<IPatientsManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new PatientsManagement(db);
});

builder.Services.AddScoped<IClinicServiceManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new ClinicServiceManagement(db);
});

builder.Services.AddScoped<IServiceProviderManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new ServiceProviderManagement(db);
});

builder.Services.AddScoped<IAppointmentManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new AppointmentManagement(db);
});

builder.Services.AddScoped<IAppointmentsSlotManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new AppointmentsSlotManagement(db);
});

// הוסף את BranchManagement
builder.Services.AddScoped<IBranchManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new BranchManagement(db);
});

// הוסף את AddressManagement
builder.Services.AddScoped<IAddressManagement>(provider =>
{
    var db = provider.GetRequiredService<DB_Manager>();
    return new AddressManagement(db);
});

builder.Services.AddScoped<IAppointmentService>(provider =>
{
    var appointmentManagement = provider.GetRequiredService<IAppointmentManagement>();
    var appointmentsSlotManagement = provider.GetRequiredService<IAppointmentsSlotManagement>();
    var serviceProviderManagement = provider.GetRequiredService<IServiceProviderManagement>();
    var patientsManagement = provider.GetRequiredService<IPatientsManagement>();
    return new AppointmentService(appointmentManagement, appointmentsSlotManagement, serviceProviderManagement, patientsManagement);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001") // כתובות ה-React app
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // חשוב עבור JWT tokens
    });
});

builder.Services.AddLogging();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Clinic Management System API is running!");

app.Run();