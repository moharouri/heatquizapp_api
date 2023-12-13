using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Mapping;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using heatquizapp_api.Models.Questions.MultipleChoiceQuestion;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.SharedQuestionController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class SharedQuestionController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public SharedQuestionController(
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
        public async Task<IActionResult> GetQuestionSeriesMapRelations([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
         
            //Check question exists
            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

            var Maps = await _applicationDbContext.CourseMap
                .Include(m => m.Course)
                .Include(m => m.Elements)
                .ThenInclude(e => e.QuestionSeries)
                .ThenInclude(qs => qs.Elements)
                .Where(m => m.Elements.Any(el => el.QuestionSeriesId.HasValue && el.QuestionSeries.Elements.Any(e => e.QuestionId == VM.Id)))
                .ToListAsync();

            return Ok(Maps.Select(m => new
            {
                Id = m.Id,
                Title = m.Title,
                Course = m.Course.Name,
                CourseId = m.CourseId,
                ImageURL = MappingProfile.GetGeneralImageURL(m),
                Elements = _mapper.Map<List<CourseMapElement>, List<CourseMapElementViewModel>>
                (m.Elements.Where(el => el.QuestionSeriesId.HasValue && el.QuestionSeries.Elements.Any(e => e.QuestionId == VM.Id)).ToList())
            }));

        }

        

        [HttpPut("[action]")]
        public async Task<IActionResult> EditQuestionBaseInfo([FromBody] UpdateQuestionBasicInfoViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get datapool
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == QuestionVM.DatapoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check question exists
            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == QuestionVM.Id);

            if (Question is null)
                return NotFound("Question not found");

            //Check code unique
            var codeUsed = await _applicationDbContext.QuestionBase
                .AnyAsync(q => q.Id != Question.Id && q.Code.ToUpper() == QuestionVM.Code.ToUpper() && q.DataPoolId == DP.Id);

            if (codeUsed)
                return BadRequest("Code already in use");

            //Get Level Of Difficulty
            var LOD = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.Id == QuestionVM.LevelOfDifficultyId);

            if (LOD is null)
                return BadRequest("Level of difficulty not found");

            //Subtopic
            var Subtopic = await _applicationDbContext.Subtopics
                .FirstOrDefaultAsync(s => s.Id == QuestionVM.SubtopicId);

            if (Subtopic is null)
                return BadRequest("Subtopic not found");


            Question.Code = QuestionVM.Code;
            Question.LevelOfDifficultyId = LOD.Id;
            Question.SubtopicId = Subtopic.Id;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditQuestionLatex([FromBody] UpdateQuestionLatexViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == QuestionVM.Id);

            if (Question is null)
                return NotFound("Question not found");

            Question.Latex = QuestionVM.Latex;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionBase, QuestionBaseViewModel>(Question));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditQuestionImage([FromForm] UpdateQuestionImageViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Question Exists
            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Check picture
            if (VM.Picture is null)
                return Ok("Please provide picture");

            //Verify extension
            var isExtenstionValid = validateImageExtension(VM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extension not valid");

            //Remove image
            var removeImageResult = RemoveFile(Question.ImageURL);

            if (!removeImageResult)
                return BadRequest("Failed to remove the image");

            //Generate a Path for The Picture
            var URL = await SaveFile(VM.Picture);

            Question.ImageURL = URL;
            Question.ImageSize = VM.Picture.Length;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionBase, QuestionBaseViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CopyQuestion([FromBody] CopyQuestionViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get Question
            var Question = await _applicationDbContext.QuestionBase

                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Check code
            if (string.IsNullOrEmpty(VM.Code))
                return NotFound("Code cannot be empty");

            var codeExists = await _applicationDbContext.QuestionBase
                .AnyAsync(q => q.Code == VM.Code && q.DataPoolId == Question.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            var adder = await getCurrentUser(_contextAccessor, _userManager);

            if(Question.Type == Constants.CLICKABLE_QUESTION_PARAMETER)
            {
                var CQ = await _applicationDbContext.SimpleClickableQuestions
                            .Include(q => q.ClickCharts)
                            .Include(q => q.ClickImages)
                            .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

                var Copy = (SimpleClickableQuestion)_applicationDbContext.Entry(CQ).CurrentValues.ToObject();
                Copy.Id = 0;
                Copy.Code = VM.Code;
                Copy.AddedById = adder.Id;

                _applicationDbContext.SimpleClickableQuestions.Add(Copy);
            }
            else if (Question.Type == Constants.KEYBOARD_QUESTION_PARAMETER)
            {
                var KQ = await _applicationDbContext.KeyboardQuestion
                            .Include(q => q.Answers)
                            .ThenInclude(a => a.AnswerElements)
                            .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

                var Copy = (KeyboardQuestion)_applicationDbContext.Entry(KQ).CurrentValues.ToObject();
                Copy.Id = 0;
                Copy.Code = VM.Code;
                Copy.AddedById = adder.Id;

                _applicationDbContext.KeyboardQuestion.Add(Copy);
            }
            else if (Question.Type == Constants.MUTLIPLE_CHOICE_QUESTION_PARAMETER)
            {
                var MQ = await _applicationDbContext.MultipleChoiceQuestions
                            .Include(q => q.Choices)
                            .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

                var Copy = (MultipleChoiceQuestion)_applicationDbContext.Entry(MQ).CurrentValues.ToObject();
                Copy.Id = 0;
                Copy.Code = VM.Code;
                Copy.AddedById = adder.Id;

                _applicationDbContext.MultipleChoiceQuestions.Add(Copy);
            }
            else
            {
                return BadRequest("Unidentified question type");
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok("Question copied");
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> RemoveQuestionSolution([FromForm] RemoveQuestionSolutionViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);


            //Check Question Exists
            var Question = await _applicationDbContext.QuestionBase
                            .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Try delete pdf file
            if(Question.PDFURL != null)
            {
                var removalResult = RemoveFile(Question.PDFURL);

                if (!removalResult)
                    return BadRequest("Failed to remove file");
            }

            Question.PDFURL = null;
            Question.PDFSize = 0;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddEditQuestionPDF([FromForm] AddQuestionSolutionViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.QuestionBase
                            .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //PDF
            if (VM.PDF != null)
            {
                //Verify Extension
                var isPDFExtensionValid = validatePDFExtension(VM.PDF);

                if (!isPDFExtensionValid)
                    return BadRequest("PDF extension not valid");

                //Try removing existing file
                var removalResult = true;

                if (Question.PDFURL != null)
                {
                    removalResult = RemoveFile(Question.PDFURL);
                }

                //PDF
                var PDFURL = await SaveFile(VM.PDF);
                Question.PDFURL = PDFURL;
                Question.PDFSize = VM.PDF.Length;

                await _applicationDbContext.SaveChangesAsync();

                if (removalResult)
                {
                    return Ok("File updated");
                }

                return Ok("File updated but had problem deleting existing file");
            }
            else
            {
                return BadRequest("Please provide a solution file");
            }
        }

    }
}
