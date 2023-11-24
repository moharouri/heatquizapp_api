using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HeatQuizAPI.Utilities.Constants;

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

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager
         ) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
           
            return Ok("Test");
        }
    }
}
