using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentSectionViewModel : BaseEntityViewModel
    {
        public QuestionBaseViewModel Question { get; set; }
        public int QuestionId { get; set; }

        public List<QuestionCommentSectionTagViewModel> Tages { get; set; } = new List<QuestionCommentSectionTagViewModel>();

        public List<QuestionCommentViewModel> Comments { get; set; } = new List<QuestionCommentViewModel>();

    }
}
