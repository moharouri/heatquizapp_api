using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Keyboard;

namespace heatquizapp_api.Models.Questions
{
    public class AbastractKeyboardAnswerElementViewModel: BaseEntity
    {
        public KeyboardNumericKeyRelationViewModel NumericKey { get; set; }
        public int? NumericKeyId { get; set; }

        public KeyboardVariableKeyImageRelationViewModel Image { get; set; }
        public int? ImageId { get; set; }

        public string TextPresentation { get; set; }

        public string Value { get; set; }

     
    }
}
