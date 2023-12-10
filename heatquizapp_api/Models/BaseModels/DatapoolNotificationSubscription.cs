using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.BaseModels
{
    public class DatapoolNotificationSubscription
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastSeen { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }

        public DataPool Datapool { get; set; }
        public int DatapoolId { get; set; }
    }
}
