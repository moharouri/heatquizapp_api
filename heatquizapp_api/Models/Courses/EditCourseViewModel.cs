namespace heatquizapp_api.Models.Courses
{
    public class EditCourseViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public IFormFile Picture { get; set; }
        public int DataPoolId { get; set; }
        public int CourseId { get; set; }
        public bool SameImage { get; set; }
    }
}
