namespace heatquizapp_api.Models.SearchEngineRequests
{
    public class SeriesSearchViewModel
    {
        public string Code { get; set; }
        public int Page { get; set; }
        public int QperPage { get; set; }

        public int NumberOfSeries { get; set; }

        public string Adder { get; set; }

        public bool Used { get; set; }

        public int DataPoolId { get; set; }
    }
}
