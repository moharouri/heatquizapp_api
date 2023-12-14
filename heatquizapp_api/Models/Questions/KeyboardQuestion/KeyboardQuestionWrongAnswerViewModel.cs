using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionWrongAnswerViewModel : BaseEntityViewModel
    {
        public int QuestionId { get; set; }

        public string AnswerLatex { get; set; }
    }
}
