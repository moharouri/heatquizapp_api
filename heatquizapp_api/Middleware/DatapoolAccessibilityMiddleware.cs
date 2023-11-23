using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Xml;

namespace heatquizapp_api.Middleware
{
    public class DatapoolAccessibilityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationDbContext _applicationDbContext;

        public DatapoolAccessibilityMiddleware
            (
                RequestDelegate next,
                ApplicationDbContext applicationDbContext
            )
        {
            _next = next;
            _applicationDbContext = applicationDbContext;
        }

      
        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager)
        {
            await _next(context);

            return;

            //Check if the request is sent to datapool aware controller
            if (context.Request.Path.StartsWithSegments("/apidpaware"))
            {
                //Check if the request sent by a registered user or a player
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {

                    //Return not authorized to this datapool
                    await _next(context);
                }
                else
                {
                    //Get datapool Id from request
                    int datapoolId = 0;

                    if (context.Request.HasJsonContentType())
                    {
                        var requestBody = JsonSerializer.Deserialize<dynamic>(context.Request.Body);

                        if(requestBody != null)
                        {
                            try
                            {
                                datapoolId = requestBody.DatapoolId;

                                Console.WriteLine("datapoolId = " + datapoolId);
                            }
                            catch
                            {
                                Console.WriteLine("Exception");
                            }
                        }
                        else
                        {
                        }

                    }
                    else if(context.Request.HasFormContentType)
                    {
                        //Read form data
                        var form = await context.Request.ReadFormAsync();

                        //Read datapool Id
                        string? dpIdString = form?["DatapoolId"];
                        int.TryParse(dpIdString, out datapoolId);
                    }

                    //Check if datapool exists
                    var datapoolExists = await _applicationDbContext.DataPools.AnyAsync(dp => dp.Id == datapoolId);

                    if (!datapoolExists)
                    {
                        //Return not authorized to this datapool

                    }

                    //Check if user has datapool access
                    var hasAccess = true;

                    //Handle 
                    if (hasAccess)
                    {
                        await _next(context);
                    }
                    else
                    {
                        //Return not authorized to this datapool

                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        private void CreateUserNotDatapoolAuthorizedResponse(HttpContext context)
        {

        }

    }
}
