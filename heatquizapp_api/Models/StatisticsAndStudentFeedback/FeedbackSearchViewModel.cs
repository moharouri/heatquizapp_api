namespace heatquizapp_api.Models.StatisticsAndStudentFeedback
{
    public class FeedbackSearchViewModel
    {
        public string encryption { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }
        public int DataPoolId { get; set; }
    }
}
