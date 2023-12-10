namespace heatquizapp_api.Models.Questions
{
    public class GetCommentsViewModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool GetUnseen { get; set; }
    }
}
