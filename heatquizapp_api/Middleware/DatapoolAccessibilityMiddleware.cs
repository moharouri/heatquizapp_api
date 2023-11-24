using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Http;
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

      
        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ILogger<DatapoolAccessibilityMiddleware> logger)
        {
            
            //Check if the request is sent to datapool aware controller
            if (context.Request.Path.StartsWithSegments("/apidpaware"))
            {
                //Check if the request sent by a registered user or a player
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    //Return not authorized to this datapool
                    await handleDatapoolNotAuthorized(context, "Error finding user");
                    return;
                }
                else
                {
                    //Admin user does not need datapool access filtering
                    var userRoles = await userManager.GetRolesAsync(user);
                    var isUserAdmin = userRoles.Any(r => r.ToLower() == "admin".ToLower());

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
                            }
                            catch
                            {
                                await handleDatapoolNotAuthorized(context, "Error reading datapool");
                                return;
                            }
                        }
                        else
                        {
                            await handleDatapoolNotAuthorized(context, "Error reading datapool");
                            return;
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
                    var datapool = await _applicationDbContext.DataPools
                        .Include(p => p.PoolAccesses)
                        .FirstOrDefaultAsync(dp => dp.Id == datapoolId);

                    if (datapool == null)
                    {
                        //Return datapool does not exsit
                        await handleDatapoolNotAuthorized(context, "Datapool does not exist");
                        return;
                    }

                    //Check if user has datapool access
                    var hasAccess = datapool.PoolAccesses.Any(a => a.UserId == user.Id);

                    //Handle  -- admin passes the filtering
                    if (isUserAdmin || hasAccess)
                    {
                        await _next(context);
                        return;
                    }
                    else
                    {
                        //Return not authorized to this datapool
                        await handleDatapoolNotAuthorized(context, $"User has no access to this datapool {datapool.NickName}");
                        return;
                    }
                }
            }
            else
            {
                await _next(context);
                return;
            }
        }

        private async Task handleDatapoolNotAuthorized(HttpContext context ,string message)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(message);
            return;
        }

    }
}
