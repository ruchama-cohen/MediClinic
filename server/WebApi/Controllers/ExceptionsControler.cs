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
                _logger.LogError(exceptionDetails.Error, "שגיאה במערכת התורים: {Message}", exceptionDetails.Error.Message);
            }

            // טיפול בשגיאות תורים
            if (exceptionDetails?.Error is InvalidAppointmentDataException invalidAppointmentData)
            {
                _logger.LogWarning("נתוני תור לא תקינים: {Message}", invalidAppointmentData.Message);
                return BadRequest(CreateProblemDetails(
                    title: "נתוני תור לא תקינים",
                    detail: invalidAppointmentData.Message,
                    statusCode: invalidAppointmentData.StatusCode,
                    errorCode: "INVALID_APPOINTMENT_DATA"
                ));
            }

            if (exceptionDetails?.Error is AppointmentNotFoundException appointmentNotFound)
            {
                _logger.LogWarning("תור לא נמצא: {Message}", appointmentNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "תור לא נמצא",
                    detail: appointmentNotFound.Message,
                    statusCode: appointmentNotFound.StatusCode,
                    errorCode: "APPOINTMENT_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is SlotAlreadyBookedException slotBooked)
            {
                _logger.LogWarning("תור כבר תפוס: {Message}", slotBooked.Message);
                return Conflict(CreateProblemDetails(
                    title: "התור כבר תפוס",
                    detail: slotBooked.Message,
                    statusCode: slotBooked.StatusCode,
                    errorCode: "SLOT_ALREADY_BOOKED"
                ));
            }

            if (exceptionDetails?.Error is TimeConflictException timeConflict)
            {
                _logger.LogWarning("חפיפה בזמנים: {Message}", timeConflict.Message);
                return Conflict(CreateProblemDetails(
                    title: "חפיפה בזמני תורים",
                    detail: timeConflict.Message,
                    statusCode: timeConflict.StatusCode,
                    errorCode: "TIME_CONFLICT"
                ));
            }

            if (exceptionDetails?.Error is PastAppointmentException pastAppointment)
            {
                _logger.LogWarning("תור בעבר: {Message}", pastAppointment.Message);
                return BadRequest(CreateProblemDetails(
                    title: "תור בזמן עבר",
                    detail: pastAppointment.Message,
                    statusCode: pastAppointment.StatusCode,
                    errorCode: "PAST_APPOINTMENT"
                ));
            }

            if (exceptionDetails?.Error is DoctorNotFoundException doctorNotFound)
            {
                _logger.LogWarning("רופא לא נמצא: {Message}", doctorNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "רופא לא נמצא",
                    detail: doctorNotFound.Message,
                    statusCode: doctorNotFound.StatusCode,
                    errorCode: "DOCTOR_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is DoctorNotActiveException doctorNotActive)
            {
                _logger.LogWarning("רופא לא פעיל: {Message}", doctorNotActive.Message);
                return BadRequest(CreateProblemDetails(
                    title: "רופא לא פעיל",
                    detail: doctorNotActive.Message,
                    statusCode: doctorNotActive.StatusCode,
                    errorCode: "DOCTOR_NOT_ACTIVE"
                ));
            }

            if (exceptionDetails?.Error is PatientNotFoundException patientNotFound)
            {
                _logger.LogWarning("מטופל לא נמצא: {Message}", patientNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "מטופל לא נמצא",
                    detail: patientNotFound.Message,
                    statusCode: patientNotFound.StatusCode,
                    errorCode: "PATIENT_NOT_FOUND"
                ));
            }

            if (exceptionDetails?.Error is DatabaseException dbException)
            {
                _logger.LogError(dbException, "שגיאת מסד נתונים: {Message}", dbException.Message);
                return StatusCode(500, CreateProblemDetails(
                    title: "שגיאת מסד נתונים",
                    detail: "אירעה שגיאה במסד הנתונים, אנא נסה שוב מאוחר יותר",
                    statusCode: 500,
                    errorCode: "DATABASE_ERROR"
                ));
            }

            // שגיאה כללית
            _logger.LogError("שגיאה לא מזוהה: {Message}", exceptionDetails?.Error?.Message ?? "Unknown error");
            return StatusCode(500, CreateProblemDetails(
                title: "שגיאה במערכת",
                detail: "שגיאה לא צפויה במערכת, אנא רענן את הדף ונסה שוב",
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