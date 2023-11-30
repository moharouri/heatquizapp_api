using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.InterpretedTrees
{
    public class InterpretedImageGroupViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public string AddedByName { get; set; }

        //Images
        public List<InterpretedImageViewModel> Images { get; set; } = new List<InterpretedImageViewModel>();
    }
}
