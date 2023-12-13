namespace heatquizapp_api.Models.Series
{
    public class UpdateSeriesInfoViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public bool IsRandom { get; set; }
        public int RandomSize { get; set; }
    }
}
