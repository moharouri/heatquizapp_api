namespace heatquizapp_api.Models.Courses
{
    public class AddEditMapBadgeEntityViewModel
    {
        public int BadgeEntityId { get; set; }

        public int? Progress { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
