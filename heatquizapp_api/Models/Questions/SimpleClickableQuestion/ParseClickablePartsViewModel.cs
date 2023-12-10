namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class ParseClickablePartsViewModel
    {
        //Clickable Images
        public List<ClickImageViewModel> ClickImages { get; set; } = new List<ClickImageViewModel>();

        //Clickable Charts
        public List<ClickChartViewModel> ClickCharts { get; set; } = new List<ClickChartViewModel>();
    }
}
