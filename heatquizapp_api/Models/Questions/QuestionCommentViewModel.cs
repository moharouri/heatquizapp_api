namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentViewModel
    {
        public QuestionCommentSectionViewModel CommentSection { get; set; }
        public int CommentSectionId { get; set; }

        public string Text { get; set; }

        public string AddedByName { get; set; }
        public string AddedByProfilePicture { get; set; }

        public List<QuestionCommentTagViewModel> Tages { get; set; } = new List<QuestionCommentTagViewModel>();
    }
}
