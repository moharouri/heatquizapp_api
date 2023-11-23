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
        public AccountController(
            
         ) 
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }

        
        /*[HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginForm VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(HTTP_REQUEST_INVALID_DATA);

            //Get users

            //Check password

            //Check datapool access

            //Create token, and embed roles

            return Ok("Test");
        }*/

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
           
            return Ok("Test");
        }
    }
}
