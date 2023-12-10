namespace heatquizapp_api.Models.Questions
{
    public class AddQuestionSolutionViewModel
    {
        public int QuestionId { get; set; }
        public IFormFile PDF { get; set; }
    }
}
