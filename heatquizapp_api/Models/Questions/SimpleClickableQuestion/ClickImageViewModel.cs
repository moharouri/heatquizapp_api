using heatquizapp_api.Models.ClickImageTrees;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ClickImageViewModel : ClickablePartViewModel
    {
        //Answer
        public ImageAnswerViewModel Answer { get; set; }
        public int AnswerId { get; set; }
       
        //Question
        public int QuestionId { get; set; }
    }
}
