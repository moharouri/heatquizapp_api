namespace heatquizapp_api.Models.Keyboard
{
    public class KeyboardsSearchViewModel
    {
        public string Code { get; set; }

        public int Page { get; set; }
        public int QperPage { get; set; }

        public int DataPoolId { get; set; }

        public List<int> KeyLists { get; set; } = new List<int>();
    }
}
