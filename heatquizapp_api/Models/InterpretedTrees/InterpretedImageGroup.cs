using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.InterpretedTrees
{
    public class InterpretedImageGroup : BaseEntity, IAddedByCarrier
    {
        public string Name { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        //Images
        public List<InterpretedImage> Images { get; set; } = new List<InterpretedImage>();

    }

}
