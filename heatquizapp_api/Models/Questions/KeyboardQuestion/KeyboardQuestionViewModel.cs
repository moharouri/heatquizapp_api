namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionViewModel : QuestionBaseViewModel
    {
        public bool IsEnergyBalance { get; set; }

        public bool DisableDivision { get; set; }

        public Keyboard.KeyboardViewModel Keyboard { get; set; }
        public int KeyboardId { get; set; }

        //Answers
        public List<KeyboardQuestionAnswerViewModel> Answers { get; set; } = new List<KeyboardQuestionAnswerViewModel>();

        //Wrong answers
        public List<KeyboardQuestionWrongAnswerViewModel> WrongAnswers { get; set; } = new List<KeyboardQuestionWrongAnswerViewModel>();
    }
}
