using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace heatquizapp_api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(
            RequestDelegate next
           )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //Logging
                logger.LogError
                    (ex, $"Web API exception @machine {Environment.MachineName} @traceId {Activity.Current?.Id}");

                //Return an error message
                await Results.BadRequest("Server error - please try again later").ExecuteAsync(context);
            }
        }
    }
}
