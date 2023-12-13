using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeriesStatisticViewModel : BaseEntityViewModel
    {
        public int SeriesId { get; set; }

        public string Player { get; set; }
        public string MapKey { get; set; }

        public string MapName { get; set; }
        public string MapElementName { get; set; }

        public string SuccessRate { get; set; }

        public int TotalTime { get; set; }
        public bool OnMobile { get; set; }
    }
}

