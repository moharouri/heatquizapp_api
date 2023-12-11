using HeatQuizAPI.Database;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using heatquizapp_api.Models.ClickImageTrees;
using heatquizapp_api.Models.InterpretedTrees;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;
using heatquizapp_api.Models.Questions.MultipleChoiceQuestion;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using HeatQuizAPI.Utilities;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using static heatquizapp_api.Utilities.Utilities;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Series;

namespace heatquizapp_api.Controllers.StudentsController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public StudentsController(
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

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetClickableQuestion(int Id)
        {
            //Get Question
            var Question = await _applicationDbContext.SimpleClickableQuestions

                .Include(q => q.ClickImages)
                .ThenInclude(ci => ci.Answer)

                .Include(q => q.ClickCharts)
                .ThenInclude(cc => cc.Answer)
                .ThenInclude(a => a.Jump)

                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");

            //Get images groups 
            var ImageAnswerGroups = await _applicationDbContext.ImageAnswerGroups
                .Where(g => Question.ClickImages.Any(ci => ci.Answer.GroupId == g.Id))
                .Include(g => g.Images)
                .ToListAsync();

            foreach (var Group in ImageAnswerGroups)
            {
                //Get roots including leafs that include thier leafs ...... -- tree
                var Images = Group.Images
                    .Where(i => !i.RootId.HasValue)
                    .ToList();

                Group.Images = Images;
            }

            var InterpretedImageGroups = await _applicationDbContext.InterpretedImageGroups
                .Where(g => Question.ClickCharts.Any(cc => cc.Answer.GroupId == g.Id))
                .Include(g => g.Images)
                .ToListAsync();

            return Ok(
                new
                {
                    Question = _mapper.Map<SimpleClickableQuestion, SimpleClickableQuestionViewModel>(Question),

                    ImageAnswerGroups = _mapper.Map<List<ImageAnswerGroup>, List<ImageAnswerGroupViewModel>>
                    (ImageAnswerGroups),

                    InterpretedImageGroups = _mapper.Map<List<InterpretedImageGroup>, List<InterpretedImageGroupViewModel>>
                    (InterpretedImageGroups)
                });
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetKeyboardQuestion(int Id)
        {
            var Question = await _applicationDbContext.KeyboardQuestion

                .Include(q => q.Keyboard)
                .ThenInclude(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)

                .Include(q => q.Keyboard)
                .ThenInclude(k => k.VariableKeyImages)
                .ThenInclude(vk => vk.Variation)

                .Include(q => q.Answers)
                .ThenInclude(a => a.AnswerElements)
                .ThenInclude(e => e.NumericKey)
                .ThenInclude(k => k.NumericKey)

                .Include(q => q.Answers)
                .ThenInclude(a => a.AnswerElements)
                .ThenInclude(e => e.Image)
                .ThenInclude(e => e.Variation)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");

            foreach (var a in Question.Answers)
            {
                a.AnswerElements = a.AnswerElements.OrderBy(e => e.Id).ToList();
            }

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetMultipleChoiceQuestion(int Id)
        {
            //Get question
            var Question = await _applicationDbContext.MultipleChoiceQuestions

                .Include(q => q.Choices)

                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpGet("[action]/{Code}")]
        public async Task<IActionResult> GetSeriesPlayByCode(string Code)
        {
            var Series = await _applicationDbContext.QuestionSeries

                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.Information)

                .FirstOrDefaultAsync(s => s.Code == Code);

            if (Series is null)
                return NotFound("Series not found");

            //Send elements in order
            Series.Elements = Series.Elements.OrderBy(e => e.Order).ToList();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetCourseMapPlayById(int Id)
        {
            var Course = await _applicationDbContext.CourseMap

                .Include(c => c.Elements)
                .ThenInclude(e => e.QuestionSeries)
                
                .Include(c => c.Elements)
                .ThenInclude(e => e.RequiredElement)

                .Include(c => c.Elements)
                .ThenInclude(e => e.Badges)

                 .Include(c => c.Elements)
                .ThenInclude(e => e.CourseMapElementImages)

                .Include(c => c.Elements)
                .ThenInclude(e => e.MapAttachment)
                .ThenInclude(a => a.Map)
                .FirstOrDefaultAsync(c => c.Id == Id && !c.Disabled);

            if (Course is null)
                return NotFound("Map not found");

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Course));
        }

        [HttpPost("[action]")]
        //Change name and position controller-wise -- original: PostStatistic
        public async Task<IActionResult> AddQuestionStatistic([FromForm] AddQuestionStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Add statistic
            Question.QuestionStatistics.Add(new QuestionStatistic()
            {
                Correct = VM.Correct,
                Score = VM.Score,
                TotalTime = VM.TotalTime,
                Key = VM.Key,
                Player = VM.Player,
                DataPoolId = Question.DataPoolId
            });

             
            //Try adding linked key if the user is registed 
            //Check if the user wants to have his statistics posted
            try
            {
                var registered_user = await getCurrentUser(_contextAccessor, _userManager);
                var player_key = VM.Player;

                var key_exists = await _applicationDbContext.UserLinkedPlayerKeys
                    .AnyAsync(k => k.UserId == registered_user.Id && k.PlayerKey == player_key);

                if (!key_exists)
                {
                    _applicationDbContext.UserLinkedPlayerKeys.Add(new UserLinkedPlayerKey()
                    {
                        PlayerKey = player_key,
                        User = registered_user,
                        DateCreated = DateTime.Now
                    });
                }
            }
            catch
            {

            }

            if (Question.Type == Constants.KEYBOARD_QUESTION_PARAMETER && !VM.Correct)
            {
                var KQuestion = await _applicationDbContext.KeyboardQuestion
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

                if(KQuestion != null)
                {
                    KQuestion.WrongAnswers.Add(new KeyboardQuestionWrongAnswer()
                    {
                        AnswerLatex = VM.Latex,

                        DataPoolId = Question.DataPoolId
                    });
                }
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        //Change position controller-wise 
        public async Task<IActionResult> AddQuestionPDFStatistic([FromForm] AddQuestionPDFStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (question is null)
                return NotFound("Question not found");

            //Add statistc
            question.QuestionPDFStatistics.Add(new QuestionPDFStatistic()
            {
                Player = VM.Player,
                Correct = VM.Correct,

            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        //Change position controller-wise
        public async Task<IActionResult> AddStatistic([FromForm] AddSeriesStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
               .FirstOrDefaultAsync(q => q.Id == VM.SeriesId);

            if (Series is null)
                return NotFound("Series not found");

            //Add statistic
            Series.Statistics.Add(new QuestionSeriesStatistic()
            {
                Player = VM.Player,
                MapKey = VM.MapKey,
                MapName = VM.MapName,
                MapElementName = VM.MapElementName,
                SuccessRate = VM.SuccessRate,

                TotalTime = VM.TotalTime,
                DataPoolId = Series.DataPoolId,
                OnMobile = VM.OnMobile,
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        //Change name and position controller-wise -- original: AddPDFStatistic
        public async Task<IActionResult> AddMapPDFStatistic([FromForm] AddMapPDFStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var element = await _applicationDbContext.CourseMapElement
                .FirstOrDefaultAsync(q => q.Id == VM.ElementId);

            if (element is null)
                return NotFound("Element not found");

            element.PDFStatistics.Add(new CourseMapPDFStatistics()
            {
                Player = VM.Player,
                OnMobile = VM.OnMobile
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
