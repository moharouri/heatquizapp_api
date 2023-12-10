namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class AddKeyboardQuestionSingleStepViewModel : AddQuestionSingleStepViewModel
    {
        public int? DefaultImageId { get; set; }

        public bool IsEnergyBalance { get; set; }
        public bool DisableDivision { get; set; }

        public int KeyboardId { get; set; }
        public string AnswerForLatex { get; set; }

        public string AnswersString { get; set; }
    }
}
