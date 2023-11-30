namespace heatquizapp_api.Models.InterpretedTrees
{
    public class AddInterpretedNodeViewModel
    {
        public string Code { get; set; }

        public int GroupId { get; set; }

        public int JumpId { get; set; }
        public int LeftId { get; set; }
        public int RightId { get; set; }
        public int RatioId { get; set; }

        public IFormFile Picture { get; set; }
    }
}
