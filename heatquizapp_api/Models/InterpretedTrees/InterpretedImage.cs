using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;

namespace heatquizapp_api.Models.InterpretedTrees
{
    public class InterpretedImage : BaseEntity, IImageCarrier
    {
        public string Code { get; set; }

        //Left
        public LeftGradientValue Left { get; set; }
        public int LeftId { get; set; }

        //Right
        public RightGradientValue Right { get; set; }
        public int RightId { get; set; }

        //Ratio
        public RationOfGradientsValue RationOfGradients { get; set; }
        public int RationOfGradientsId { get; set; }

        //Jump
        public JumpValue Jump { get; set; }
        public int JumpId { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long Size { get; set; }

        //Group
        public InterpretedImageGroup Group { get; set; }
        public int GroupId { get; set; }

        //Relation
        public List<ClickChart> ClickCharts { get; set; } = new List<ClickChart>();
    }

}
