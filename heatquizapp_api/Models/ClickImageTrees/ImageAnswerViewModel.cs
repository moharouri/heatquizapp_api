using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswerViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long Size { get; set; }

        //Group 
        public ImageAnswerGroupViewModel Group { get; set; }
        public int GroupId { get; set; }

        //Root 
        public ImageAnswerViewModel Root { get; set; }
        public int? RootId { get; set; }

        //Leafs
        public List<ImageAnswerViewModel> Leafs { get; set; } = new List<ImageAnswerViewModel>();

        public List<ClickImageViewModel> ClickImages { get; set; } = new List<ClickImageViewModel>();
    }
}
