namespace heatquizapp_api.Models.QuestionInformation
{
    public class AssignQuestionsToExplanationViewModel
    {
        public int Id { get; set; }  

        public List<int> QuestionIds { get; set; } = new List<int>();
    }
}
