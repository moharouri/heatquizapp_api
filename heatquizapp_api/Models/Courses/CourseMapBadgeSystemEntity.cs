using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapBadgeSystemEntity : BaseEntity, IImageCarrier
    {
        public CourseMapBadgeSystem System { get; set; }
        public int SystemId { get; set; }
        
        public string ImageURL { get; set; }
        public long ImageSize { get; set; }

        public int Progress { get; set; }
    }
}
