using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.Courses
{
    public class AddCourseMapBadgeGroupSingleStepViewModel
    {
        public int MapId { get; set; }

        public string Title { get; set; }

        public List<IFormFile> Pictures { get; set; } = new List<IFormFile>();

        public List<int> ProgressList { get; set; } = new List<int>();
    }
}
