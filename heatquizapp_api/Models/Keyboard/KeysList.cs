using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeysList : BaseEntity, IAddedByCarrier
    {
        public string Code { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public List<KeyboardNumericKey> NumericKeys { get; set; } = new List<KeyboardNumericKey>();

        public List<KeyboardVariableKey> VariableKeys { get; set; } = new List<KeyboardVariableKey>();
    }
}
