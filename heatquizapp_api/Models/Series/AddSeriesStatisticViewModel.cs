namespace heatquizapp_api.Models.Series
{
    public class AddSeriesStatisticViewModel
    {
        public int SeriesId { get; set; }
        public string Player { get; set; }
        public string? MapKey { get; set; }
        public string? MapName { get; set; }
        public string? MapElementName { get; set; }
        public string SuccessRate { get; set; }
        public int TotalTime { get; set; }
        public bool OnMobile { get; set; }
    }
}
