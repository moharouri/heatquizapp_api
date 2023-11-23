using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.BaseModels
{
    public class AddEditDataPoolViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string NickName { get; set; }

        public bool IsHidden { get; set; }
    }
}
