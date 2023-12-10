using heatquizapp_api.Models.InterpretedTrees;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ClickChart : ClickablePart
    {
        //Answer
        public InterpretedImage Answer { get; set; }
        public int AnswerId { get; set; }

        //Question
        public SimpleClickableQuestion Question { get; set; }
        public int QuestionId { get; set; }
    }
}
