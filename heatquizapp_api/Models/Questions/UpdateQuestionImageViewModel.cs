namespace heatquizapp_api.Models.Questions
{
    public class UpdateQuestionImageViewModel
    {
        public int QuestionId { get; set; }
        public IFormFile Picture { get; set; }
    }
}
