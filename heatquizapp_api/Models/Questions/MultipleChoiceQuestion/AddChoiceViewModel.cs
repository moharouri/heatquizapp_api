namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class AddChoiceViewModel
    {
        public int QuestionId { get; set; } 
        public IFormFile? Picture { get; set; }
        public string? Latex { get; set; }

        public bool Correct { get; set; }
    }
}
