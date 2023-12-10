using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapBadgeSystem : BaseEntity
    {
        public string Title { get; set; }

        public CourseMap Map { get; set; }
        public int MapId { get; set; }

        public List<CourseMapBadgeSystemEntity> Entities { get; set; } = new List<CourseMapBadgeSystemEntity>();
    }
}
