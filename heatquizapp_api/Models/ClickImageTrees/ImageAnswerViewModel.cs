using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswerViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }

        //Image
        public string ImageURL { get; set; }
        public string URL { get; set; }
        public long Size { get; set; }

        //Group 
        public ImageAnswerGroupViewModel Group { get; set; }
        public int GroupId { get; set; }

        //Leafs
        public List<ImageAnswerViewModel> Leafs { get; set; } = new List<ImageAnswerViewModel>();

    }
}
