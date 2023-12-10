namespace heatquizapp_api.Models.Keyboard
{
    public class SwabKeyboardKeysViewModel
    {
        public int KeyboardId { get; set; }

        public int FirstKeyId { get; set; }

        public bool IsFirstNumeric { get; set; }

        public int SecondKeyId { get; set; }

        public bool IsSecondNumeric { get; set; }
    }
}
