using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Topics
{
    public class SubtopicViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        public int TopicId { get; set; }
        public TopicViewModel Topic { get; set; }

        public List<QuestionBaseViewModel> Questions { get; set; } = new List<QuestionBaseViewModel>();
    }
}
