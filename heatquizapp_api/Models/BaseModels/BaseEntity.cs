
namespace HeatQuizAPI.Models.BaseModels
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public DataPool DataPool { get; set; }
        public int DataPoolId { get; set; }
    }
}
