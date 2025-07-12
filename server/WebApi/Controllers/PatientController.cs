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
        private readonly ILogger<PatientController> _logger;

        public PatientController(IBL bl, ILogger<PatientController> logger)
        {
            _patientService = bl.PatientService;
            _logger = logger;
        }

        /// <summary>
        /// קבלת פרטי משתמש עם כתובת לפי PatientId
        /// </summary>
        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetPatient(string patientId)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdString(patientId);

                var response = new PatientResponse
                {
                    PatientKey = patient.PatientKey,
                    PatientId = patient.PatientId,
                    PatientName = patient.PatientName,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    Address = patient.address != null ? new AddressResponse
                    {
                        CityName = patient.address.City?.Name ?? "",
                        StreetName = patient.address.Street?.Name ?? "",
                        HouseNumber = patient.address.HouseNumber,
                        PostalCode = patient.address.PostalCode
                    } : null
                };

                return Ok(response);
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient {PatientId}", patientId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// קבלת פרטי משתמש עם כתובת לפי PatientKey
        /// </summary>
        [HttpGet("by-key/{patientKey}")]
        public async Task<IActionResult> GetPatientByKey(int patientKey)
        {
            try
            {
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
                        CityName = patient.address.City?.Name ?? "",
                        StreetName = patient.address.Street?.Name ?? "",
                        HouseNumber = patient.address.HouseNumber,
                        PostalCode = patient.address.PostalCode
                    } : null
                };

                return Ok(response);
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by key {PatientKey}", patientKey);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// עדכון פרטי משתמש עם כתובת
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new { success = false, message = "Invalid data", errors = errors });
                }

                bool success;

                if (request.Address != null)
                {
                    success = await _patientService.UpdatePatientWithAddress(
                        request.PatientKey,
                        request.PatientName,
                        request.Email,
                        request.Phone,
                        request.Address.CityName,
                        request.Address.StreetName,
                        request.Address.HouseNumber,
                        request.Address.PostalCode);
                }
                else
                {
                    success = await _patientService.UpdatePatientDetailsByKey(
                        request.PatientKey,
                        request.PatientName,
                        request.Email,
                        request.Phone);
                }

                return Ok(new { success = true, message = "Patient updated successfully" });
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
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
        /// שינוי סיסמה
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new { success = false, message = "Invalid data", errors = errors });
                }

                await _patientService.ChangePasswordByKey(
                    request.PatientKey,
                    request.CurrentPassword,
                    request.NewPassword);

                return Ok(new { success = true, message = "Password changed successfully" });
            }
            catch (PatientNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidAppointmentDataException ex)
            {
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
    }
}