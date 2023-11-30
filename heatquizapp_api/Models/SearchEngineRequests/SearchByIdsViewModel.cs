namespace heatquizapp_api.Models.SearchEngineRequests
{
    public class SearchByIdsViewModel
    {
        public List<int> Ids { get; set; } = new List<int>();

        public List<CodeNumberViewModel> Codes { get; set; } = new List<CodeNumberViewModel>();
        public int NumberOfObjects { get; set; }
        public List<int> ObjectsIds { get; set; } = new List<int>();
    }
}
