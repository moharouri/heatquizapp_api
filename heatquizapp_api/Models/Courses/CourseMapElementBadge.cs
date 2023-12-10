using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapElementBadge : BaseEntity, IImageCarrier
    {
        public CourseMapElement CourseMapElement { get; set; }
        public int CourseMapElementId { get; set; }

        public int Progress { get; set; }

        public string ImageURL { get; set; }
    }
}
