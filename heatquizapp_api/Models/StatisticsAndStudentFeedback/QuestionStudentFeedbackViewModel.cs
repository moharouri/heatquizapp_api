using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class QuestionStudentFeedbackViewModel
    {
        public QuestionBaseViewModel Question { get; set; }
        public int QuestionId { get; set; }

        public string Player { get; set; }
        public string FeedbackContent { get; set; }
    }
}
