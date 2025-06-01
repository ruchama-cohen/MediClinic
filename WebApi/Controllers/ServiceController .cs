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

        /// <summary>
        /// קבלת שירות רפואי לפי ID
        /// </summary>
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetService(int serviceId)
        {
            _logger.LogInformation("Getting service with ID: {ServiceId}", serviceId);

            if (serviceId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid service ID" });
            }

            var service = await _serviceManagement.GetServiceById(serviceId);
            if (service == null)
            {
                return NotFound(new { success = false, message = "Service not found" });
            }

            return Ok(new
            {
                success = true,
                data = new ServiceResponse
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName
                }
            });
        }

        /// <summary>
        /// קבלת רופאים לפי שירות רפואי
        /// </summary>
        [HttpGet("{serviceId}/providers")]
        public async Task<IActionResult> GetProvidersByService(int serviceId)
        {
            _logger.LogInformation("Getting providers for service ID: {ServiceId}", serviceId);

            if (serviceId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid service ID" });
            }

            var providers = await _providerManagement.GetAllServiceProvidersByServiceId(serviceId);

            return Ok(new
            {
                success = true,
                data = providers.Select(p => new ProviderResponse
                {
                    ProviderKey = p.ProviderKey,
                    ProviderId = p.ProviderId,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive,
                    MeetingTime = p.MeetingTime,
                    ServiceId = p.ServiceId,
                    BranchId = p.BranchId
                }),
                serviceId = serviceId,
                count = providers.Count
            });
        }

        /// <summary>
        /// חיפוש רופאים לפי שם
        /// </summary>
        [HttpGet("providers/search")]
        public async Task<IActionResult> SearchProviderByName([FromQuery] string name)
        {
            _logger.LogInformation("Searching for provider with name: {ProviderName}", name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { success = false, message = "Provider name is required" });
            }

            var providerKey = await _providerManagement.GetProviderKeyByName(name);
            if (providerKey <= 0)
            {
                return NotFound(new { success = false, message = "Provider not found" });
            }

            var provider = await _providerManagement.GetProviderWithWorkHoursAsync(providerKey);
            if (provider == null)
            {
                return NotFound(new { success = false, message = "Provider not found" });
            }

            return Ok(new
            {
                success = true,
                data = new ProviderDetailResponse
                {
                    ProviderKey = provider.ProviderKey,
                    ProviderId = provider.ProviderId,
                    Name = provider.Name,
                    Email = provider.Email,
                    Phone = provider.Phone,
                    IsActive = provider.IsActive,
                    MeetingTime = provider.MeetingTime,
                    ServiceId = provider.ServiceId,
                    BranchId = provider.BranchId,
                    WorkHours = provider.WorkHours?.Select(wh => new WorkHourResponse
                    {
                        WorkHourId = wh.WorkHourId,
                        Weekday = wh.Weekday,
                        StartTime = wh.StartTime.ToString(),
                        EndTime = wh.EndTime.ToString(),
                        BranchId = wh.BranchId
                    }).ToList() ?? new List<WorkHourResponse>()
                }
            });
        }

        /// <summary>
        /// קבלת כל הרופאים הפעילים
        /// </summary>
        [HttpGet("providers")]
        public async Task<IActionResult> GetAllActiveProviders()
        {
            _logger.LogInformation("Getting all active providers");

            var providers = await _providerManagement.GetAllAsync();

            return Ok(new
            {
                success = true,
                data = providers.Select(p => new ProviderResponse
                {
                    ProviderKey = p.ProviderKey,
                    ProviderId = p.ProviderId,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive,
                    MeetingTime = p.MeetingTime,
                    ServiceId = p.ServiceId,
                    BranchId = p.BranchId
                }),
                count = providers.Count
            });
        }

        /// <summary>
        /// הוספת שירות רפואי חדש
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] CreateServiceRequest request)
        {
            _logger.LogInformation("Adding new medical service: {ServiceName}", request.ServiceName);

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data provided",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var service = new ClinicService { ServiceName = request.ServiceName.Trim() };
            await _serviceManagement.AddClinicService(service);

            _logger.LogInformation("Medical service added successfully: {ServiceName}", service.ServiceName);

            return CreatedAtAction(nameof(GetAllServices), new
            {
                success = true,
                message = "Medical service added successfully",
                data = new ServiceResponse
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName
                }
            });
        }

        /// <summary>
        /// מחיקת שירות רפואי
        /// </summary>
        [HttpDelete("{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            _logger.LogInformation("Deleting medical service ID: {ServiceId}", serviceId);

            if (serviceId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid service ID" });
            }

            var success = await _serviceManagement.DeleteClinicService(serviceId);

            if (!success)
            {
                return NotFound(new { success = false, message = "Service not found or cannot be deleted" });
            }

            _logger.LogInformation("Medical service deleted successfully: {ServiceId}", serviceId);

            return Ok(new
            {
                success = true,
                message = "Medical service deleted successfully",
                serviceId = serviceId
            });
        }
    }

    // Response Models
    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }

    public class ProviderResponse
    {
        public int ProviderKey { get; set; }
        public string ProviderId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? MeetingTime { get; set; }
        public int ServiceId { get; set; }
        public int BranchId { get; set; }
    }

    public class ProviderDetailResponse : ProviderResponse
    {
        public List<WorkHourResponse> WorkHours { get; set; } = new List<WorkHourResponse>();
    }

    public class WorkHourResponse
    {
        public int WorkHourId { get; set; }
        public int Weekday { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }

    // Request Models
    public class CreateServiceRequest
    {
        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Service name must be between 2 and 100 characters")]
        public string ServiceName { get; set; } = string.Empty;
    }
}