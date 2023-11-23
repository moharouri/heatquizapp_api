namespace heatquizapp_api.Models.BaseModels
{
    public class DataPoolViewModel
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public string Name { get; set; }
        public string NickName { get; set; }

        public bool IsHidden { get; set; }
    }
}
