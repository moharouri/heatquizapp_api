using System.ComponentModel.DataAnnotations;

namespace HeatQuizAPI.Models.Login
{
    public class LoginForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int DataPoolId { get; set; }
    }
}
