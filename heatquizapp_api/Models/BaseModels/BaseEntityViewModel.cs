namespace HeatQuizAPI.Models.BaseModels
{
    public class BaseEntityViewModel
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public int DataPoolId { get; set; }
    }
}
