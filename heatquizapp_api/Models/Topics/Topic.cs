using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Topics
{
    public class Topic : BaseEntity, IAddedByCarrier
    {
        public string Name { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public bool Active { get; set; }

        //Subtopics
        public List<Subtopic> Subtopics { get; set; } = new List<Subtopic>();
    }
}
