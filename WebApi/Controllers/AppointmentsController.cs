using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        [HttpGet("GetAllAppointmentByNameOfServiceProvider")]
        public IActionResult GetServiceProviderByName(string name)
        {
            // Simulate fetching data from a database
            var serviceProviders = new List<string> { "John Doe", "Jane Smith", "Alice Johnson" };
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
