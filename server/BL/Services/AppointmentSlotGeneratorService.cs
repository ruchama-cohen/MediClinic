using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using DAL.API;
using DAL.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Services
{
    public class AppointmentSlotGeneratorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentSlotGeneratorService> _logger;

        public AppointmentSlotGeneratorService(IServiceProvider serviceProvider, ILogger<AppointmentSlotGeneratorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚀 Background Service Started!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"⏰ Running at {DateTime.Now}");

                    using var scope = _serviceProvider.CreateScope();

                    var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                    var serviceProviderManagement = scope.ServiceProvider.GetRequiredService<IServiceProviderManagement>();

                    var targetDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));
                    var providers = await serviceProviderManagement.GetAllAsync();

                    Console.WriteLine($"👥 Found {providers.Count} active providers");
                    Console.WriteLine($"📅 Target date: {targetDate}");

                    foreach (var provider in providers)
                    {
                        try
                        {
                            Console.WriteLine($"🔄 Processing provider {provider.ProviderKey} ({provider.Name})");

                            bool result = await appointmentService.GenerateSlotsForProviderAsync(provider.ProviderKey, targetDate, targetDate);

                            Console.WriteLine($"✅ Success for {provider.Name}");
                            _logger.LogInformation($"Slots generated for provider {provider.ProviderKey} on {targetDate}");
                        }
                        catch (Exception providerEx)
                        {
                            Console.WriteLine($"❌ Failed for {provider.Name}: {providerEx.Message}");
                            _logger.LogWarning($"Failed to generate slots for provider {provider.ProviderKey} ({provider.Name}): {providerEx.Message}");
                            continue;
                        }
                    }

                    Console.WriteLine($"✅ Finished processing all {providers.Count} providers");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"💥 System Error: {ex.Message}");
                    _logger.LogError(ex, "Error in background service");
                }

                Console.WriteLine("💤 Waiting 24 hours...");
                var delay = TimeSpan.FromDays(1); // ✅ תוקן מ־90 ימים ל־1

                if (delay <= TimeSpan.Zero || delay.TotalMilliseconds > int.MaxValue)
                {
                    _logger.LogWarning("Invalid delay duration, skipping wait.");
                }
                else
                {
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
    }
}
