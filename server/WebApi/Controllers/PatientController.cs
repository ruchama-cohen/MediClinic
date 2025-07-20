using Microsoft.AspNetCore.Mvc;
using BLL.API;
using BLL.Exceptions;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ICityStreetService _cityStreetService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IBL bl, ILogger<PatientController> logger)
        {
            _patientService = bl.PatientService;
            _cityStreetService = bl.CityStreetService;
            _logger = logger;
        }

        /// <summary>
        /// קבלת פרטי מטופל לפי PatientKey
        /// </summary>
        [HttpGet("by-key/{patientKey}")]
        public async Task<IActionResult> GetPatientByKey(int patientKey)
        {
            try
            {
                _logger.LogInformation("Getting patient details for PatientKey: {PatientKey}", patientKey);

                var patient = await _patientService.GetPatientByKey(patientKey);

                var response = new PatientResponse
                {
                    PatientKey = patient.PatientKey,
                    PatientId = patient.PatientId,
                    PatientName = patient.PatientName,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    Address = patient.address != null ? new AddressResponse
                    {
                        CityId = patient.address.CityId,
                        CityName = patient.address.City?.Name ?? "",
                        StreetId = patient.address.StreetId,
                        StreetName = patient.address.Street?.Name ?? "",
                        HouseNumber = patient.address.HouseNumber,
                        PostalCode = patient.address.PostalCode
                    } : null
                };

                _logger.LogInformation("Successfully retrieved patient details for PatientKey: {PatientKey}", patientKey);
                return Ok(response);
            }
            catch (PatientNotFoundException ex)
            {
                _logger.LogWarning("Patient not found: {PatientKey}", patientKey);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
                _logger.LogWarning("Invalid patient key: {PatientKey}", patientKey);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by key {PatientKey}", patientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// עדכון חלקי של פרטי מטופל - כל השדות אופציונליים
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Invalid model state for patient update: {PatientKey}", request.PatientKey);
                    return BadRequest(new { success = false, message = "Invalid data", errors = errors });
                }

                _logger.LogInformation("Updating patient: {PatientKey}", request.PatientKey);

                // בדיקה אם סופקו נתוני כתובת חלקיים
                if (request.Address != null)
                {
                    bool hasAnyAddressField = request.Address.CityId.HasValue ||
                                            request.Address.StreetId.HasValue ||
                                            request.Address.HouseNumber.HasValue ||
                                            !string.IsNullOrWhiteSpace(request.Address.PostalCode);

                    bool hasAllAddressFields = request.Address.CityId.HasValue &&
                                             request.Address.StreetId.HasValue &&
                                             request.Address.HouseNumber.HasValue &&
                                             !string.IsNullOrWhiteSpace(request.Address.PostalCode);

                    if (hasAnyAddressField && !hasAllAddressFields)
                    {
                        _logger.LogWarning("Partial address data provided for patient: {PatientKey}", request.PatientKey);
                        return BadRequest(new
                        {
                            success = false,
                            message = "To update address, all address fields must be provided (city, street, house number, postal code)"
                        });
                    }
                }

                await _patientService.UpdatePatientPartial(
                    request.PatientKey,
                    request.PatientName,
                    request.Email,
                    request.Phone,
                    request.Address?.CityId,
                    request.Address?.StreetId,
                    request.Address?.HouseNumber,
                    request.Address?.PostalCode);

                _logger.LogInformation("Successfully updated patient: {PatientKey}", request.PatientKey);
                return Ok(new { success = true, message = "Patient updated successfully" });
            }
            catch (PatientNotFoundException ex)
            {
                _logger.LogWarning("Patient not found for update: {PatientKey}", request.PatientKey);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
                _logger.LogWarning("Invalid data for patient update: {PatientKey}, Error: {Error}",
                    request.PatientKey, ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error updating patient {PatientKey}", request.PatientKey);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientKey}", request.PatientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// שינוי סיסמה של מטופל
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Invalid model state for password change: {PatientKey}", request.PatientKey);
                    return BadRequest(new { success = false, message = "Invalid data", errors = errors });
                }

                _logger.LogInformation("Changing password for patient: {PatientKey}", request.PatientKey);

                await _patientService.ChangePassword(
                    request.PatientKey,
                    request.CurrentPassword,
                    request.NewPassword);

                _logger.LogInformation("Successfully changed password for patient: {PatientKey}", request.PatientKey);
                return Ok(new { success = true, message = "Password changed successfully" });
            }
            catch (PatientNotFoundException ex)
            {
                _logger.LogWarning("Patient not found for password change: {PatientKey}", request.PatientKey);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
                _logger.LogWarning("Invalid password change data for patient: {PatientKey}, Error: {Error}",
                    request.PatientKey, ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database error changing password for patient {PatientKey}", request.PatientKey);
                return StatusCode(500, new { success = false, message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for patient {PatientKey}", request.PatientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// קבלת כל הערים הזמינות
        /// </summary>
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                _logger.LogInformation("Getting all available cities");

                var cities = await _cityStreetService.GetAllCitiesAsync();
                var response = cities.Select(c => new CityResponse
                {
                    CityId = c.CityId,
                    Name = c.Name
                }).OrderBy(c => c.Name).ToList();

                _logger.LogInformation("Successfully retrieved {Count} cities", response.Count);
                return Ok(new { success = true, data = response, count = response.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cities");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// קבלת רחובות לפי עיר
        /// </summary>
        [HttpGet("streets/{cityId}")]
        public async Task<IActionResult> GetStreetsByCity(int cityId)
        {
            try
            {
                if (cityId <= 0)
                {
                    _logger.LogWarning("Invalid city ID provided: {CityId}", cityId);
                    return BadRequest(new { success = false, message = "Valid city ID is required" });
                }

                _logger.LogInformation("Getting streets for city: {CityId}", cityId);

                var streets = await _cityStreetService.GetStreetsByCityIdAsync(cityId);
                var response = streets.Select(s => new StreetResponse
                {
                    StreetId = s.StreetId,
                    Name = s.Name,
                    CityId = s.CityId
                }).OrderBy(s => s.Name).ToList();

                _logger.LogInformation("Successfully retrieved {Count} streets for city: {CityId}",
                    response.Count, cityId);

                return Ok(new { success = true, data = response, count = response.Count });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument for streets request: {CityId}, Error: {Error}",
                    cityId, ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting streets for city {CityId}", cityId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// בדיקת מצב השרת
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                success = true,
                message = "Patient service is running",
                timestamp = DateTime.UtcNow
            });
        }
    }
}