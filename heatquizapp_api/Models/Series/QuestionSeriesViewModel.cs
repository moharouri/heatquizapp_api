using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Courses;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeriesViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }
        public string AddedByName { get; set; }

        public bool IsRandom { get; set; }
        public int RandomSize { get; set; }

        public List<QuestionSeriesElementViewModel> Elements { get; set; } = new List<QuestionSeriesElementViewModel>();

        public int NumberOfPools { get; set; }

        //Statistics
        public List<QuestionSeriesStatisticViewModel> Statistics { get; set; } = new List<QuestionSeriesStatisticViewModel>();

        //Relations
        public List<CourseMapElementViewModel> MapElements { get; set; } = new List<CourseMapElementViewModel>();
    }
}
