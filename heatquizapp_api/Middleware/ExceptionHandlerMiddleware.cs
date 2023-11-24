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

                //Return the result
                await Results
                 .Problem(
                    title: "Server error - please try again later",

                    statusCode: StatusCodes.Status500InternalServerError,

                    extensions: new Dictionary<string, object?>()
                    {
                        {"traceId", Activity.Current?.Id}
                    }
                 )
                 .ExecuteAsync(context);
            }
        }
    }
}
