using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardNumericKeyRelationViewModel : BaseEntityViewModel
    {
        public int KeyboardId { get; set; }

        public KeyboardNumericKeyViewModel NumericKey { get; set; }

        public int NumericKeyId { get; set; }

        public string KeySimpleForm { get; set; } // example A, B, C, D .... etc in the Keybaord 
                                                  // for simplicity of evaluation in Keyboard Questions

        public int Order { get; set; }
    }
}
