namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class MultipleChoiceQuestionViewModel : QuestionBaseViewModel
    {
        public List<MultipleChoiceQuestionChoiceViewModel> Choices { get; set; } = new List<MultipleChoiceQuestionChoiceViewModel>();

    }
}
