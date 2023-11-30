using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswerGroup : BaseEntity, IAddedByCarrier
    {
        public string Name { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        //Trees
        public List<ImageAnswer> Images { get; set; } = new List<ImageAnswer>();
    }
}
