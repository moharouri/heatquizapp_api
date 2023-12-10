using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Keyboard;

namespace heatquizapp_api.Models.Questions
{
    public class AbastractKeyboardAnswerElement: BaseEntity
    {
        public KeyboardNumericKeyRelation NumericKey { get; set; }
        public int? NumericKeyId { get; set; }

        public KeyboardVariableKeyImageRelation Image { get; set; }
        public int? ImageId { get; set; }

        public string Value { get; set; }
    }
}
