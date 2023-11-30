using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Services
{
    public class StatisticsStartDateStorage : IStatisticsStartDateStorage
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public DateTime? StartDate { get; set; }    

        public StatisticsStartDateStorage(
                IHttpContextAccessor contextAccessor,
                UserManager<User> userManager
            ) 
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;

            StartDate = GetStatisticsStartDate().Result;
        }

        async Task<DateTime?> GetStatisticsStartDate()
        {
            var currentUser = await getCurrentUser(_contextAccessor, _userManager);

            return currentUser?.StatisticsStartDate;
        }
    }

    public interface IStatisticsStartDateStorage
    {
        public DateTime? StartDate { get; set; }
    }
}
