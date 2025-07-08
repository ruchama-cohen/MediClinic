using Microsoft.AspNetCore.Mvc;
using BLL.API;
using DAL.API;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IClinicServiceManagement _serviceManagement;
        private readonly IServiceProviderManagement _providerManagement;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(
            IBL bl, 
            ILogger<ServiceController> logger)
        {
            _serviceManagement = bl.ClinicServiceManagement;
            _providerManagement = bl.ServiceProviderManagement;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            _logger.LogInformation("Getting all medical services");

            var services = await _serviceManagement.GetAllServices();

            return Ok(new
            {
                success = true,
                data = services
                .Where(s => !string.Equals(s.ServiceName, "Branch Manager", StringComparison.OrdinalIgnoreCase))
                .Select(s => new ServiceResponse
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName
                }),
                count = services.Count


            });
        }
    }

    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}