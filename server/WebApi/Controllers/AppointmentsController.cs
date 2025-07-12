using BLL.API;
using BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IBL bl, ILogger<AppointmentsController> logger)
        {
            _appointmentService = bl.AppointmentService;
            _logger = logger;
        }

        [HttpGet("byProvider/{doctorName}")]
        public async Task<IActionResult> GetAppointmentsByProviderName(string doctorName)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByProviderNameAsync(doctorName);
                return Ok(new { success = true, data = appointments, count = appointments.Count });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (NoAvailableSlotsException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error getting appointments for doctor: {DoctorName}", doctorName);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for doctor: {DoctorName}", doctorName);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("byProviderCity")]
        public async Task<IActionResult> GetAvailableSlotsByProviderAndCity([FromQuery] string doctorName, [FromQuery] string cityName)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsByProviderAndCityAsync(doctorName, cityName);
                return Ok(new { success = true, data = slots, count = slots.Count });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (NoAvailableSlotsException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error getting slots for doctor: {DoctorName} in city: {CityName}", doctorName, cityName);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting slots for doctor: {DoctorName} in city: {CityName}", doctorName, cityName);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("byService/{serviceId}")]
        public async Task<IActionResult> GetAvailableSlotsByService(int serviceId)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsByServiceAsync(serviceId);
                return Ok(new { success = true, data = slots, count = slots.Count });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (NoAvailableSlotsException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error getting slots for service: {ServiceId}", serviceId);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting slots for service: {ServiceId}", serviceId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("book/{slotId}")]
        public async Task<IActionResult> BookAppointment(int slotId, [FromBody] BookAppointmentRequest request)
        {
            try
            {
                await _appointmentService.BookAppointmentAsync(slotId, request.PatientKey);
                return Ok(new { success = true, message = "Appointment booked successfully" });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
            catch (DoctorNotActiveException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error booking appointment - SlotId: {SlotId}, PatientKey: {PatientKey}", slotId, request.PatientKey);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment - SlotId: {SlotId}, PatientKey: {PatientKey}", slotId, request.PatientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(appointmentId);
                return Ok(new { success = true, message = "Appointment cancelled successfully" });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error cancelling appointment: {AppointmentId}", appointmentId);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment: {AppointmentId}", appointmentId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("byUser/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByUser(string patientId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByUserAsync(patientId);
                return Ok(new { success = true, data = appointments, count = appointments.Count });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error getting appointments for patient: {PatientId}", patientId);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for patient: {PatientId}", patientId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // הוסף endpoint חדש לעבודה עם PatientKey
        [HttpGet("byUserKey/{patientKey}")]
        public async Task<IActionResult> GetAppointmentsByUserKey(int patientKey)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientKey);
                return Ok(new { success = true, data = appointments, count = appointments.Count });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error getting appointments for patient key: {PatientKey}", patientKey);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for patient key: {PatientKey}", patientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("generateSlots")]
        public async Task<IActionResult> GenerateSlotsForProvider([FromQuery] int providerKey, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            try
            {
                await _appointmentService.GenerateSlotsForProviderAsync(providerKey, startDate, endDate);
                return Ok(new { success = true, message = "Slots generated successfully" });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (DoctorNotActiveException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (PastAppointmentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error generating slots for provider: {ProviderKey}", providerKey);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating slots for provider: {ProviderKey}", providerKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        public class BookAppointmentRequest
        {
            public int PatientKey { get; set; } // שונה מ-PatientId ל-PatientKey
        }
    }
}