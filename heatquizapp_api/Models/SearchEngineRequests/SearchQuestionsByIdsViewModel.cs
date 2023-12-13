namespace heatquizapp_api.Models.SearchEngineRequests
{
    public class SearchQuestionsByIdsViewModel
    {
        public List<int> Ids { get; set; } = new List<int>();

        public List<CodeNumberViewModel> Codes { get; set; } = new List<CodeNumberViewModel>();
        public int NumberOfQuestions { get; set; }
        public List<KeyValuePair<int, int>> QuestionsIdsTypes { get; set; } = new List<KeyValuePair<int, int>>();
    }
}
