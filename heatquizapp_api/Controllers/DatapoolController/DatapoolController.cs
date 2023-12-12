using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.BaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.DatapoolController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DatapoolController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public DatapoolController(
                ApplicationDbContext applicationDbContext,
                IMapper mapper,
                IHttpContextAccessor contextAccessor,
                UserManager<User> userManager
            )
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> GetDataPoolsAdmin()
        {
            var DPs = await _applicationDbContext.DataPools
                .OrderBy(dp => dp.NickName)
                .Include(dp => dp.PoolAccesses)
                .ThenInclude(dpa => dpa.User)
                .ToListAsync();

            return Ok(_mapper.Map<List<DataPoolViewModelAdmin>>(DPs));
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDataPools()
        {
            var DPs = await _applicationDbContext.DataPools
                .Where(dp => !dp.IsHidden)
                .OrderBy(dp => dp.NickName)
                .ToListAsync();

            return Ok(_mapper.Map<List<DataPoolViewModel>>(DPs));
        }

        [HttpPost("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> AddDataPool([FromBody] AddEditDataPoolViewModel VM)
        {
            if(!ModelState.IsValid) 
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check if name or nick name exist already 
            var nameExists = await _applicationDbContext.DataPools
                .AnyAsync(dp => dp.Name.ToUpper() == VM.Name.ToUpper());

            if (nameExists)
                return BadRequest("Name/Nickname already exist");

            //Add
            _applicationDbContext.DataPools.Add(new DataPool()
            {
                Name = VM.Name,
                NickName = VM.NickName,
                IsHidden = false,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
            });

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> EditDataPool([FromBody] AddEditDataPoolViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == VM.Id);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check if name is used
            var nameExists = await _applicationDbContext.DataPools
                .AnyAsync(dp => dp.Name.ToUpper() == VM.Name.ToUpper() && dp.Id != VM.Id);

            if (nameExists)
                return BadRequest("Name/Nickname already exist");

            //Update
            DP.Name = VM.Name;
            DP.NickName = VM.NickName;
            DP.IsHidden = VM.IsHidden;
            DP.DateModified = DateTime.Now;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> EditDataPoolAccess([FromBody] UpdateDataPoolAccessViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .Include(dp => dp.PoolAccesses)
                .FirstOrDefaultAsync(dp => dp.Id == VM.UpdateDataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Get users and check if they exist or repeated
            var Users = await _applicationDbContext.Users
                .Where(u => VM.UsersWithAccess.Any(ua => ua == u.Name))
                .ToListAsync();

            if (Users.Count != VM.UsersWithAccess.Distinct().Count())
                return BadRequest("Users not found or users repeated");

            //Clear access list and repopulate
            DP.PoolAccesses.Clear();

            DP.PoolAccesses.AddRange(Users.Select(u => new DataPoolAccess()
            {
                DataPoolId = DP.Id,
                UserId = u.Id,
            }));

            //Update
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> HideUnhideDataPool([FromBody] AddEditDataPoolViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == VM.Id);

            if (DP is null)
                return NotFound("Not Found");

            //Update
            DP.IsHidden = VM.IsHidden;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserNotificationSubscriptions()
        {
            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            var subscriptions = await _applicationDbContext.DatapoolNotificationSubscriptions
                .Include(a => a.Datapool)
                .Include(a => a.User)
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            return Ok(_mapper.Map<List<DatapoolNotificationSubscriptionViewModel>>(subscriptions));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubscribeNotifications([FromForm] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var datapool = await _applicationDbContext.DataPools
                .Include(dp => dp.PoolAccesses)
                .FirstOrDefaultAsync(a => a.Id == VM.DatapoolId);

            if (datapool is null)
                return NotFound("Datapool not found");

            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            if (!datapool.PoolAccesses.Any(a => a.UserId == user.Id))
                return BadRequest("Not authorized to this datapool");

            var subscription = await _applicationDbContext.DatapoolNotificationSubscriptions
                .FirstOrDefaultAsync(a => a.DatapoolId == datapool.Id && a.UserId == user.Id);

            if (subscription != null)
                return BadRequest("Already subscribed");

            var newSubscription = new DatapoolNotificationSubscription()
            {
                DatapoolId = datapool.Id,
                UserId = user.Id,
                LastSeen = DateTime.Now
            };

            _applicationDbContext.DatapoolNotificationSubscriptions.Add(newSubscription);

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UnsubscribeNotifications([FromForm] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var datapool = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(a => a.Id == VM.DatapoolId);

            if (datapool is null)
                return NotFound("Datapool not found");

            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");


            var subscription = await _applicationDbContext.DatapoolNotificationSubscriptions
                .FirstOrDefaultAsync(a => a.DatapoolId == datapool.Id && a.UserId == user.Id);

            if (subscription is null)
                return BadRequest("Not subscribed");

            _applicationDbContext.DatapoolNotificationSubscriptions.Remove(subscription);

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> RegisterSeenNotifications()
        {
            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            //Update last seen in notifications subscribers
            var notificationSubscribtions = await _applicationDbContext.DatapoolNotificationSubscriptions
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            foreach (var ns in notificationSubscribtions)
            {
                ns.LastSeen = DateTime.Now;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
