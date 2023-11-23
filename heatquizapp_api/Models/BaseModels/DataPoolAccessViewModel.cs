using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.BaseModels
{
    public class DataPoolAccessViewModel
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public DataPool DataPool { get; set; }
        public int DataPoolId { get; set; }

        public string UserName { get; set; }
    }
}
