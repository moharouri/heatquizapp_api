using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography.Xml;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.FeedbackController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [Authorize]
    [ApiController]
    public class FeedbackController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public FeedbackController(
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
        public async Task<IActionResult> GetFeedback([FromBody] FeedbackSearchViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get user to update notifications status
            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            //Parse datetime
            DateTime From = DateTime.Now;
            DateTime To = DateTime.Now;

            if (!string.IsNullOrEmpty(VM.FromDate) && !string.IsNullOrEmpty(VM.ToDate))
            {
                try
                {
                    From = DateTime.ParseExact(VM.FromDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    To = DateTime.ParseExact(VM.ToDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return BadRequest("invalid date format");
                }
            }

            //Get questions 
            var Questions = await _applicationDbContext.QuestionBase
                .Include(q => q.StudentFeedback)
                .Include(q => q.AddedBy)
                .Where(q =>
                    q.DataPoolId == VM.DataPoolId &&
                    q.StudentFeedback.Any(stf => stf.DateCreated >= From && stf.DateCreated <= To)
                 )
                .ToListAsync();

            //Sort questions
            var questionsSorted = Questions
                .OrderByDescending((q) => q.StudentFeedback.OrderByDescending(stf => stf.DateCreated).First().DateCreated)
                .Select(q => new {
                    data = _mapper.Map<QuestionBaseViewModel>(q),

                    feedback = _mapper.Map
                    <List<QuestionStudentFeedbackViewModel>>
                    (
                        q.StudentFeedback.Where(c => c.DateCreated >= From && c.DateCreated <= To).OrderByDescending(f => f.DateCreated)
                    )
                })
                .ToList();

            //Update last seen in notifications subscribers
            /*var notificationSubscriber = await _applicationDbContext.DatapoolNotificationSubscriptions
                .FirstOrDefaultAsync(a => a.UserId == user.Id && a.DatapoolId == VM.DataPoolId);

            if (notificationSubscriber != null)
            {
                notificationSubscriber.LastSeen = DateTime.Now;

                await _applicationDbContext.SaveChangesAsync();
            }*/

            return Ok(questionsSorted);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetQuestionFeedback([FromBody] UniversalAccessByIdViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Question = await _applicationDbContext.QuestionBase
                .Include(q => q.StudentFeedback)

                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

            return Ok(_mapper.Map<List<QuestionStudentFeedbackViewModel>>(Question.StudentFeedback.OrderByDescending(f => f.DateCreated)));
        }

    }
}
