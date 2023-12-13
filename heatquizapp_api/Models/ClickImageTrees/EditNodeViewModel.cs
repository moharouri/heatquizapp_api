namespace heatquizapp_api.Models.ClickImageTrees
{
    public class EditNodeViewModel
    {
        public string? Name { get; set; }

        public int AnswerId { get; set; }

        public bool? SameImage { get; set; }

        public IFormFile? Picture { get; set; }
    }
}
