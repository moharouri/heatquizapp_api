using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapPDFStatistics : BaseEntity
    {
        public CourseMapElement Element { get; set; }
        public int ElementId { get; set; }

        public string Player { get; set; }

        public bool OnMobile { get; set; }
    }
}
