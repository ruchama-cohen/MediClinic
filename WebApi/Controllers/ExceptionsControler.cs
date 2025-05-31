using MediClinic.Exceptions;
.
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

            // Appointment System Exceptions
            if (exceptionDetails?.Error is AppointmentNotFoundException appointmentNotFound)
            {
                _logger.LogWarning("תור לא נמצא: {Message}", appointmentNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "תור לא נמצא",
                    detail: appointmentNotFound.Message,
                    statusCode: appointmentNotFound.StatusCode,
                    errorCode: appointmentNotFound.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is SlotAlreadyBookedException slotBooked)
            {
                _logger.LogWarning("תור כבר תפוס: {Message}", slotBooked.Message);
                return Conflict(CreateProblemDetails(
                    title: "התור כבר תפוס",
                    detail: slotBooked.Message,
                    statusCode: slotBooked.StatusCode,
                    errorCode: slotBooked.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is PatientNotFoundException patientNotFound)
            {
                _logger.LogWarning("מטופל לא נמצא: {Message}", patientNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "מטופל לא נמצא",
                    detail: patientNotFound.Message,
                    statusCode: patientNotFound.StatusCode,
                    errorCode: patientNotFound.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is DoctorNotFoundException doctorNotFound)
            {
                _logger.LogWarning("רופא לא נמצא: {Message}", doctorNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "רופא לא נמצא",
                    detail: doctorNotFound.Message,
                    statusCode: doctorNotFound.StatusCode,
                    errorCode: doctorNotFound.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is DoctorNotActiveException doctorNotActive)
            {
                _logger.LogWarning("רופא לא פעיל: {Message}", doctorNotActive.Message);
                return BadRequest(CreateProblemDetails(
                    title: "רופא לא פעיל",
                    detail: doctorNotActive.Message,
                    statusCode: doctorNotActive.StatusCode,
                    errorCode: doctorNotActive.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is TimeConflictException timeConflict)
            {
                _logger.LogWarning("חפיפה בזמנים: {Message}", timeConflict.Message);
                return Conflict(CreateProblemDetails(
                    title: "חפיפה בזמנים",
                    detail: timeConflict.Message,
                    statusCode: timeConflict.StatusCode,
                    errorCode: timeConflict.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is PastAppointmentException pastAppointment)
            {
                _logger.LogWarning("תור בעבר: {Message}", pastAppointment.Message);
                return BadRequest(CreateProblemDetails(
                    title: "תור בזמן שעבר",
                    detail: pastAppointment.Message,
                    statusCode: pastAppointment.StatusCode,
                    errorCode: pastAppointment.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is TooEarlyBookingException tooEarly)
            {
                _logger.LogWarning("קביעת תור מוקדם מדי: {Message}", tooEarly.Message);
                return BadRequest(CreateProblemDetails(
                    title: "קביעת תור מוקדם מדי",
                    detail: tooEarly.Message,
                    statusCode: tooEarly.StatusCode,
                    errorCode: tooEarly.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is DailyLimitExceededException dailyLimit)
            {
                _logger.LogWarning("חריגה ממגבלת תורים יומית: {Message}", dailyLimit.Message);
                return StatusCode(429, CreateProblemDetails(
                    title: "חריגה ממגבלת תורים יומית",
                    detail: dailyLimit.Message,
                    statusCode: dailyLimit.StatusCode,
                    errorCode: dailyLimit.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is BranchNotFoundException branchNotFound)
            {
                _logger.LogWarning("סניף לא נמצא: {Message}", branchNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "סניף לא נמצא",
                    detail: branchNotFound.Message,
                    statusCode: branchNotFound.StatusCode,
                    errorCode: branchNotFound.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is ServiceNotFoundException serviceNotFound)
            {
                _logger.LogWarning("שירות לא נמצא: {Message}", serviceNotFound.Message);
                return NotFound(CreateProblemDetails(
                    title: "שירות לא נמצא",
                    detail: serviceNotFound.Message,
                    statusCode: serviceNotFound.StatusCode,
                    errorCode: serviceNotFound.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is InvalidAppointmentDataException invalidData)
            {
                _logger.LogWarning("נתונים לא תקינים: {Message}", invalidData.Message);
                return BadRequest(CreateProblemDetails(
                    title: "נתונים לא תקינים",
                    detail: invalidData.Message,
                    statusCode: invalidData.StatusCode,
                    errorCode: invalidData.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is NoAvailableSlotsException noSlots)
            {
                _logger.LogInformation("אין תורים פנויים: {Message}", noSlots.Message);
                return NotFound(CreateProblemDetails(
                    title: "אין תורים פנויים",
                    detail: noSlots.Message,
                    statusCode: noSlots.StatusCode,
                    errorCode: noSlots.ErrorCode
                ));
            }

            if (exceptionDetails?.Error is UnauthorizedAppointmentException unauthorized)
            {
                _logger.LogWarning("אין הרשאה: {Message}", unauthorized.Message);
                return StatusCode(403, CreateProblemDetails(
                    title: "אין הרשאה",
                    detail: unauthorized.Message,
                    statusCode: unauthorized.StatusCode,
                    errorCode: unauthorized.ErrorCode
                ));
            }

        // במקום:
        // catch (Exception ex) when (!(ex is AppointmentBaseException))

        // עכשיו פשוט:
        catch (Exception ex) when(!(ex is AppointmentNotFoundException ||
                                    ex is SlotAlreadyBookedException ||
                                    ex is PatientNotFoundException ||
                                    ex is DoctorNotFoundException))
            if (exceptionDetails?.Error is ArgumentException argumentEx)
            {
                _logger.LogWarning("פרמטרים לא תקינים: {Message}", argumentEx.Message);
                return BadRequest(CreateProblemDetails(
                    title: "פרמטרים לא תקינים",
                    detail: "אחד או יותר מהפרמטרים שנשלחו אינם תקינים",
                    statusCode: 400,
                    errorCode: "INVALID_ARGUMENTS"
                ));
            }

            if (exceptionDetails?.Error is ArgumentNullException)
            {
                _logger.LogWarning("פרמטר חסר");
                return BadRequest(CreateProblemDetails(
                    title: "פרמטר חסר",
                    detail: "אחד מהפרמטרים הנדרשים חסר",
                    statusCode: 400,
                    errorCode: "MISSING_PARAMETER"
                ));
            }

            if (exceptionDetails?.Error is InvalidOperationException invalidOperation)
            {
                _logger.LogWarning("פעולה לא חוקית: {Message}", invalidOperation.Message);
                return BadRequest(CreateProblemDetails(
                    title: "פעולה לא חוקית",
                    detail: invalidOperation.Message,
                    statusCode: 400,
                    errorCode: "INVALID_OPERATION"
                ));
            }

            if (exceptionDetails?.Error is NullReferenceException)
            {
                _logger.LogError("שגיאת מערכת פנימית - NullReference");
                return StatusCode(500, CreateProblemDetails(
                    title: "שגיאה פנימית",
                    detail: "אנא פנה לתמיכה טכנית: 03-1234567",
                    statusCode: 500,
                    errorCode: "INTERNAL_ERROR"
                ));
            }

            // Database Exceptions
            if (exceptionDetails?.Error.GetType().Name.Contains("DbUpdate") ||
                exceptionDetails?.Error.GetType().Name.Contains("Sql"))
            {
                _logger.LogError("שגיאת מסד נתונים: {Message}", exceptionDetails.Error.Message);
                return StatusCode(500, CreateProblemDetails(
                    title: "שגיאת מסד נתונים",
                    detail: "בעיה זמנית במערכת, אנא נסה שוב מאוחר יותר",
                    statusCode: 500,
                    errorCode: "DATABASE_ERROR"
                ));
            }

            // Default Error
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