using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionCommentSectionTag : BaseEntity, IAddedByCarrier
    {
        public QuestionCommentSection Section { get; set; }
        public int SectionId { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public DateTime LastSeen { get; set; }
    }
}
