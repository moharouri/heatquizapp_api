using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapBadgeSystemViewModel : BaseEntityViewModel
    {
        public string Title { get; set; }

        public int MapId { get; set; }

        public List<CourseMapBadgeSystemEntityViewModel> Entities { get; set; } = new List<CourseMapBadgeSystemEntityViewModel>();
    }
}
