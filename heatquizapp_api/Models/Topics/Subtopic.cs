using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.Topics
{
    public class Subtopic : BaseEntity
    {
        public string Name { get; set; }

        public Topic Topic { get; set; }
        public int TopicId { get; set; }

        //Questions 
        public List<QuestionBase> Questions { get; set; } = new List<QuestionBase>();
    }
}
