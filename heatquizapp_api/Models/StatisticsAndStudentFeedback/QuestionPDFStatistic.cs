using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class QuestionPDFStatistic : BaseEntity
    {
        public QuestionBase Question { get; set; }
        public int QuestionId { get; set; }

        public string Player { get; set; }
        public bool Correct { get; set; }
    }
}
