using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.BaseModels
{
    public class DataPoolAccess
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public DataPool DataPool { get; set; }
        public int DataPoolId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
    }
}
