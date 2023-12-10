using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ClickablePartViewModel : BaseEntityViewModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
