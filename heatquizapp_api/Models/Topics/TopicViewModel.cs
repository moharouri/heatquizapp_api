using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Topics
{
    public class TopicViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }

        public string AddedByName { get; set; }

        public bool Active { get; set; }

        //Subtopics
        public List<SubtopicViewModel> Subtopics { get; set; } = new List<SubtopicViewModel>();
    }
}
