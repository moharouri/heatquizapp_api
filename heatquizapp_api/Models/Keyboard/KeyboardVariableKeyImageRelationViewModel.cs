using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyImageRelationViewModel : BaseEntityViewModel
    {
        public KeyboardVariableKeyVariationViewModel Variation { get; set; }
        public int VariationId { get; set; }

        public int KeyboardId { get; set; }

        public string ReplacementCharacter { get; set; }
    }
}
