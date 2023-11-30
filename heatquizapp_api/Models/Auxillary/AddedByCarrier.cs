using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.Auxillary
{
    public interface IAddedByCarrier
    {
        public User AddedBy { get; set; }   
    }
}
