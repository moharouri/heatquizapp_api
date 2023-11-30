namespace heatquizapp_api.Models.Series
{
    public class AssignElementsToPoolViewModel
    {
        public List<int> SelectedElements { get; set; } = new List<int>();
        public int Pool { get; set; }
    }
}
