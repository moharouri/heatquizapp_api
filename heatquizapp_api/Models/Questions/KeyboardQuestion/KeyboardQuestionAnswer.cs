using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionAnswer : BaseEntity
    {
        public KeyboardQuestion Question { get; set; }
        public int QuestionId { get; set; }

        //Answer sequence
        public List<KeyboardQuestionAnswerElement> AnswerElements { get; set; } = new List<KeyboardQuestionAnswerElement>();
    }
}
