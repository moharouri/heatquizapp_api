using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Courses
{
    public class Course : BaseEntity, IAddedByCarrier, IImageCarrier
    {
        public string Name { get; set; }

        public string Code { get; set; }

        //Thumbnail
        public string ImageURL { get; set; }

        public long ImageSize { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        //Maps
        public List<CourseMap> CourseMaps { get; set; } = new List<CourseMap>();
    }
}
