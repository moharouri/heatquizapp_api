using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.KeyboardQuestion;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardNumericKeyRelation : BaseEntity
    {
        public Keyboard Keyboard { get; set; }
        public int KeyboardId { get; set; }

        public KeyboardNumericKey NumericKey { get; set; }
        public int NumericKeyId { get; set; }

        public int Order { get; set; }

        public string KeySimpleForm { get; set; }

        public List<AbastractKeyboardAnswerElement> AnswerElements { get; set; } = new List<AbastractKeyboardAnswerElement>();
    }
}
