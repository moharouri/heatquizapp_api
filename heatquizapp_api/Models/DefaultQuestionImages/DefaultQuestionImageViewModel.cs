using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.DefaultQuestionImages
{
    public class DefaultQuestionImageViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        public string AddedByName { get; set; }

        public string ImageURL { get; set; }
    }
}
