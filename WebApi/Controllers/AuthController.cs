using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.API;
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

        public AuthController(IBL bl, IJwtService jwtService, IPatientsManagement patientsManagement)
        {
            _bl = bl;
            _jwtService = jwtService;
            _patientsManagement = patientsManagement;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequest request)
        {
            try
            {
                var patientKey = await _bl.AuthService.Login(request.UserId, request.UserPassword);

                if (patientKey == -2)
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "המטופל טרם השלים הרשמה למערכת."
                    });
                }

                if (patientKey <= 0)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "מספר זהות או סיסמה שגויים."
                    });
                }

                var patient = await _patientsManagement.GetPatientByIdString(patientKey);
                if (patient == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "משתמש לא נמצא."
                    });
                }


                var token = _jwtService.GenerateToken(patient.PatientKey, patient.PatientId, patient.PatientName);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Message = "התחברות הצליחה!",
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
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "שגיאה במערכת."
                });
            }
        }
        [HttpPost("test-password")]
        public async Task<IActionResult> TestPassword([FromBody] TestPasswordRequest request)
        {
            try
            {
                var result = await _bl.AuthService.SetPasswordForTesting(request.PatientId, request.NewPassword);

                if (result)
                {
                    return Ok($"Password set successfully for patient {request.PatientId}");
                }
                else
                {
                    return NotFound("Patient not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class TestPasswordRequest
        {
            public string PatientId { get; set; }
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}