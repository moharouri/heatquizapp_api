namespace heatquizapp_api.Models.CourseMapElementImages
{
    public class AddMapElementImagesViewModel
    {
        public string Code { get; set; }
        public IFormFile Play { get; set; }
        public IFormFile PDF { get; set; }
        public IFormFile Video { get; set; }
        public IFormFile Link { get; set; }
        public int DataPoolId { get; set; }
    }
}
