using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardNumericKeyViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        public string TextPresentation { get; set; }

        public bool IsInteger { get; set; }

        public string KeySimpleForm { get; set; }

        //Key list
        public KeysListViewModel KeysList { get; set; }
        public int KeysListId { get; set; }
    }
}
