using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        [HttpGet("AllAppointmentByNameOfServiceProvider")]
        public IActionResult GetServiceProviderByName(string name)
        {
            var serviceProviders =
            // Filter the list based on the provided name
            var filteredProviders = serviceProviders.Where(sp => sp.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredProviders.Count == 0)
            {
                return NotFound("No service providers found with the given name.");
            }
            return Ok(filteredProviders);
        }


    }
}
