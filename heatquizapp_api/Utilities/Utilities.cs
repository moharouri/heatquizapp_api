using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Keyboard;
using heatquizapp_api.Models.Questions;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
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
        public static bool validateImageExtension(IFormFile Picture)
        {
            var validExtenstions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtensionIsValid = validExtenstions.Any(ve => Picture.FileName.EndsWith(ve));

            if (!fileExtensionIsValid)
                return false;

            return true;
        }

        //Validate the extension of a PDF file
        public static bool validatePDFExtension(IFormFile PDF)
        {
            var validExtenstions = new List<string>() { ".pdf"};
            var fileExtensionIsValid = validExtenstions.Any(ve => PDF.FileName.EndsWith(ve));

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

            //Save file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            var URL = new string(path.SkipWhile(s => s != 's').Skip(2).ToArray());

            return URL;
        }

        //Copy a file from a source path
        public async static Task<string> CopyFile(string sourcePath)
        {
            //Get random file name
            var FileExtension = Path.GetExtension(sourcePath);
            var fileName = Path.GetRandomFileName();

            //Add file to path
            var path = Path.Combine("wwwroot/Files", fileName + FileExtension);

            //Save file
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(path))
                {
                    await source.CopyToAsync(destination);
                }
            }

            var URL = new string(path.SkipWhile(s => s != 's').Skip(2).ToArray());

            return URL;
        }

        //Remove a file from a source path
        public static bool RemoveFile(string sourcePath)
        { 
            //Get path
            var path = Path.Combine("wwwroot/Files", sourcePath);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch {
                    return false;
                }
            }

            return true;
        }

        public static List<string> GetKeyboardReplacementChars()
        {
            List<char> array = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            array.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray());

            var charArray1 = new List<string>();

            foreach (var c in array)
            {
                charArray1.Add(new string(c, 1));
            }


            var charArray2 = new List<string>();

            foreach (var s in charArray1)
            {
                foreach (var ss in charArray1)
                {
                    charArray2.Add(s + ss);
                }
            }

            return charArray2;
        }

        public static string? GetUniqueKeyboardReplacementChar(Keyboard keyboard)
        {
            List<char> array = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            array.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray());

            var charArray1 = new List<string>();

            foreach (var c in array)
            {
                charArray1.Add(new string(c, 1));
            }


            var charArray2 = new List<string>();

            foreach (var s in charArray1)
            {
                foreach (var ss in charArray1)
                {
                    charArray2.Add(s + ss);
                }
            }

            var finalChar = charArray2
            .Where(c => 
            !keyboard.NumericKeys.Any(k => k.KeySimpleForm == c) 
            && 
            !keyboard.VariableKeyImages.Any(k => k.ReplacementCharacter == c))
            .FirstOrDefault();

            return finalChar;
        }

    }
}
