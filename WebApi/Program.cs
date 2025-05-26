using BL;
using BLL.API;
using BLL.Services;
using DAL.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DB_Manager>();
builder.Services.AddScoped<IBL, BlManager>();
builder.Services.AddControllers();
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
app.MapControllers();
app.Run();



