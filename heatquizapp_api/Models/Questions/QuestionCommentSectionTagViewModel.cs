using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentSectionTagViewModel : BaseEntityViewModel
    {
        public QuestionCommentSectionViewModel Section { get; set; }
        public int SectionId { get; set; }

        public string AddedByName { get; set; }
    }
}
