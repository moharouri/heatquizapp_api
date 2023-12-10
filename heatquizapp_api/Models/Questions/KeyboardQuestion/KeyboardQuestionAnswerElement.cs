namespace heatquizapp_api.Models.Questions.KeyboardQuestion
{
    public class KeyboardQuestionAnswerElement : AbastractKeyboardAnswerElement
    {
        public KeyboardQuestionAnswer Answer { get; set; }
        public int AnswerId { get; set; }
    }
}
