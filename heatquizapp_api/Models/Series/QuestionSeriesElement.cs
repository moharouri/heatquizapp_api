using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeriesElement : BaseEntity
    {
        public QuestionSeries Series { get; set; }
        public int SeriesId { get; set; }

        public int Order { get; set; }

        public QuestionBase Question { get; set; }
        public int QuestionId { get; set; }

        public int PoolNumber { get; set; }
    }
}
