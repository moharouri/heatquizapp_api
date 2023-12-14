namespace heatquizapp_api.Models.Keyboard
{
    public class AddVariableKeyViewModel
    {
        public string Code { get; set; }
        public string TextPresentation { get; set; }

        public List<string> Variations { get; set; }
        public int KeysListId { get; set; }
        public int DataPoolId { get; set; }


    }
}
