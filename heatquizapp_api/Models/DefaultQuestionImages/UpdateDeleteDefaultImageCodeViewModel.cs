namespace heatquizapp_api.Models.DefaultQuestionImages
{
    public class UpdateDeleteDefaultImageCodeViewModel
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public IFormFile? Picture { get; set; }
    } 
}
