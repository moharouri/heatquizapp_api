using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace heatquizapp_api.Utilities
{
    public static class Utilities
    {
        //Get user sending an HTTP request to the server
        public static async Task<User> getCurrentUser(IHttpContextAccessor context, UserManager<User> userManager)
        {
            var userId = context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await userManager.FindByIdAsync(userId);

            return user;
        }

        //Validate the extension of a picture file
        public static async Task<bool> validateImageExtension(IFormFile Picture)
        {
            var validExtenstions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtensionIsValid = validExtenstions.Any(ve => Picture.FileName.EndsWith(ve));

            if (!fileExtensionIsValid)
                return false;

            return true;
        }

        //Save a file to local storage
        public async static Task<string> SaveFile(IFormFile File)
        {
            //Get random file name and save it to the global files directory wwwroot/Files
            var fileName = Path.GetRandomFileName();
            var FileExtension = Path.GetExtension(File.FileName);

            var path = Path.Combine("wwwroot/Files", fileName + FileExtension);

            //Save File
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            var URL = new string(path.SkipWhile(s => s != 't').Skip(2).ToArray());

            return URL;
        }
    }
}
