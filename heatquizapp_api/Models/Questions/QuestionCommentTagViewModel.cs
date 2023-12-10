using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentTagViewModel : BaseEntityViewModel
    {
        public QuestionCommentViewModel Comment { get; set; }
        public int CommentId { get; set; }
        public string AddedByName { get; set; }
    }
}
