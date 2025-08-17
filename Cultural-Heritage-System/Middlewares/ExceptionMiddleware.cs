using Cultural_Heritage_System.Dtos.Response;
using Newtonsoft.Json;
using System.Net;

namespace Cultural_Heritage_System.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedResponse(context);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleForbiddenResponse(context);
                }
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "AppException occurred.");
                await HandleAppException(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleUnauthorizedResponse(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleGenericException(context, ex);
            }
        }

        private Task HandleAppException(HttpContext context, AppException exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = exception.ErrorCode.Code;

            var errorResponse = new ErrorResponse
            {
                Code = response.StatusCode,
                Message = exception.Message,
                Url = context.Request.Path,
                Timestamp = DateTime.Now
            };

            return response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private Task HandleUnauthorizedResponse(HttpContext context)
        {
            var response = context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;

                var errorResponse = new ErrorResponse
                {
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = "Unauthorized - Access is denied due to invalid credentials.",
                    Url = context.Request.Path,
                    Timestamp = DateTime.Now
                };

                return response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            return Task.CompletedTask;
        }

        private Task HandleForbiddenResponse(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Code = (int)HttpStatusCode.Forbidden,
                Message = "Forbidden - You do not have permission to access this resource.",
                Url = context.Request.Path,
                Timestamp = DateTime.Now
            };

            return response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private Task HandleGenericException(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var errorResponse = new ErrorResponse
            {
                Code = response.StatusCode,
                Message = exception.Message,
                Url = context.Request.Path,
                Timestamp = DateTime.Now
            };

            return response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}