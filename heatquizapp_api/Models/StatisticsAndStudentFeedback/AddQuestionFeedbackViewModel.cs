using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class AddQuestionFeedbackViewModel
    {
        public int QuestionId { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string Feedback { get; set; }
        public string Player { get; set; }
    }
}
