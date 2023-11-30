using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Series
{
    public class QuestionSeriesElementViewModel : BaseEntityViewModel
    {
        public int Order { get; set; }

        public QuestionBaseViewModel Question { get; set; }
        public int QuestionId { get; set; }

        public int PoolNumber { get; set; }
    }
}
