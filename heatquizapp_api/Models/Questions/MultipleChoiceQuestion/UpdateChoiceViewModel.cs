namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class UpdateChoiceViewModel
    {
        public int QuestionId { get; set; } 
        public int AnswerId { get; set; }

        public IFormFile? Picture { get; set; }
        public string? Latex { get; set; }
        public bool? Correct { get; set; }
    }
}
