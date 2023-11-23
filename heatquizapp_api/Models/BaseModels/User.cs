using Microsoft.AspNetCore.Identity;

namespace HeatQuizAPI.Models.BaseModels
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public DateTime RegisteredOn { get; set; }

        public bool Active { get; set; }

        public string? ProfilePicture { get; set; }
    }
}
