using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapElementBadgeViewModel : BaseEntityViewModel
    {
        public int Progress { get; set; }

        public string ImageURL { get; set; }
    }
}
