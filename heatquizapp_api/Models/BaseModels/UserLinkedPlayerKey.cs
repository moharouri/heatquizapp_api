using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.BaseModels
{
    public class UserLinkedPlayerKey
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string PlayerKey { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
