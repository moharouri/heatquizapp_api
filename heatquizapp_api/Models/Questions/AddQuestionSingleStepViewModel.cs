namespace heatquizapp_api.Models.Questions
{
    public class AddQuestionSingleStepViewModel
    {
        public string Code { get; set; }

        public int SubtopicId { get; set; }
        public int LODId { get; set; }

        public IFormFile Picture { get; set; }
        public int PictureWidth { get; set; }
        public int PictureHeight { get; set; }

        public IFormFile? PDF { get; set; }

        public int DataPoolId { get; set; }
    }
}
