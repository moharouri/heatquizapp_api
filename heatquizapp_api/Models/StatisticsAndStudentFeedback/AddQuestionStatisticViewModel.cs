namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class AddQuestionStatisticViewModel
    {
        public int QuestionId { get; set; }

        public string Score { get; set; }
        public int TotalTime { get; set; }

        public bool Correct { get; set; }

        public string? Key { get; set; }
        public string Player { get; set; }

        //For keyboard question
        public string? Latex { get; set; }
    }
}
