using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.BaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.AccountController
{

    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext applicationDbContext,
            IHttpContextAccessor contextAccessor
         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = await getCurrentUser(_contextAccessor, _userManager);

            var users = await _applicationDbContext.Users.ToListAsync();

            return Ok(users.Select(user => new {
                Name = user.Name,
                RegisteredOn = user.RegisteredOn.ToString("d", new CultureInfo("de-De")),
                Email = user.Email,
                PlayerKeys = new List<string>(),
                ProfilePicture = string.Empty,
            }));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsersAdmin()
        {
            var users = await _applicationDbContext.Users.ToListAsync();

            var VMUsers = new List<dynamic>();

            foreach (var user in users) { 
                var roles = _userManager.GetRolesAsync(user);

                VMUsers.Add(new
                {
                    Username = user.UserName,
                    Name = user.Name,
                    RegisteredOn = user.RegisteredOn.ToString("d", new CultureInfo("de-De")),
                    Email = user.Email,
                    Roles = roles,
                    ProfilePicture = String.Empty
                });
            }

            return Ok(VMUsers);
        }

        [HttpGet("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> AddUser([FromBody] RegisterUserViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            return Ok();
        }
    }
}
