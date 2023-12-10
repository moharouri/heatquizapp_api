using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions.KeyboardQuestion;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }

        public string AddedByName { get; set; }

        public List<KeyboardNumericKeyRelationViewModel> NumericKeys { get; set; } = new List<KeyboardNumericKeyRelationViewModel>();

        //Variable Keys
        public List<KeyboardVariableKeyRelationViewModel> VariableKeys { get; set; } = new List<KeyboardVariableKeyRelationViewModel>();
        public List<KeyboardVariableKeyImageRelationViewModel> VariableKeyImages { get; set; } = new List<KeyboardVariableKeyImageRelationViewModel>();

        //Question relations 
        //public List<KeyboardQuestion> KeyboardQuestions { get; set; } = new List<KeyboardQuestion>();
    }
}
