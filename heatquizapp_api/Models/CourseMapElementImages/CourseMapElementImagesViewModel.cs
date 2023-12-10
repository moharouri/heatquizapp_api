using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.CourseMapElementImages
{
    public class CourseMapElementImagesViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        public string AddedByName { get; set; }
        public string Play { get; set; }
        public string PDF { get; set; }
        public string Video { get; set; }
        public string Link { get; set; }
    }
}
