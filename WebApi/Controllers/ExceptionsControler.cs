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
