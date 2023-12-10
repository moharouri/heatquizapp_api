namespace heatquizapp_api.Models.DefaultQuestionImages
{
    public class AddDefaultQuestionImageViewModel
    {
        public string Code { get; set; } 
        public IFormFile Picture { get; set; }
        public int DataPoolId { get; set; }
    }
}
