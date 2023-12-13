namespace heatquizapp_api.Models.SearchEngineRequests
{
    public class SearchQuestionsViewModel
    {

        //Basic info
        public int? Topic { get; set; }
        public int? Subtopic { get; set; }

        public int? LevelOfDifficulty { get; set; }

        public string? Code { get; set; }


        public int Page { get; set; }
        public int QperPage { get; set; }


        //Search based on question types
        public bool SearchBasedOnQuestionTypes { get; set; }

        public bool ShowClickableQuestions { get; set; }
        public bool ShowKeyboardQuestions { get; set; }
        public bool ShowMultipleChoiceQuestions { get; set; }

        //Search based on median play time
        public bool SearchBasedOnMedianTime { get; set; }

        public int? MedianTime1 { get; set; }
        public int? MedianTime2 { get; set; }

        //Search based on play success rates
        public bool SearchBasedOnPlayStats { get; set; }

        public int? MinimumQuestionPlay { get; set; }
        public int? SuccessRate1 { get; set; }
        public int? SuccessRate2 { get; set; }

        //Datapool
        public int DataPoolId { get; set; }
        
    }
}
