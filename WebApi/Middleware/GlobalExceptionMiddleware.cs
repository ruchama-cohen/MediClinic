using BLL.Exceptions;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Success = false,
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier,
                Path = context.Request.Path
            };

            switch (exception)
            {
                case AppointmentBaseException appointmentEx:
                    response.Status = appointmentEx.StatusCode;
                    response.Title = GetTitleForStatusCode(appointmentEx.StatusCode);
                    response.Detail = appointmentEx.Message;
                    response.ErrorCode = GetErrorCodeForException(appointmentEx);
                    break;

                case ArgumentException argEx:
                    response.Status = 400;
                    response.Title = "Invalid Request";
                    response.Detail = argEx.Message;
                    response.ErrorCode = "INVALID_ARGUMENT";
                    break;

                case UnauthorizedAccessException:
                    response.Status = 401;
                    response.Title = "Unauthorized";
                    response.Detail = "Access denied. Please check your credentials.";
                    response.ErrorCode = "UNAUTHORIZED";
                    break;

                case NotImplementedException:
                    response.Status = 501;
                    response.Title = "Not Implemented";
                    response.Detail = "This feature is not yet implemented.";
                    response.ErrorCode = "NOT_IMPLEMENTED";
                    break;

                case TimeoutException:
                    response.Status = 408;
                    response.Title = "Request Timeout";
                    response.Detail = "The request timed out. Please try again.";
                    response.ErrorCode = "TIMEOUT";
                    break;

                default:
                    response.Status = 500;
                    response.Title = "Internal Server Error";
                    response.Detail = "An unexpected error occurred. Please try again later.";
                    response.ErrorCode = "INTERNAL_ERROR";
                    _logger.LogError(exception, "Unhandled exception occurred");
                    break;
            }

            context.Response.StatusCode = response.Status;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                429 => "Too Many Requests",
                500 => "Internal Server Error",
                _ => "Error"
            };
        }

        private string GetErrorCodeForException(AppointmentBaseException exception)
        {
            return exception switch
            {
                AppointmentNotFoundException => "APPOINTMENT_NOT_FOUND",
                SlotAlreadyBookedException => "SLOT_ALREADY_BOOKED",
                PatientNotFoundException => "PATIENT_NOT_FOUND",
                DoctorNotFoundException => "DOCTOR_NOT_FOUND",
                DoctorNotActiveException => "DOCTOR_NOT_ACTIVE",
                TimeConflictException => "TIME_CONFLICT",
                PastAppointmentException => "PAST_APPOINTMENT",
                TooEarlyBookingException => "TOO_EARLY_BOOKING",
                DailyLimitExceededException => "DAILY_LIMIT_EXCEEDED",
                BranchNotFoundException => "BRANCH_NOT_FOUND",
                ServiceNotFoundException => "SERVICE_NOT_FOUND",
                InvalidAppointmentDataException => "INVALID_DATA",
                NoAvailableSlotsException => "NO_AVAILABLE_SLOTS",
                UnauthorizedAppointmentException => "UNAUTHORIZED_ACTION",
                SlotNotFoundException => "SLOT_NOT_FOUND",
                DatabaseException => "DATABASE_ERROR",
                CityNotFoundException => "CITY_NOT_FOUND",
                StreetNotFoundException => "STREET_NOT_FOUND",
                AddressValidationException => "ADDRESS_VALIDATION_ERROR",
                AddressNotFoundException => "ADDRESS_NOT_FOUND",
                PatientUpdateException => "PATIENT_UPDATE_ERROR",
                PasswordValidationException => "PASSWORD_VALIDATION_ERROR",
                InsufficientPermissionsException => "INSUFFICIENT_PERMISSIONS",
                DuplicateDataException => "DUPLICATE_DATA",
                _ => "UNKNOWN_APPOINTMENT_ERROR"
            };
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public int Status { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}