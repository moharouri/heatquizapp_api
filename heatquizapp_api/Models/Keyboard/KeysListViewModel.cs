using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeysListViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        public string AddedByName { get; set; }

        public List<KeyboardNumericKeyViewModel> NumericKeys { get; set; } = new List<KeyboardNumericKeyViewModel>();

        public List<KeyboardVariableKeyViewModel> VariableKeys { get; set; } = new List<KeyboardVariableKeyViewModel>();
    }
}
