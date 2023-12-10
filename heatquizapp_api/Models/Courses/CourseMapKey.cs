using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapKey : BaseEntity
    {
        public CourseMap Map { get; set; }
        public int MapId { get; set; }

        public string Key { get; set; }
    }
}
