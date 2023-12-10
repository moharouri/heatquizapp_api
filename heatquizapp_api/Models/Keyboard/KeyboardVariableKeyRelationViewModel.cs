using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyRelationViewModel : BaseEntityViewModel
    {
        public int KeyboardId { get; set; }

        public KeyboardVariableKeyViewModel VariableKey { get; set; }
        public int VariableKeyId { get; set; }

        public int Order { get; set; }
    }
}
