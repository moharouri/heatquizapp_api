using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace heatquizapp_api.Utilities
{
    public static class Utilities
    {
        public static async Task<User> getCurrentUser(IHttpContextAccessor context, UserManager<User> userManager)
        {
            var userId = context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await userManager.FindByIdAsync(userId);

            return user;
        }
    }
}
