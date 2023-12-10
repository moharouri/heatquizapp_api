namespace heatquizapp_api.Models.Courses
{
    public class CopyBadgesViewModel
    {
        public int MapElementId { get; set; }

        public List<int> BadgeEntityIds { get; set; } = new List<int>();
    }
}
