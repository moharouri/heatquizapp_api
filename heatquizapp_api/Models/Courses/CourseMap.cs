using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMap : BaseEntity, IImageCarrier
    {
        public Course Course { get; set; }
        public int CourseId { get; set; }

        public string Title { get; set; }

        public bool ShowBorder { get; set; }
        public bool ShowSolutions { get; set; }

        public bool Disabled { get; set; }

        public string ImageURL { get; set; }
        public long ImageSize { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public List<CourseMapElement> Elements { get; set; } = new List<CourseMapElement>();

        public List<CourseMapBadgeSystem> BadgeSystems { get; set; } = new List<CourseMapBadgeSystem>();

        public List<MapElementLink> Attachments { get; set; } = new List<MapElementLink>();
    }
}
