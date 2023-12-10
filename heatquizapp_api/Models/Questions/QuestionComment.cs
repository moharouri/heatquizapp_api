using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionComment : BaseEntity, IAddedByCarrier
    {
        public QuestionCommentSection CommentSection { get; set; }
        public int CommentSectionId { get; set; }

        public string Text { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public List<QuestionCommentTag> Tages { get; set; } = new List<QuestionCommentTag>();
    }

}
