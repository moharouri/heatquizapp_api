using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;
using System.Globalization;
using HeatQuizAPI.Mapping;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models;

namespace heatquizapp_api.Controllers.QuestionCommentsController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionCommentsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public QuestionCommentsController(
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
        //Change type in vs code
        public async Task<IActionResult> GetQuestionComments([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get question
            var Question = await _applicationDbContext.QuestionBase
                .Include(q => q.CommentSection)
                .ThenInclude(cs => cs.Comments)
                .ThenInclude(c => c.AddedBy)

                .Include(q => q.CommentSection)
                .ThenInclude(cs => cs.Comments)
                .ThenInclude(c => c.Tages)
                .ThenInclude(t => t.User)

                .Include(q => q.CommentSection)
                .ThenInclude(cs => cs.Tages)
                .ThenInclude(t => t.AddedBy)

                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");


            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            var commentSectionTag = Question.CommentSection != null ?
                Question.CommentSection.Tages.FirstOrDefault(t => t.AddedById == user.Id) : null;

            if (commentSectionTag != null)
            {
                commentSectionTag.LastSeen = DateTime.Now;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionBaseCommentsViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetComments([FromBody] GetCommentsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return BadRequest("User not found");

            DateTime From;
            DateTime To;

            try
            {
                From = DateTime.ParseExact(VM.From, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                To = DateTime.ParseExact(VM.To, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                return BadRequest("Invalid date format");
            }

            var commentsSections = await _applicationDbContext.QuestionCommentSection
                .Where((cs) =>
                cs.Comments.Any(c => c.AddedById != user.Id
                &&
                c.CommentSection.Tages.Any(t => t.AddedById == user.Id)
                &&
                c.DateCreated >= From
                &&
                c.DateCreated <= To))

                .Include(cs => cs.Question)
                .Include(cs => cs.Comments)
                .ThenInclude(c => c.AddedBy)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            return Ok(commentsSections
                .Select(g => {
                    var first_comment = g.Comments.OrderByDescending(a => a.DateCreated).FirstOrDefault();

                    var cs = g;
                    cs.Comments = g.Comments.Where(c => c.DateCreated >= From && c.DateCreated <= To).ToList();

                    return new
                    {
                        AddedByName = first_comment.AddedBy.Name,
                        AddedByProfilePicture = MappingProfile.GetUserProfilePictureURL(first_comment.AddedBy),

                        Text = first_comment.Text,
                        DateCreated = first_comment.DateCreated,

                        CommentSection = _mapper.Map<QuestionCommentSectionViewModel>(cs),

                        NumberOfComments = g.Comments.Count()
                    };
                }).OrderByDescending(a => a.DateCreated));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetUnseenComments([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return Ok(new
                {
                    Count = "not registered"
                });

            var comments = await _applicationDbContext.QuestionComments
                .Include(c => c.AddedBy)

                    .Include(c => c.CommentSection)
                    .ThenInclude(cs => cs.Tages)
                    .Include(c => c.CommentSection)
                    .ThenInclude(cs => cs.Question)
                    .Where(c =>
                    c.AddedById != user.Id
                    &&
                    c.CommentSection.Tages
                    .Any(t => t.AddedById == user.Id && t.LastSeen <= c.DateCreated))

                    .ToListAsync();

            var notificationSubscribtions = await _applicationDbContext.DatapoolNotificationSubscriptions
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            var notificationsDates = notificationSubscribtions.ToDictionary(a => a.DatapoolId);
            var notificationsDPIds = notificationSubscribtions.Select(a => a.DatapoolId).ToList();

            var StudentFeedback = await _applicationDbContext.QuestionStudentFeedback
                .Where(f => f.DateCreated >= DateTime.Today && notificationsDPIds.Any(Id => Id == f.DataPoolId))
                .Include(r => r.Question)
                .Select(r => new
                {
                    Id = r.Id,
                    Player = r.Player,
                    FeedbackContent = r.FeedbackContent,
                    DateCreated = r.DateCreated,
                    New = r.DateCreated > notificationsDates[r.DataPoolId].LastSeen,
                    Question = new
                    {
                        Code = r.Question.Code,
                        Type = r.Question.Type,
                        Id = r.Question.Id,
                        ImageURL = MappingProfile.GetGeneralImageURL(r.Question)
                    }
                })
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();

            return Ok(new
            {
                Count = comments.GroupBy(c => c.CommentSection).Count() + StudentFeedback.Count(a => a.New),
                CountInactive = comments.GroupBy(c => c.CommentSection).Count() + StudentFeedback.Count(),

                Comments = comments.GroupBy(c => c.CommentSection).Select(g => {
                    var first_comment = g.OrderByDescending(a => a.DateCreated).FirstOrDefault();

                    return new
                    {
                        AddedByName = first_comment.AddedBy.Name,
                        AddedByProfilePicture = MappingProfile.GetUserProfilePictureURL(first_comment.AddedBy),

                        Text = first_comment.Text,
                        DateCreated = first_comment.DateCreated,

                        CommentSection = _mapper.Map<QuestionCommentSectionViewModel>(g.Key),

                        NumberOfComments = g.Count()
                    };
                }).OrderByDescending(a => a.DateCreated),

                StudentFeedback = StudentFeedback
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionComment([FromForm] AddQuestionCommentViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Question = await _applicationDbContext.QuestionBase
                .Include(q => q.CommentSection)
                .ThenInclude(cs => cs.Comments)

                .Include(q => q.CommentSection)
                .ThenInclude(cs => cs.Tages)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            //Check question
            if (Question is null)
                return NotFound("Question not found");

            //Check tags unique
            if (VM.Tags.Distinct().Count() != VM.Tags.Count())
                return BadRequest("Tagged user(s) repeated");

            //Check user exists
            var Adder = await getCurrentUser(_contextAccessor, _userManager);

            if (Adder is null)
                return BadRequest("User not found");

            var TaggedUsers = new List<User>();

            foreach (var tag in VM.Tags)
            {
                var taggedUser = await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.Name.ToUpper() == tag.ToUpper());

                if (taggedUser is null)
                    return NotFound("Tagged user not found");

                TaggedUsers.Add(taggedUser);
            }

            if (Question.CommentSection is null)
            {
                Question.CommentSection = new QuestionCommentSection()
                {
                    DataPoolId = Question.DataPoolId
                };
            }


            var non_existing_tagged_users = TaggedUsers.Where(tg => 
            !Question.CommentSection.Tages.Any(cstg => cstg.AddedById == tg.Id) && tg.Id != Adder.Id);

            //Update general tags
            Question.CommentSection.Tages.AddRange(non_existing_tagged_users.Select(a => new QuestionCommentSectionTag()
            {
                AddedById = a.Id,
                DataPoolId = Question.DataPoolId,
                LastSeen = DateTime.Now.AddDays(-1)
            }));

            if (!Question.CommentSection.Tages.Any(t => t.AddedById == Adder.Id))
            {
                Question.CommentSection.Tages.Add(new QuestionCommentSectionTag()
                {
                    AddedById = Adder.Id,
                    DataPoolId = Question.DataPoolId,
                    LastSeen = DateTime.Now.AddDays(-1)
                });
            }

            //add comments
            Question.CommentSection.Comments.Add(new QuestionComment()
            {
                AddedById = Adder.Id,

                Text = VM.Comment,

                Tages = TaggedUsers.Where(tg => tg.Id != Adder.Id).Select(a => new QuestionCommentTag()
                {
                    User = a,
                    DataPoolId = Question.DataPoolId
                }).ToList(),
                DataPoolId = Question.DataPoolId
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterSeenNotification([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);


            var user = await getCurrentUser(_contextAccessor, _userManager);

            if (user is null)
                return NotFound("User not found");

            var cs = await _applicationDbContext.QuestionCommentSection
                .Include(csa => csa.Tages)
                .FirstOrDefaultAsync(csa => csa.Id == VM.Id && csa.Tages.Any(t => t.AddedById == user.Id));

            if (cs is null)
                return NotFound("Comment section not found");

            var tag = cs.Tages.FirstOrDefault(t => t.AddedById == user.Id);

            tag.LastSeen = DateTime.Now;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
