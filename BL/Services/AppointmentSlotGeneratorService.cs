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


namespace BLL.Services
{
    public class AppointmentSlotGeneratorService : BackgroundService
    {
            private readonly IAppointmentService _appointmentService;
            private readonly IServiceProviderManagement _serviceProviderManagementDal;
            private readonly IAppointmentManagement _appointmentManagement;
            private readonly IAppointmentsSlotManagement _appointmentsSlotManagementDal;
            private readonly IPatientsManagement _patientsManagementDal;
            private readonly ILogger<AppointmentSlotGeneratorService> _logger;

            public AppointmentSlotGeneratorService(
                IAppointmentService appointmentService,IAppointmentManagement appointmentManagement, IAppointmentsSlotManagement appointmentsSlotManagementDal, IServiceProviderManagement serviceProviderManagementDal, IPatientsManagement patientsManagementDal,
                ILogger<AppointmentSlotGeneratorService> logger)
            {
                _appointmentService = appointmentService;
                _serviceProviderManagementDal = serviceProviderManagementDal;
                _appointmentManagement = appointmentManagement;
                _appointmentsSlotManagementDal = appointmentsSlotManagementDal;
                _patientsManagementDal = patientsManagementDal;
                _logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var targetDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));
                        var providers = await _serviceProviderManagementDal.GetAllAsync();

                        foreach (var provider in providers)
                        {
                            bool result = await _appointmentService.GenerateSlotsForProviderAsync(provider.Id, targetDate, targetDate);
                            if (result)
                                _logger.LogInformation($"Slots generated for provider {provider.Id} on {targetDate}");
                            else
                                _logger.LogWarning($"Failed to generate slots for provider {provider.Id} on {targetDate}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating appointment slots");
                    }

                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
            }
        }
    }

