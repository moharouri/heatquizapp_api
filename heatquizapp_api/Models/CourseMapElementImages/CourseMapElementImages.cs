using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.CourseMapElementImages
{
    public class CourseMapElementImages : BaseEntity, IAddedByCarrier
    {
        public User AddedBy { get; set; }
        public string AddedById { get; set; }
        public string Code { get; set; }

        public string Play { get; set; }
        public string PDF { get; set; }
        public string Video { get; set; }
        public string Link { get; set; }
    }
}
