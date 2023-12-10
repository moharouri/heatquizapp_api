namespace heatquizapp_api.Models.Questions.SimpleClickableQuestion
{
    public class RemoveUpdateClickablePartViewModel
    {
        public int Id { get; set; }
        public bool IsImage { get; set; }

        public int AnswerId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
