using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.BaseModels
{
    public class EditUserInfoViewModel
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
