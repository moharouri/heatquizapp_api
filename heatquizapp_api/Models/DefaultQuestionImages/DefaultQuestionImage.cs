using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.DefaultQuestionImages
{
    public class DefaultQuestionImage : BaseEntity, IAddedByCarrier, IImageCarrier
    {
        public string Code { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public string ImageURL { get; set; }
        public long ImageSize { get; set; }
    }
}
