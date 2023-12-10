namespace heatquizapp_api.Models.Courses
{
    public class MapElementLinkViewModel
    {
        public CourseMapElementViewModel Element { get; set; }
        public int ElementId { get; set; }

        public CourseMapViewModel Map { get; set; }
        public int MapId { get; set; }
    }
}
