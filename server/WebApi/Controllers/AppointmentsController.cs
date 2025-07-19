using BLL.API;
using BLL.Exceptions;
using DAL.API;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceProviderManagement _serviceProviderManagement;
        private readonly IClinicServiceManagement _clinicServiceManagement;
        private readonly IBranchManagement _branchManagement;
        private readonly IAppointmentsSlotManagement _appointmentsSlotManagement; // הוסף את זה
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IBL bl, ILogger<AppointmentsController> logger)
        {
            _appointmentService = bl.AppointmentService;
            _serviceProviderManagement = bl.ServiceProviderManagement;
            _clinicServiceManagement = bl.ClinicServiceManagement;
            _branchManagement = bl.BranchManagement;
            _appointmentsSlotManagement = bl.AppointmentsSlotManagement; // הוסף את זה
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAvailableSlots([FromBody] SearchSlotsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid search parameters" });
                }

                // קבלת כל הסלוטים לשירות עם כל הפרטים הנדרשים
                var allSlots = await _appointmentsSlotManagement.GetAppointmentSlotByServiceTypeWithDetails(request.ServiceId);

                if (allSlots == null || allSlots.Count == 0)
                {
                    return Ok(new
                    {
                        success = true,
                        data = new List<object>(),
                        count = 0,
                        message = "No slots found for this service"
                    });
                }

                // סינון רק תורים פנויים ועתידיים
                var availableSlots = allSlots
                    .Where(s => !s.IsBooked && s.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .ToList();

                _logger.LogInformation($"Found {availableSlots.Count} available slots out of {allSlots.Count} total slots");

                // סינון לפי ספק שירות אם נבחר
                if (request.ProviderKey.HasValue)
                {
                    availableSlots = availableSlots.Where(s => s.ProviderKey == request.ProviderKey.Value).ToList();
                    _logger.LogInformation($"After provider filter: {availableSlots.Count} slots");
                }

                // סינון לפי עיר אם נבחרה
                if (!string.IsNullOrEmpty(request.CityName))
                {
                    availableSlots = availableSlots.Where(s =>
                        s.Branch?.Address?.City?.Name?.Equals(request.CityName, StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
                    _logger.LogInformation($"After city filter: {availableSlots.Count} slots");
                }

                // סינון לפי זמן אם נבחר
                if (!string.IsNullOrEmpty(request.TimePeriod))
                {
                    availableSlots = FilterByTimePeriod(availableSlots, request.TimePeriod);
                    _logger.LogInformation($"After time period filter: {availableSlots.Count} slots");
                }

                // סינון לפי תאריך אם נבחר
                if (request.PreferredDate.HasValue)
                {
                    availableSlots = availableSlots.Where(s => s.SlotDate == request.PreferredDate.Value).ToList();
                    _logger.LogInformation($"After date filter: {availableSlots.Count} slots");
                }

                // מיון לפי תאריך ושעה
                var sortedSlots = availableSlots
                    .OrderBy(s => s.SlotDate)
                    .ThenBy(s => s.SlotStart)
                    .Take(50) // הגבלה ל-50 תוצאות
                    .Select(s => new
                    {
                        SlotId = s.SlotId,
                        SlotDate = s.SlotDate, // עכשיו הconverter יטפל בזה
                        SlotStart = s.SlotStart, // עכשיו הconverter יטפל בזה
                        SlotEnd = s.SlotEnd, // עכשיו הconverter יטפל בזה
                        ProviderName = s.ProviderKeyNavigation?.Name ?? "Unknown Provider",
                        BranchName = s.Branch?.BranchName ?? "Unknown Branch",
                        CityName = s.Branch?.Address?.City?.Name ?? "Unknown City",
                        Address = s.Branch?.Address != null ?
                            $"{s.Branch.Address.Street?.Name ?? "Unknown Street"} {s.Branch.Address.HouseNumber}, {s.Branch.Address.City?.Name ?? "Unknown City"}" :
                            "Unknown Address"
                    })
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = sortedSlots,
                    count = sortedSlots.Count,
                    message = sortedSlots.Count == 0 ? "No available slots found for the selected criteria" : null
                });
            }
            catch (InvalidAppointmentDataException ex)
            {
                _logger.LogWarning(ex, "Invalid appointment data in search");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ServiceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Service not found in search");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (NoAvailableSlotsException ex)
            {
                _logger.LogWarning(ex, "No available slots found");
                return Ok(new
                {
                    success = true,
                    data = new List<object>(),
                    count = 0,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching available slots");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        private List<DAL.Models.AppointmentsSlot> FilterByTimePeriod(List<DAL.Models.AppointmentsSlot> slots, string timePeriod)
        {
            return timePeriod.ToLower() switch
            {
                "morning" => slots.Where(s => s.SlotStart >= TimeOnly.Parse("06:00") && s.SlotStart < TimeOnly.Parse("12:00")).ToList(),
                "afternoon" => slots.Where(s => s.SlotStart >= TimeOnly.Parse("12:00") && s.SlotStart < TimeOnly.Parse("18:00")).ToList(),
                "evening" => slots.Where(s => s.SlotStart >= TimeOnly.Parse("18:00") && s.SlotStart < TimeOnly.Parse("22:00")).ToList(),
                _ => slots
            };
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetAvailableServices()
        {
            try
            {
                var services = await _clinicServiceManagement.GetAllServices();
                var availableServices = services
                    .Where(s => !string.Equals(s.ServiceName, "Branch Manager", StringComparison.OrdinalIgnoreCase))
                    .Select(s => new { ServiceId = s.ServiceId, ServiceName = s.ServiceName })
                    .ToList();

                return Ok(new { success = true, data = availableServices });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available services");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("providers/{serviceId}")]
        public async Task<IActionResult> GetProvidersByService(int serviceId)
        {
            try
            {
                var providers = await _serviceProviderManagement.GetAllServiceProvidersByServiceId(serviceId);
                var activeProviders = providers
                    .Where(p => p.IsActive)
                    .Select(p => new {
                        ProviderKey = p.ProviderKey,
                        ProviderName = p.Name,
                        ProviderId = p.ProviderId
                    })
                    .OrderBy(p => p.ProviderName)
                    .ToList();

                return Ok(new { success = true, data = activeProviders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting providers for service {ServiceId}", serviceId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetAvailableCities()
        {
            try
            {
                var branches = await _branchManagement.GetAllBranches();
                var cities = branches
                    .Where(b => b.Address?.City != null)
                    .Select(b => new {
                        CityId = b.Address.City.CityId,
                        CityName = b.Address.City.Name
                    })
                    .Distinct()
                    .OrderBy(c => c.CityName)
                    .ToList();

                return Ok(new { success = true, data = cities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available cities");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("time-periods")]
        public IActionResult GetTimePeriods()
        {
            try
            {
                var timePeriods = new[]
                {
                    new { Value = "morning", Label = "Morning (06:00-12:00)", StartTime = "06:00", EndTime = "12:00" },
                    new { Value = "afternoon", Label = "Afternoon (12:00-18:00)", StartTime = "12:00", EndTime = "18:00" },
                    new { Value = "evening", Label = "Evening (18:00-22:00)", StartTime = "18:00", EndTime = "22:00" }
                };

                return Ok(new { success = true, data = timePeriods });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time periods");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("book/{slotId}")]
        public async Task<IActionResult> BookAppointment(int slotId, [FromBody] BookAppointmentRequest request)
        {
            try
            {
                await _appointmentService.BookAppointmentAsync(slotId, request.PatientKey.ToString());
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

        // שאר המתודות נשארות כפי שהן...
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

        public class BookAppointmentRequest
        {
            public int PatientKey { get; set; }
        }

        public class SearchSlotsRequest
        {
            public int ServiceId { get; set; }
            public int? ProviderKey { get; set; }
            public string? CityName { get; set; }
            public string? TimePeriod { get; set; } // morning, afternoon, evening
            public DateOnly? PreferredDate { get; set; }
        }
    }
}