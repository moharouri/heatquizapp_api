using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.ClickImageTrees
{
    public class ImageAnswerGroupViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public string AddedByName { get; set; }

        //Trees
        public List<ImageAnswerViewModel> Images { get; set; } = new List<ImageAnswerViewModel>();


    }
    
}
