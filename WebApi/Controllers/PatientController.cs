using Microsoft.AspNetCore.Mvc;
using BLL.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IBL bl)
        {
            _patientService = bl.PatientService;
        }

        /// <summary>
        /// קבלת פרטי משתמש עם כתובת
        /// </summary>
        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetPatient(string patientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(patientId))
                    return BadRequest("Patient ID is required.");

                var patient = await _patientService.GetPatientByIdString(patientId);
                if (patient == null)
                    return NotFound("Patient not found.");

                var response = new PatientResponse
                {
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
            catch (Exception)
            {
                return StatusCode(500, "Error retrieving patient.");
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
                    return BadRequest(ModelState);

                bool success;

                if (request.Address != null)
                {
                    // עדכון עם כתובת
                    success = await _patientService.UpdatePatientWithAddress(
                        request.PatientId,
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
                    // עדכון רק פרטי קשר
                    success = await _patientService.UpdatePatientDetails(
                        request.PatientId,
                        request.PatientName,
                        request.Email,
                        request.Phone);
                }

                if (!success)
                    return NotFound("Patient not found or update failed.");

                return Ok(new
                {
                    success = true,
                    message = "Patient details updated successfully."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error updating patient.");
            }
        }

        /// <summary>
        /// עדכון פרטי קשר בלבד (ללא כתובת)
        /// </summary>
        [HttpPut("update-contact")]
        public async Task<IActionResult> UpdateContactInfo([FromBody] UpdateContactInfoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _patientService.UpdatePatientDetails(
                    request.PatientId,
                    request.PatientName,
                    request.Email,
                    request.Phone);

                if (!success)
                    return NotFound("Patient not found or update failed.");

                return Ok(new
                {
                    success = true,
                    message = "Contact information updated successfully."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error updating contact information.");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _patientService.ChangePassword(
                    request.PatientId,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!success)
                    return BadRequest("Patient not found or current password is incorrect.");

                return Ok(new
                {
                    success = true,
                    message = "Password changed successfully."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error changing password.");
            }
        }
    }
}