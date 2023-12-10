using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class MapElementLink : BaseEntity
    {
        public CourseMapElement Element { get; set; }
        public int ElementId { get; set; }

        public CourseMap Map { get; set; }
        public int MapId { get; set; }
    }
}
