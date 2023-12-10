using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class MultipleChoiceQuestionChoiceViewModel : BaseEntityViewModel
    {
        public int QuestionId { get; set; }

        public string Latex { get; set; }

        public string ImageURL { get; set; }

        public bool Correct { get; set; }
    }
}
