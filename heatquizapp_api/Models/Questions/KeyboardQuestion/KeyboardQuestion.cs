using heatquizapp_api.Models.Keyboard;
using heatquizapp_api.Models.Series;

namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestion : QuestionBase
    {
        public bool IsEnergyBalance { get; set; }

        public bool DisableDivision { get; set; }

        public Keyboard.Keyboard Keyboard { get; set; }
        public int KeyboardId { get; set; }

        //Answers
        public List<KeyboardQuestionAnswer> Answers { get; set; } = new List<KeyboardQuestionAnswer>();

        //Wrong answers
        public List<KeyboardQuestionWrongAnswer> WrongAnswers { get; set; } = new List<KeyboardQuestionWrongAnswer>();
    }
}
