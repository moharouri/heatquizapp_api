namespace heatquizapp_api.Models.Keyboard
{
    public class AddKeysListViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public List<int> NumericKeys = new List<int>();

        public List<int> VariableKeys = new List<int>();

        public int DataPoolId { get; set; }
    }
}
