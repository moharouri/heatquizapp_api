namespace heatquizapp_api.Models.CourseMapElementImages
{
    public class UpdateMapElementImagesImageViewModel
    {
        public int Id { get; set; }
        public IFormFile Picture { get; set; }
        public EDIT_TYPE EditType { get; set; }
    }

    public enum EDIT_TYPE
    {
        PLAY = 0,
        PDF = 2,
        VIDEO = 4,
        LINK = 8
    }
}
