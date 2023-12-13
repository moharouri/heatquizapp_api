using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswer : BaseEntity, IImageCarrier
    {
        public string Name { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long Size { get; set; }

        //Group 
        public ImageAnswerGroup Group { get; set; }
        public int? GroupId { get; set; }

        //Root 
        public ImageAnswer Root { get; set; }
        public int? RootId { get; set; }

        //Leafs
        public List<ImageAnswer> Leafs { get; set; } = new List<ImageAnswer>();

        //Relations
        public List<ClickImage> ClickImages { get; set; } = new List<ClickImage>();
    }
}
