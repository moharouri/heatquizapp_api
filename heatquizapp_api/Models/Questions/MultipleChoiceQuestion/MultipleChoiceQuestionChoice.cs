using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Questions.MultipleChoiceQuestion
{
    public class MultipleChoiceQuestionChoice : BaseEntity, IImageCarrier
    {
        public MultipleChoiceQuestion Question { get; set; }
        public int QuestionId { get; set; }

        public string? Latex { get; set; }

        public string? ImageURL { get; set; }

        public bool Correct { get; set; }
    }
}
