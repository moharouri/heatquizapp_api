using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace heatquizapp_api.Middleware
{
    public class DatapoolAccessibilityMiddleware
    {
        private readonly RequestDelegate _next;

        public DatapoolAccessibilityMiddleware
            (
                RequestDelegate next,
                ApplicationDbContext applicationDbContext
            )
        {
            _next = next;
        }

      
        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ILogger<DatapoolAccessibilityMiddleware> logger, ApplicationDbContext applicationDbContext)
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
                        var request = context.Request;

                        request.EnableBuffering();
                        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

                        await request.Body.ReadAsync(buffer, 0, buffer.Length);

                        var requestContent = Encoding.UTF8.GetString(buffer);

                        if(requestContent is null)
                        {
                            await handleDatapoolNotAuthorized(context, "Empty body");
                            return;
                        }

                        DatapoolCarrierViewModel requestBody = null;

                        try
                        {
                            requestBody = JsonConvert.DeserializeObject<DatapoolCarrierViewModel>(requestContent);
                        }
                        catch
                        {

                        }

                        //rewinding the stream to 0
                        request.Body.Position = 0;                      

                        if(!(requestBody is null))
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
                    var datapool = await applicationDbContext.DataPools
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
