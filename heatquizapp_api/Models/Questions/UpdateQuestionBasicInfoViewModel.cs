namespace heatquizapp_api.Models.Questions
{
    public class UpdateQuestionBasicInfoViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int LevelOfDifficultyId { get; set; }
        public int SubtopicId { get; set; }

        public int DatapoolId { get; set; }

    }
}
