using HeatQuizAPI.Database;
using HeatQuizAPI.Mapping;
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

        private readonly List<string> NOTALLOWED_NAMES = new List<string>() {"admin", "student"};

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
        public async Task<IActionResult> CheckUserToken()
        {
            var currentUser = await getCurrentUser(_contextAccessor, _userManager);

            var Roles = await _userManager.GetRolesAsync(currentUser);

            return Ok(new
            {
                username = currentUser.UserName,
                name = currentUser.Name,
                userProfile = !string.IsNullOrEmpty(currentUser.ProfilePicture)
                ? MappingProfile.FILES_PATH + currentUser.ProfilePicture : null,
                roles = Roles
            });
        }

        [HttpGet("[action]")]
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
        [Authorize("admin")]
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

        [HttpPost("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> AddUser([FromBody] RegisterUserViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Name Taken
            var nameTaken = await _applicationDbContext.Users
                .AnyAsync(u => u.Name.ToUpper() == VM.Name.ToUpper() || u.UserName.ToUpper() == VM.Username.ToUpper());

            if (nameTaken)
                return BadRequest("Name/Username already exists");

            //User cannot have "admin" or "student" names
            if (NOTALLOWED_NAMES.Any(n => VM.Name.ToUpper() == n.ToUpper()))
                return BadRequest("Choose another name");

            //Create user
            var user = new User
            {
                UserName = VM.Username,
                Email = VM.Email,
                Name = VM.Name,
                RegisteredOn = DateTime.UtcNow
            };

            //Add user
            var result = await _userManager.CreateAsync(user, VM.Password);

            if (!result.Succeeded)
                return BadRequest("User could not be created");

            //Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            //Add course editor role
            //this implies that only one admin exists in the system and other users are exclusively course editors
            await _userManager.AddToRoleAsync(user, "course_editor");

            return Ok();
        }

        [HttpPut("[action]")]
        //change type 
        public async Task<IActionResult> EditNameEmail([FromBody] EditUserInfoViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            if (string.IsNullOrEmpty(VM.Name) || string.IsNullOrEmpty(VM.Email))
                return BadRequest("Please provide a name and an email");

            //Check user exists
            var User = await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == VM.Username);

            if (User is null)
                return NotFound("User not found");

            //Check Name Taken
            var nameTaken = await _applicationDbContext.Users
                .AnyAsync(u => u.Name.ToUpper() == VM.Name.ToUpper() && u.UserName.ToUpper() != VM.Username.ToUpper());

            if (nameTaken)
                return BadRequest("Name alraedy taken");

            User.Name = VM.Name;
            User.Email = VM.Email;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

     
        [HttpPut("[action]")]
        //Remove Username from website change type 
        public async Task<IActionResult> UpdateProfilePicture(IFormFile Picture)
        {
            var currentUser = await getCurrentUser(_contextAccessor, _userManager);

            if (currentUser == null)
                return BadRequest("Unable to find user");

            var User = await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            //Check image provided
            if (Picture is null)
                return BadRequest("Please provide picture");

            //Verify extension
            var isExtensionValid = validateImageExtension(Picture);
            
            if (!isExtensionValid)
                return BadRequest("Picture extenstion not valid");


            //Save image and get url
             var URL = await SaveFile(Picture);

            User.ProfilePicture = URL;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPut("[action]")]
        //Remove Username from website change type 
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var currentUser = await getCurrentUser(_contextAccessor, _userManager);

            if (currentUser == null)
                return BadRequest("Unable to find user");

            var User = await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            User.ProfilePicture = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
