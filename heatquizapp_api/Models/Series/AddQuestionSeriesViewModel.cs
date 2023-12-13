namespace heatquizapp_api.Models.Series
{
    public class AddQuestionSeriesViewModel
    {
        public string Code { get; set; }
        public bool IsRandom { get; set; }
        public int RandomSize { get; set; }

        public List<AddSeriesIncludeQuestionSeriesElementViewModel> Elements { get; set; } = new List<AddSeriesIncludeQuestionSeriesElementViewModel>();

        public int DataPoolId { get; set; }
    }
}
