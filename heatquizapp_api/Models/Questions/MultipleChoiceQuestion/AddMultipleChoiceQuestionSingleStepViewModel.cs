namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class AddMultipleChoiceQuestionSingleStepViewModel : AddQuestionSingleStepViewModel
    {
        public int? DefaultImageId { get; set; }
        public string AnswerForLatex { get; set; }

        public List<IFormFile> MultipleChoiceImages { get; set; } = new List<IFormFile>();

        public string AnswersString { get; set; }
    }
}
