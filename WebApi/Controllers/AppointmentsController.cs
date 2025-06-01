using BLL.API;
using BLL.Exceptions;
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

        public AppointmentsController(IBL bl) 
        {
            _appointmentService = bl.AppointmentService; 
        }

       
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

        [HttpGet("byprovidercity")]
        public async Task<IActionResult> GetAvailableSlotsByProviderAndCity([FromQuery] string doctorName, [FromQuery] string cityName)
        {
            if (string.IsNullOrWhiteSpace(doctorName) || string.IsNullOrWhiteSpace(cityName))
                return BadRequest("Doctor name and city name are required.");

            var slots = await _appointmentService.GetAvailableSlotsByProviderAndCityAsync(doctorName, cityName);
            return Ok(slots);
        }

        [HttpGet("byservice/{serviceId}")]
        public async Task<IActionResult> GetAvailableSlotsByService(int serviceId)
        {
            if (serviceId <= 0)
                return BadRequest("Invalid service ID.");

            var slots = await _appointmentService.GetAvailableSlotsByServiceAsync(serviceId);
            return Ok(slots);
        }

        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                if (appointmentId <= 0)
                    return BadRequest("Invalid appointment ID.");

                await _appointmentService.CancelAppointmentAsync(appointmentId);
                return Ok(new
                {
                    success = true,
                    message = "Appointment cancelled successfully.",
                    appointmentId = appointmentId
                });
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error cancelling appointment." });
            }
        }

        [HttpGet("byuser/{PatientId}")]
        public async Task<IActionResult> GetAppointmentsByUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Patient name is required.");

            var appointments = await _appointmentService.GetAppointmentsByUserAsync(id);
            return Ok(appointments);
        }

        [HttpPost("book/{slotId}")]
        public async Task<IActionResult> BookAppointment(int slotId, [FromBody] BookAppointmentRequest request)
        {
            try
            {
                if (slotId <= 0)
                    return BadRequest("Invalid slot ID.");

                if (request == null || string.IsNullOrWhiteSpace(request.PatientId))
                    return BadRequest("Patient ID is required.");

                bool success = await _appointmentService.BookAppointmentAsync(slotId, request.PatientId);

                return Ok(new
                {
                    success = true,
                    message = "Appointment booked successfully.",
                    slotId = slotId,
                    patientId = request.PatientId
                });
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (SlotNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (SlotAlreadyBookedException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (TimeConflictException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (PastAppointmentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DailyLimitExceededException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        public class BookAppointmentRequest
        {
            public string PatientId { get; set; } = string.Empty;
        }

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
