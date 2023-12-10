using heatquizapp_api.Models.ClickImageTrees;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ClickImage : ClickablePart
    {
        //Answer
        public ImageAnswer Answer { get; set; }
        public int AnswerId { get; set; }

        //Question
        public SimpleClickableQuestion Question { get; set; }
        public int QuestionId { get; set; }
    }
}
