using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class QuestionStatistic : BaseEntity
    {
        public QuestionBase Question { get; set; }
        public int QuestionId { get; set; }

        public string Score { get; set; }
        public int TotalTime { get; set; }

        public bool Correct { get; set; }

        public string Key { get; set; }

        public string Player { get; set; }
    }
}
