using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionWrongAnswer : BaseEntity
    {
        public KeyboardQuestion Question { get; set; }
        public int QuestionId { get; set; }

        public string AnswerLatex { get; set; }
    }
}
