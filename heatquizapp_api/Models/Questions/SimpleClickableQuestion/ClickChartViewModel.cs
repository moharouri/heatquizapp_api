using heatquizapp_api.Models.InterpretedTrees;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ClickChartViewModel : ClickablePartViewModel
    {
        //Answer
        public InterpretedImageViewModel Answer { get; set; }
        public int AnswerId { get; set; }

        //Question
        public int QuestionId { get; set; }
    }
}
