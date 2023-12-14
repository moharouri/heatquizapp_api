using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardNumericKey : BaseEntity
    {
        public string Code { get; set; }

        public string TextPresentation { get; set; }

        public bool IsInteger { get; set; }

        //Key list
        public KeysList KeysList { get; set; }
        public int KeysListId { get; set; }


        //Relations
        public List<KeyboardNumericKeyRelation> Relations { get; set; } = new List<KeyboardNumericKeyRelation>();


        
    }
}
