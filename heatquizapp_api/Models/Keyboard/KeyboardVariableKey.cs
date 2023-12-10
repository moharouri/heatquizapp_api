using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKey : BaseEntity
    {
        public string Code { get; set; }

        public string TextPresentation { get; set; }

        //Variations -- previously Images
        public List<KeyboardVariableKeyVariation> Variations { get; set; } = new List<KeyboardVariableKeyVariation>();

        //Key list
        public KeysList KeysList { get; set; }
        public int KeysListId { get; set; }

        public List<KeyboardVariableKeyRelation> Relations { get; set; } = new List<KeyboardVariableKeyRelation>();
    }
}
