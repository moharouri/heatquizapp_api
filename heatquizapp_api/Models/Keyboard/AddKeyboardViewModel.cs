namespace heatquizapp_api.Models.Keyboard
{
    public class AddKeyboardViewModel
    {
        public string Name { get; set; }

        public List<AddKeyboardIncludeKeyViewModel> NumericKeys { get; set; } = new List<AddKeyboardIncludeKeyViewModel>();

        //Variable Keys
        public List<AddKeyboardIncludeKeyViewModel> VariableKeys { get; set; } = new List<AddKeyboardIncludeKeyViewModel>();

        public int DataPoolId { get; set; }
    }
}
