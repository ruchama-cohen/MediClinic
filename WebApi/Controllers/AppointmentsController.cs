using BLL.API;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentsController(IBL appointmentService)
        {
            _appointmentService = _appointmentService;
        }

        //קונטרולר שמקבל שם רופא ומחזיר את התורים הזמינים של אותו רופא
        [HttpGet("byprovider/{doctorName}")]
        public async Task<IActionResult> GetAppointmentsByProviderName(string doctorName)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
                return BadRequest("Doctor name is required.");

            var appointments = await _appointmentService.GetAppointmentsByProviderNameAsync(doctorName);
            if (appointments == null || !appointments.Any())
                return NotFound($"No appointments found for provider '{doctorName}'.");

            return Ok(appointments);
        }

        //קונטרולר שמקבל שם רופא ותורים 
        [HttpGet("byprovidercity")]
        public async Task<IActionResult> GetAvailableSlotsByProviderAndCity([FromQuery] string doctorName, [FromQuery] string cityName)
        {
            if (string.IsNullOrWhiteSpace(doctorName) || string.IsNullOrWhiteSpace(cityName))
                return BadRequest("Doctor name and city name are required.");

            var slots = await _appointmentService.GetAvailableSlotsByProviderAndCityAsync(doctorName, cityName);
            return Ok(slots);
        }


        //קונטרולר שמקבל 
        [HttpGet("byservice/{serviceId}")]
        public async Task<IActionResult> GetAvailableSlotsByService(int serviceId)
        {
            if (serviceId <= 0)
                return BadRequest("Invalid service ID.");

            var slots = await _appointmentService.GetAvailableSlotsByServiceAsync(serviceId);
            return Ok(slots);
        }



        //ביטול תור לפי ID
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            if (appointmentId <= 0)
                return BadRequest("Invalid appointment ID.");

            await _appointmentService.CancelAppointmentAsync(appointmentId);
            return NoContent();
        }


        //מחזירה תורים של מטופל לפי השם שלו אולי כדאי לשנות?
        [HttpGet("byuser/{patientName}")]
        public async Task<IActionResult> GetAppointmentsByUser(string patientName)
        {
            if (string.IsNullOrWhiteSpace(patientName))
                return BadRequest("Patient name is required.");

            var appointments = await _appointmentService.GetAppointmentsByUserAsync(patientName);
            return Ok(appointments);
        }


        //קביעת תור
        [HttpPost("book/{slotId}")]
        public async Task<IActionResult> BookAppointment(int slotId, [FromBody] int appointment)
        {
            if (slotId <= 0 || appointment <= 0)
                return BadRequest("Invalid slot ID or appointment data.");

            bool success = await _appointmentService.BookAppointmentAsync(slotId, appointment);
            if (!success)
                return Conflict("Slot is already booked or unavailable.");

            return Ok("Appointment booked successfully.");
        }


        //אם פעם נרצה להפעיל את זה בעצמנו
        [HttpPost("generateslots")]
        public async Task<IActionResult> GenerateSlotsForProvider([FromQuery] int providerKey, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            if (providerKey <= 0)
                return BadRequest("Invalid provider key.");

            if (startDate > endDate)
                return BadRequest("Start date must be before end date.");

            bool success = await _appointmentService.GenerateSlotsForProviderAsync(providerKey, startDate, endDate);
            if (!success)
                return BadRequest("Failed to generate slots for provider.");

            return Ok("Slots generated successfully.");
        }
    }
}
