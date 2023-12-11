using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeriesViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }
        public string AddedByName { get; set; }

        public bool IsRandom { get; set; }
        public int RandomSize { get; set; }

        public List<QuestionSeriesElementViewModel> Elements { get; set; } = new List<QuestionSeriesElementViewModel>();

        public int DataPoolId { get; set; }

        public int NumberOfPools { get; set; }
    }
}
