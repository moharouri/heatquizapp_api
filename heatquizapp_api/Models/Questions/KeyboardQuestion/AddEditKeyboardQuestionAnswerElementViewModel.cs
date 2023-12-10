namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class AddEditKeyboardQuestionAnswerElementViewModel
    {
        public int? VariationId { get; set; }

        public int? NumericKeyId { get; set; }

        public string Value { get; set; }

        public int Order { get; set; }
    }
}
