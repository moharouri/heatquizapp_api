using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionAnswerViewModel : BaseEntityViewModel
    {
        public int QuestionId { get; set; }

        //Answer sequence
        public List<KeyboardQuestionAnswerElementViewModel> AnswerElements { get; set; } = new List<KeyboardQuestionAnswerElementViewModel>();
    }
}
