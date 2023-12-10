namespace heatquizapp_api.Models.Courses
{
    public class AddCourseMapSingleStepViewModel
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public bool ShowBorder { get; set; }

        public IFormFile Picture { get; set; }

        public float LargeMapWidth { get; set; }
        public float LargeMapLength { get; set; }

        public string ElementsString { get; set; }

        public int DataPoolId { get; set; }
    }
}
