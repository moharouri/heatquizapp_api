namespace heatquizapp_api.Models.ClickImageTrees
{
    public class AddNodeViewModel
    {
        public string Name { get; set; }

        public int GroupId {  get; set; } 
            
        public int? RootId { get; set; }
            
        public IFormFile Picture { get; set; }
    }
}
