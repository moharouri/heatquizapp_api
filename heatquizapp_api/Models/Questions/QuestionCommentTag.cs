using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentTag : BaseEntity
    {
        public QuestionComment Comment { get; set; }
        public int CommentId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
    }
}
