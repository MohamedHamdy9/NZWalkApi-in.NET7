using Microsoft.AspNetCore.Http;
using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger , RequestDelegate requestDelegate)
        {
            _logger = logger;
            _next = requestDelegate;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try 
            {
                await _next(context);
            }
            catch (Exception e) 
            {
                var errorId = Guid.NewGuid();

                // Log This Exception
                _logger.LogError(e, $"{errorId} : {e.Message}");

                // Return A Custom Exrror Response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong! We are looking into resolving this."
                };

                await context.Response.WriteAsJsonAsync(error);


            }

        }
    }
}
