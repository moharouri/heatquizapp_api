using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;
using System.Xml.Linq;

namespace heatquizapp_api.Controllers.CourseController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public CourseController(
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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllCourses([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var DPExists = await _applicationDbContext.DataPools
                .AnyAsync(d => d.Id == VM.DatapoolId);

            if (!DPExists)
                return NotFound("Datapool does not exist");

            var Courses = await _applicationDbContext.Courses
                .OrderBy(c => c.Name)
                .Include(c => c.AddedBy)
                .Where(c => c.DataPoolId == VM.DatapoolId)
                .ToListAsync();

            return Ok(_mapper.Map<List<Course>, List<CourseViewModel>>(Courses));
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetCourseById(int Id)
        {
            var Course = await _applicationDbContext.Courses
                .Include(c => c.AddedBy)
                .Include(c => c.CourseMaps)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (Course is null)
                return NotFound("Course not found");

            return Ok(_mapper.Map<Course, CourseViewModel>(Course));
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddCourseSingleStep([FromForm] AddCourseViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check code not null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code can't be empty");

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check name not Taken 
            var nameTaken = await _applicationDbContext.Courses
                .AnyAsync(c => c.Name == VM.Name && c.DataPoolId == DP.Id);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Check code not Taken 
            var codeTaken = await _applicationDbContext.Courses
                .AnyAsync(c => c.Code == VM.Code);

            if (codeTaken)
                return BadRequest("Code taken, choose different code");

            //Check picture
            if (VM.Picture is null)
                return BadRequest("Please provide a picture");

            //Verify Extension
            var extensionIsValid = validateImageExtension(VM.Picture);
        
            if (!extensionIsValid)
                return BadRequest("Picture extenstion not valid");

            //Get adder
            var Adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Course
            var course = new Course()
            {
                Name = VM.Name,
                Code = VM.Code,
                AddedById = Adder.Id,
                DataPoolId = DP.Id
            };

            //Save picture and url path for it
            var URL = await SaveFile(VM.Picture);

            course.ImageURL = URL;
            course.ImageSize = VM.Picture.Length;

            _applicationDbContext.Courses.Add(course);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Course, CourseViewModel>(course));
        }

        [HttpPut("[action]")]
        //Change type on vs code
        public async Task<IActionResult> EditCourseSingleStep([FromForm] EditCourseViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Name Not Null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code  can't be empty");

            //Check course exists
            var Course = await _applicationDbContext.Courses
                .FirstOrDefaultAsync(c => c.Id == VM.CourseId);

            if (Course is null)
                return NotFound("Course not found");

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check Name/Code Not Taken 
            var nameTaken = await _applicationDbContext.Courses
                .AnyAsync(c => c.Name == VM.Name && c.Id != VM.CourseId && c.DataPoolId == DP.Id);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            var codeTaken = await _applicationDbContext.Courses
                .AnyAsync(c => c.Code == VM.Code && c.Id != VM.CourseId && c.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code taken, choose different code");

            //Edit Course Name
            Course.Name = VM.Name;
            Course.Code = VM.Code;

            if (VM.SameImage.HasValue && !VM.SameImage.Value)
            {

                //Check Picture
                if (VM.Picture is null)
                    return BadRequest("Please provide a picture");

                //Verify Extension
                var extensionIsValid = validateImageExtension(VM.Picture);

                if (!extensionIsValid)
                    return BadRequest("Picture extenstion not valid");

                //Save picture and url path for it
                var URL = await SaveFile(VM.Picture);

                Course.ImageURL = URL;
                Course.ImageSize = VM.Picture.Length;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Course, CourseViewModel>(Course));
        }

    }
}
