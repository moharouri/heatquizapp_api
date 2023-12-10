using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapViewModel : BaseEntityViewModel
    {
        public CourseViewModel Course { get; set; }
        public int CourseId { get; set; }

        public string Title { get; set; }

        public bool ShowBorder { get; set; }
        public bool ShowSolutions { get; set; }

        public bool Disabled { get; set; }

        public string ImageURL { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public List<CourseMapElementViewModel> Elements { get; set; } = new List<CourseMapElementViewModel>();

        public List<CourseMapBadgeSystemViewModel> BadgeSystems { get; set; } = new List<CourseMapBadgeSystemViewModel>();

    }
}
