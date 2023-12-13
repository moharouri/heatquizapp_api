namespace heatquizapp_api.Models.InterpretedTrees
{
    public class UpdateInterpretedNodeViewModel
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        //Left
        public int? LeftId { get; set; }

        //Right
        public int? RightId { get; set; }

        //Ratio
        public int? RatioId { get; set; }

        //Jump
        public int? JumpId { get; set; }

        //Image
        public IFormFile? Picture{ get; set; }

    }
}
