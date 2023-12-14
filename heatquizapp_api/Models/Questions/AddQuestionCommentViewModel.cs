namespace heatquizapp_api.Models.Questions
{
    public class AddQuestionCommentViewModel
    {
        public int QuestionId { get; set; } 
        public string Comment { get; set; }

        public  List<string> Tags { get; set; } = new List<string>();
    }
}
