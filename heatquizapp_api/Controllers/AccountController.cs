using HeatQuizAPI.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static HeatQuizAPI.Utilities.Constants;

namespace HeatQuizAPI.Controllers
{

    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginForm VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(HTTP_REQUEST_INVALID_DATA);

            //Get users

            //Check password

            //Check datapool access

            //Create token, and embed roles


            //Send response
            return Ok("Test");
        }
    }
}
