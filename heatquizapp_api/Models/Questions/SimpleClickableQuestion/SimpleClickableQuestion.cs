namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class SimpleClickableQuestion : QuestionBase
    {
        //Clickable Images
        public List<ClickImage> ClickImages { get; set; } = new List<ClickImage>();

        //Clickable Charts
        public List<ClickChart> ClickCharts { get; set; } = new List<ClickChart>();
    }
}
