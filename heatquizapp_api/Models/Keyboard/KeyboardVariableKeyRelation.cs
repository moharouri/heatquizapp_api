using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardVariableKeyRelation : BaseEntity
    {
        public Keyboard Keyboard { get; set; }
        public int KeyboardId { get; set; }

        public KeyboardVariableKey VariableKey { get; set; }
        public int VariableKeyId { get; set; }

        public int Order { get; set; }
    }
}
