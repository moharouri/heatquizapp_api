namespace heatquizapp_api.Models.QuestionInformation
{
    public class UpdateExplanationViewModel
    {
        public int Id { get; set; }

        public string? Code { get; set; }
        public string? Latex { get; set; }
        public IFormFile? PDF { get; set; }

        public int? DataPoolId { get; set; }
    }
}
