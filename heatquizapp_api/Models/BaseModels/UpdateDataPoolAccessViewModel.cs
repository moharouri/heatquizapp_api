namespace heatquizapp_api.Models.BaseModels
{
    public class UpdateDataPoolAccessViewModel
    {
        public List<string> UsersWithAccess { get; set; } = new List<string>();
        public int UpdateDataPoolId { get; set; }
    }
}
