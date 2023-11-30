using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }

        public string Code { get; set; }

        //Thumbnail
        public string URL { get; set; }

        public long Size { get; set; }

        public string AddedByName { get; set; }

        //public List<CourseMapViewModel> CourseMaps { get; set; } = new List<CourseMapViewModel>();
    }
}
