namespace heatquizapp_api.Models.Courses
{
    public class AddCourseViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public IFormFile Picture { get; set; }
        public int DataPoolId { get; set; }
    }
}
