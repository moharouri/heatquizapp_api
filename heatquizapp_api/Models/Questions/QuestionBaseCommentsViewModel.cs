using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionBaseCommentsViewModel
    {
        public int Type { get; set; }

        public string Code { get; set; }

        //Added By
        public string AddedByName { get; set; }

        public QuestionCommentSectionViewModel CommentSection { get; set; }
        public int CommentSectionId { get; set; }
    }
}
