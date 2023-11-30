using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswer : BaseEntity, IImageCarrier
    {
        public string Name { get; set; }

        public bool Choosable { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long Size { get; set; }

        //Group 
        public ImageAnswerGroup Group { get; set; }
        public int GroupId { get; set; }

        //Root 
        public ImageAnswer Root { get; set; }
        public int? RootId { get; set; }

        //Leafs
        public List<ImageAnswer> Leafs { get; set; } = new List<ImageAnswer>();

        //public List<ClickImage> ClickImages { get; set; } = new List<ClickImage>();
    }
}
