namespace heatquizapp_api.Models.Courses
{
    public class UpdateMapBasicInfoViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool ShowBorder { get; set; }
        public bool ShowSolutions { get; set; }

        public bool Disabled { get; set; }
    }
}
