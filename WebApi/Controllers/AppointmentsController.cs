using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        [HttpGet("clinics")] // GET api/appointments/clinics?doctorName=...
        public async Task<IActionResult> GetClinics([FromQuery] string doctorName)
        {
            try
            {
                // קריאה ל-BLL לקבלת המרפאות
                var clinics = await _appointmentService.GetClinicsByDoctorAsync(doctorName);
                return Ok(clinics); // מחזיר 200 OK עם הרשימה
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // מחזיר 400 עם הודעת שגיאה
            }
        }

        // פעולה שמחזירה את התורים של הרופא, עם סינון אופציונלי לפי מרפאה
        [HttpGet("appointments")] // GET api/appointments/appointments?doctorName=...&clinicName=...
        public async Task<IActionResult> GetAppointments([FromQuery] string doctorName, [FromQuery] string clinicName = null)
        {
            try
            {
                // קריאה ל-BLL לקבלת התורים
                var appointments = await _appointmentService.GetAppointmentsByDoctorFilteredAsync(doctorName, clinicName);
                return Ok(appointments); // מחזיר 200 OK עם התורים
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // מחזיר 400 עם שגיאה
            }
        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var success = await _appointmentService.CancelAppointmentAsync(id);
            if (success)
                return Ok(new { message = "Appointment canceled successfully." });

            return NotFound(new { message = "Appointment not found." });
        }
    }
}
