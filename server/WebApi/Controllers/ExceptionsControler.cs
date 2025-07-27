using BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class ErrorsHandlerController : ControllerBase
    {
        private readonly ILogger<ErrorsHandlerController> _logger;

        public ErrorsHandlerController(ILogger<ErrorsHandlerController> logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionDetails != null)
            {
                _logger.LogError(exceptionDetails.Error, "Queue system error: {Message}", exceptionDetails.Error.Message);
            }

            if (exceptionDetails?.Error is InvalidAppointmentDataException invalidAppointmentData)
            {
                _logger.LogWarning("Invalid appointment data: {Message}", invalidAppointmentData.Message);
                return BadRequest(CreateProblemDetails(
                    title: "Invalid appointment data",
                    detail: invalidAppointmentData.Message,
                    statusCode: invalidAppointmentData.StatusCode,
                    errorCode: "INVALID_APPOINTMENT_DATA"
                ));
            }

            if (exceptionDetails?.Error is AppointmentNotFoundException appointmentNotFound)
            {
                _logger.LogWarning("Appointment not found: {Message}", appointmentNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "Appointment not found",
                    detail: appointmentNotFound.Message,
                    statusCode: appointmentNotFound.StatusCode,
                    errorCode: "APPOINTMENT_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is SlotAlreadyBookedException slotBooked)
            {
                _logger.LogWarning("Slot already booked: {Message}", slotBooked.Message);
                return Conflict(CreateProblemDetails(
                    title: "Slot already booked",
                    detail: slotBooked.Message,
                    statusCode: slotBooked.StatusCode,
                    errorCode: "SLOT_ALREADY_BOOKED"
                ));
            }

            if (exceptionDetails?.Error is TimeConflictException timeConflict)
            {
                _logger.LogWarning("Time conflict: {Message}", timeConflict.Message);
                return Conflict(CreateProblemDetails(
                    title: "Appointment time conflict",
                    detail: timeConflict.Message,
                    statusCode: timeConflict.StatusCode,
                    errorCode: "TIME_CONFLICT"
                ));
            }

            if (exceptionDetails?.Error is PastAppointmentException pastAppointment)
            {
                _logger.LogWarning("Past appointment: {Message}", pastAppointment.Message);
                return BadRequest(CreateProblemDetails(
                    title: "Past appointment",
                    detail: pastAppointment.Message,
                    statusCode: pastAppointment.StatusCode,
                    errorCode: "PAST_APPOINTMENT"
                ));
            }

            if (exceptionDetails?.Error is DoctorNotFoundException doctorNotFound)
            {
                _logger.LogWarning("Doctor not found: {Message}", doctorNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "Doctor not found",
                    detail: doctorNotFound.Message,
                    statusCode: doctorNotFound.StatusCode,
                    errorCode: "DOCTOR_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is DoctorNotActiveException doctorNotActive)
            {
                _logger.LogWarning("Doctor not active: {Message}", doctorNotActive.Message);
                return BadRequest(CreateProblemDetails(
                    title: "Doctor not active",
                    detail: doctorNotActive.Message,
                    statusCode: doctorNotActive.StatusCode,
                    errorCode: "DOCTOR_NOT_ACTIVE"
                ));
            }

            if (exceptionDetails?.Error is PatientNotFoundException patientNotFound)
            {
                _logger.LogWarning("Patient not found: {Message}", patientNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "Patient not found",
                    detail: patientNotFound.Message,
                    statusCode: patientNotFound.StatusCode,
                    errorCode: "PATIENT_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is DatabaseException dbException)
            {
                _logger.LogError(dbException, "Database error: {Message}", dbException.Message);
                return StatusCode(500, CreateProblemDetails(
                    title: "Database error",
                    detail: "A database error occurred. Please try again later.",
                    statusCode: 500,
                    errorCode: "DATABASE_ERROR"
                ));
            }

            _logger.LogError("Unhandled error: {Message}", exceptionDetails?.Error?.Message ?? "Unknown error");
            return StatusCode(500, CreateProblemDetails(
                title: "System error",
                detail: "An unexpected system error occurred. Please refresh the page and try again.",
                statusCode: 500,
                errorCode: "UNKNOWN_ERROR"
            ));
        }

        private object CreateProblemDetails(string title, string detail, int statusCode, string errorCode)
        {
            return new
            {
                success = false,
                title = title,
                detail = detail,
                status = statusCode,
                errorCode = errorCode,
                timestamp = DateTime.UtcNow,
                traceId = HttpContext.TraceIdentifier
            };
        }
    }
}
