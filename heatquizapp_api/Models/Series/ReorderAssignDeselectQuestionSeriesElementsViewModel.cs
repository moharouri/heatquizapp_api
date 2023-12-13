namespace heatquizapp_api.Models.Series
{
    public class ReorderAssignDeselectQuestionSeriesElementsViewModel
    {
        public int SeriesId { get; set; }

        public int? ElementId { get; set; }
        
        public List<AddSeriesIncludeQuestionSeriesElementViewModel> Questions { get; set; } = new List<AddSeriesIncludeQuestionSeriesElementViewModel>();
        public List<QuestionSeriesElementReorderViewModel> Elements { get; set; } = new List<QuestionSeriesElementReorderViewModel>();
    }
}
