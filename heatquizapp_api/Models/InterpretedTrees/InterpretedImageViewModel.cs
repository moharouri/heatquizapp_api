using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.InterpretedTrees
{
    public class InterpretedImageViewModel : BaseEntityViewModel
    {
        public string Code { get; set; }

        //Left
        public InterpretationValueViewModel Left { get; set; }
        public int LeftId { get; set; }

        //Right
        public InterpretationValueViewModel Right { get; set; }
        public int RightId { get; set; }

        //Ratio
        public InterpretationValueViewModel RationOfGradients { get; set; }
        public int RationOfGradientsId { get; set; }

        //Jump
        public InterpretationValueViewModel Jump { get; set; }
        public int JumpId { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long Size { get; set; }

        //Group
        public InterpretedImageGroupViewModel Group { get; set; }
        public int GroupId { get; set; }

        //public List<ClickChartViewModel> ClickCharts { get; set; } = new List<ClickChartViewModel>();
    }
}
