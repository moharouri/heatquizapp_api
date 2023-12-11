using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Courses;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeries : BaseEntity, IAddedByCarrier
    {
        public string Code { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public bool IsRandom { get; set; }
        public int RandomSize { get; set; }
        public int NumberOfPools { get; set; }

        
        //Elements
        public List<QuestionSeriesElement> Elements { get; set; } = new List<QuestionSeriesElement>();

        //Statistics
        public List<QuestionSeriesStatistic> Statistics { get; set; } = new List<QuestionSeriesStatistic>();

        //Relations
        public List<CourseMapElement> MapElements { get; set; } = new List<CourseMapElement>();

    }
}
