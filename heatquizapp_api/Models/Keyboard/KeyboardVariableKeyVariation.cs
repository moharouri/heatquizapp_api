using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyVariation : BaseEntity
    {
        public KeyboardVariableKey Key { get; set; }
        public int KeyId { get; set; }

        public string TextPresentation { get; set; }

        public List<AbastractKeyboardAnswerElement> AnswerElements { get; set; } = new List<AbastractKeyboardAnswerElement>();

    }
}
