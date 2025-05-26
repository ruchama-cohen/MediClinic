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
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByProviderNameAsync(doctorName);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //קונטרולר שמקבל שם רופא ותורים 
        [HttpGet("byprovidercity")]
        public async Task<IActionResult> GetAvailableSlotsByProviderAndCity([FromQuery] string doctorName, [FromQuery] string cityName)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsByProviderAndCityAsync(doctorName, cityName);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //קונטרולר שמקבל 
        [HttpGet("byservice/{serviceId}")]
        public async Task<IActionResult> GetAvailableSlotsByService(int serviceId)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsByServiceAsync(serviceId);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //ביטול תור לפי ID
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(appointmentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //מחזירה תורים של מטופל לפי השם שלו אולי כדאי לשנות?
        [HttpGet("byuser/{patientName}")]
        public async Task<IActionResult> GetAppointmentsByUser(string patientName)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByUserAsync(patientName);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //קביעת תור
        [HttpPost("book/{slotId}")]
        public async Task<IActionResult> BookAppointment(int slotId, [FromBody] Appointment appointment)
        {
            try
            {
                bool success = await _appointmentService.BookAppointmentAsync(slotId, appointment);
                if (!success)
                    return Conflict("Slot is already booked or unavailable.");

                return Ok("Appointment booked successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //אם פעם נרצה להפעיל את זה בעצמנו
        [HttpPost("generateslots")]
        public async Task<IActionResult> GenerateSlotsForProvider([FromQuery] int providerKey, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            try
            {
                bool success = await _appointmentService.GenerateSlotsForProviderAsync(providerKey, startDate, endDate);
                if (!success)
                    return BadRequest("Failed to generate slots for provider.");

                return Ok("Slots generated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
