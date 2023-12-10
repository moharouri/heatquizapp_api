namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class AddEditKeyboardQuestionAnswerViewModel
    {
        public int Id { get; set; }

        public int? AnswerId { get; set; }

        public int? QuestionId { get; set; }

        public List<AddEditKeyboardQuestionAnswerElementViewModel> Answer { get; set; } = new List<AddEditKeyboardQuestionAnswerElementViewModel>();
    }
}
