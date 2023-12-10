namespace heatquizapp_api.Models.Keyboard
{
    public class KeysSearchViewModel
    {
        public string Code { get; set; }

        public int Page { get; set; }
        public int QperPage { get; set; }

        public int? ListId { get; set; }

        public bool GetNumeric { get; set; }

        public int DataPoolId { get; set; }
    }
}
