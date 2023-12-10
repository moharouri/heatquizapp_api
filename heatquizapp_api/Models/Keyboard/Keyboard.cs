using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Questions.KeyboardQuestion;

namespace heatquizapp_api.Models.Keyboard
{
    public class Keyboard : BaseEntity, IAddedByCarrier
    {
        public string Name { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public List<KeyboardNumericKeyRelation> NumericKeys { get; set; } = new List<KeyboardNumericKeyRelation>();

        //Variable Keys
        public List<KeyboardVariableKeyRelation> VariableKeys { get; set; } = new List<KeyboardVariableKeyRelation>();
        public List<KeyboardVariableKeyImageRelation> VariableKeyImages { get; set; } = new List<KeyboardVariableKeyImageRelation>();

        //Question relations 
        public List<KeyboardQuestion> KeyboardQuestions { get; set; } = new List<KeyboardQuestion>();

    }
}
