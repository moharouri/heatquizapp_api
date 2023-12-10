using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentSection : BaseEntity
    {
        public QuestionBase Question { get; set; }
        public int QuestionId { get; set; }

        public List<QuestionCommentSectionTag> Tages { get; set; } = new List<QuestionCommentSectionTag>();

        public List<QuestionComment> Comments { get; set; } = new List<QuestionComment>();


    }
}

