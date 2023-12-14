using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyImageRelation : BaseEntity
    {
        public KeyboardVariableKeyVariation Variation { get; set; }
        public int VariationId { get; set; }

        public Keyboard Keyboard { get; set; }
        public int KeyboardId { get; set; }

        public string ReplacementCharacter { get; set; }

        public List<AbastractKeyboardAnswerElement> AnswerElements { get; set; } = new List<AbastractKeyboardAnswerElement>();

    }
}
