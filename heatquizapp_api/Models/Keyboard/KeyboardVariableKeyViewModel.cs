using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        public string TextPresentation { get; set; }

        //Variations -- previously Images
        public List<KeyboardVariableKeyVariationViewModel> Variations { get; set; } = new List<KeyboardVariableKeyVariationViewModel>();

        //Key list
        public KeysList KeysList { get; set; }
        public int KeysListId { get; set; }

        public List<KeyboardVariableKeyRelationViewModel> Relations { get; set; } = new List<KeyboardVariableKeyRelationViewModel>();
    }
}
