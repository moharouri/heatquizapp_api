namespace heatquizapp_api.Models.BaseModels
{
    public class DatapoolNotificationSubscriptionViewModel
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastSeen { get; set; }

        public DataPoolViewModel Datapool { get; set; }
        public int DatapoolId { get; set; }
    }
}
