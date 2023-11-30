using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        //Thumbnail
        public string URL { get; set; }

        public long Size { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        //public List<CourseMap> CourseMaps { get; set; } = new List<CourseMap>();
    }
}
