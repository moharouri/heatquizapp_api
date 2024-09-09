using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapBadgeSystemEntityViewModel : BaseEntityViewModel
    {
        public int SystemId { get; set; }

        //Was URL
        public string URL { get; set; }

        public string ImageURL { get; set; }
        public long ImageSize { get; set; }

        public int Progress { get; set; }
    }
}
