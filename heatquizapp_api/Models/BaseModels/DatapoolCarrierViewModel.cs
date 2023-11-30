using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.BaseModels
{
    public class DatapoolCarrierViewModel
    {
        [Required]
        public int DatapoolId { get; set; } 
    }
}
