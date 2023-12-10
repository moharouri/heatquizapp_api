using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class AddClickQuestionSingleStepViewModel : AddQuestionSingleStepViewModel
    {
        public string ClickParts { get; set; }

    }
}
