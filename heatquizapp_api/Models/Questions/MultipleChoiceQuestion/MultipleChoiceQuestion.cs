namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class MultipleChoiceQuestion : QuestionBase
    {
        public List<MultipleChoiceQuestionChoice> Choices { get; set; } = new List<MultipleChoiceQuestionChoice>();
    }
}
