using Microsoft.AspNetCore.Mvc;
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
            IClinicServiceManagement serviceManagement,
            IServiceProviderManagement providerManagement,
            ILogger<ServiceController> logger)
        {
            _serviceManagement = serviceManagement;
            _providerManagement = providerManagement;
            _logger = logger;
        }

        /// <summary>
        /// קבלת כל השירותים הרפואיים
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            _logger.LogInformation("Getting all medical services");

            var services = await _serviceManagement.GetAllServices();

            return Ok(new
            {
                success = true,
                data = services.Select(s => new ServiceResponse
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName
                }),
                count = services.Count
            });
        }



    }
}