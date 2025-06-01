using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.API;
using BLL.Exceptions;
using WebAPI.Services;
using DAL.API;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IBL _bl;
        private readonly IJwtService _jwtService;
        private readonly IPatientsManagement _patientsManagement;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IBL bl, IJwtService jwtService, IPatientsManagement patientsManagement, ILogger<AuthController> logger)
        {
            _bl = bl;
            _jwtService = jwtService;
            _patientsManagement = patientsManagement;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Patient = null
                    });
                }

                var patientKey = await _bl.AuthService.Login(request.UserId, request.UserPassword);

                if (patientKey == -2)
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Patient has not completed registration in the system",
                        Patient = null
                    });
                }

                if (patientKey <= 0)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid ID or password",
                        Patient = null
                    });
                }

                var patient = await _patientsManagement.GetPatientByIdString(patientKey);
                if (patient == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "User not found",
                        Patient = null
                    });
                }

                var token = _jwtService.GenerateToken(patient.PatientKey, patient.PatientId, patient.PatientName);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Message = "Login successful",
                    Patient = new PatientInfo
                    {
                        PatientKey = patient.PatientKey,
                        PatientId = patient.PatientId,
                        PatientName = patient.PatientName,
                        Email = patient.Email ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {UserId}", request.UserId);
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "System error occurred",
                    Patient = null
                });
            }
        }

        [HttpPost("test-password")]
        public async Task<IActionResult> TestPassword([FromBody] TestPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new { success = false, message = "Invalid input data", errors });
                }

                var result = await _bl.AuthService.SetPasswordForTesting(request.PatientId, request.NewPassword);

                if (result)
                {
                    return Ok(new { success = true, message = $"Password set successfully for patient {request.PatientId}" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Patient not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting test password for patient: {PatientId}", request.PatientId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        public class TestPasswordRequest
        {
            public string PatientId { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}