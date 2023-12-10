namespace heatquizapp_api.Models.Courses
{
    public class AddBadgeGroupEntitiesViewModel
    {
        public int Id { get; set; }

        public List<IFormFile> Pictures { get; set; } = new List<IFormFile>();

        public List<int> ProgressList { get; set; } = new List<int>();
    }
}
