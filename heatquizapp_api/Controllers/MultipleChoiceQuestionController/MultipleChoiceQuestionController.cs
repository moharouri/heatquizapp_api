using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.DefaultQuestionImages;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.MultipleChoiceQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.MultipleChoiceQuestionController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class MultipleChoiceQuestionController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public MultipleChoiceQuestionController(
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
        public async Task<IActionResult> GetQuestion([FromBody] UniversalAccessByIdViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get question
            var Question = await _applicationDbContext.MultipleChoiceQuestions

                .Include(q => q.LevelOfDifficulty)

                .Include(q => q.Subtopic)
                .ThenInclude(st => st.Topic)

                .Include(q => q.AddedBy)
          
                .Include(q => q.Choices)

                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionSingleStep([FromForm] AddMultipleChoiceQuestionSingleStepViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get datapool
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == QuestionVM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check code not null
            if (string.IsNullOrEmpty(QuestionVM.Code))
                return BadRequest("Code can't be empty");

            //Check Image
            if (!QuestionVM.DefaultImageId.HasValue && QuestionVM.Picture is null)
                return BadRequest("Please provide a picture or choose a default image");

            //Verify extension && default image exist
            DefaultQuestionImage DefaultImage = null;

            if (QuestionVM.Picture != null)
            {
                var isExtenstionValid = validateImageExtension(QuestionVM.Picture);

                if (!isExtenstionValid)
                    return BadRequest("Picture extension not valid");

            }
            else
            {
                DefaultImage = await _applicationDbContext.DefaultQuestionImages
                              .FirstOrDefaultAsync(i => i.Id == QuestionVM.DefaultImageId && i.DataPoolId == DP.Id);

                if (DefaultImage is null)
                    return BadRequest("Default image not found");
            }

            //Check code not taken
            var CodeTaken = await _applicationDbContext.QuestionBase
                .AnyAsync(q => q.Code == QuestionVM.Code && q.DataPoolId == DP.Id);

            if (CodeTaken)
                return BadRequest("Code taken, choose different code");

            //Get Level Of Difficulty
            var LOD = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.Id == QuestionVM.LODId);

            if (LOD is null)
                return BadRequest("Level Of Difficulty not fFound");

            //Subtopic
            var Subtopic = await _applicationDbContext.Subtopics
                .FirstOrDefaultAsync(s => s.Id == QuestionVM.SubtopicId && s.DataPoolId == DP.Id);

            if (Subtopic is null)
                return BadRequest("Subtopic not found");

            //Get Answers
            var ParsedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MultipleChoiceQuestionChoiceViewModel>>(QuestionVM.AnswersString);

            if (ParsedModel is null)
                return BadRequest("Invalid answers");

            if (ParsedModel.Count == 0)
                return BadRequest("Please provide answers");

            if (ParsedModel.All(a => !a.Correct))
                return BadRequest("Please add atleast one correct answer");

            //Check MC Images 
            if (QuestionVM.MultipleChoiceImages.Count > ParsedModel.Count)
                return BadRequest("More images provided than multiple choices");

            if (QuestionVM.MultipleChoiceImages.Count < ParsedModel.Count(a => string.IsNullOrEmpty(a.Latex)))
                return BadRequest("Please provide an image for answers with no LaTex text");

            if (QuestionVM.MultipleChoiceImages.Count != ParsedModel.Count(a => !string.IsNullOrEmpty(a.ImageURL)))
                return BadRequest("Number of images does not match choices selected to have an image");

            //Get adder
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Question
            var Question = new MultipleChoiceQuestion()
            {
                Type = Constants.MUTLIPLE_CHOICE_QUESTION_PARAMETER,

                Code = QuestionVM.Code,

                AddedById = adder.Id,

                LevelOfDifficulty = LOD,
                SubtopicId = Subtopic.Id,

                ImageWidth = 0,
                ImageHeight = 0,

                Latex = QuestionVM.AnswerForLatex,

                DataPoolId = DP.Id
            };

            //Save Image
            var URL = "";

            //Picture
            if (QuestionVM.Picture != null)
            {
                URL = await SaveFile(QuestionVM.Picture);

                Question.ImageURL = URL;
                Question.ImageSize = QuestionVM.Picture.Length;
            }
            else
            {
                var pathToImage = DefaultImage.ImageURL;

                URL = await CopyFile(pathToImage);

                Question.ImageURL = URL;
                Question.ImageSize = DefaultImage.ImageSize;
            }

            //Add Answers
            var index = 0;
            foreach (var c in ParsedModel)
            {
                string choiceImageURL = null;

                //Save image if it exists
                if (!string.IsNullOrEmpty(c.ImageURL))
                {
                    var Picture = QuestionVM.MultipleChoiceImages[index];

                    if (Picture != null)
                    {
                        URL = await SaveFile(Picture);
                        choiceImageURL = URL;
                    }

                    index += 1;
                }

                Question.Choices.Add(new MultipleChoiceQuestionChoice()
                {
                    Latex = c.Latex,
                    Correct = c.Correct,
                    DataPoolId = DP.Id,
                    ImageURL = choiceImageURL
                });
            }

            //PDF
            if (QuestionVM.PDF != null)
            {
                //Verify Extension
                var isPDFExtensionValid = validatePDFExtension(QuestionVM.PDF);

                if (!isPDFExtensionValid)
                    return BadRequest("PDF extension not valid");

                //PDF
                var PDFURL = await SaveFile(QuestionVM.PDF);
                Question.PDFURL = PDFURL;
                Question.PDFSize = QuestionVM.PDF.Length;

            }

            _applicationDbContext.MultipleChoiceQuestions.Add(Question);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionChoice([FromForm] AddChoiceViewModel VM)
        {
            if(!ModelState.IsValid) 
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.MultipleChoiceQuestions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound($"Question not found");

            if ((VM.Picture is null) && string.IsNullOrEmpty(VM.Latex))
                return BadRequest("Please provide picture or LaTeX text");

            //Check latex unique
            if(VM.Latex != null)
            {
                if (Question.Choices.Any(a => a.Latex == VM.Latex))
                    return BadRequest("Repeated LaTeX code");
            }

            //Create new choice
            var newChoice = new MultipleChoiceQuestionChoice()
            {
                Correct = VM.Correct,
                DataPoolId = Question.DataPoolId
            };

            //Add image if available
            if (VM.Picture != null)
            {
                //Verify Extension
                var isExtenstionValid = validateImageExtension(VM.Picture);

                if (!isExtenstionValid)
                    return BadRequest("Picture extension not valid");

                //Save picture and get url
                var URL = await SaveFile(VM.Picture);

                newChoice.ImageURL = URL;
            }

            //Add LaTeX if available
            if (!string.IsNullOrEmpty(VM.Latex))
            {
                newChoice.Latex = VM.Latex;
            }

            //Add choice
            Question.Choices.Add(newChoice);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditQuestionChoice([FromForm] UpdateChoiceViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.MultipleChoiceQuestions
                .Include(c => c.Choices)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound($"Question not found");

            var Choice = Question.Choices.FirstOrDefault(c => c.Id == VM.AnswerId);

            if (Choice is null)
                return NotFound($"Choice not found");

            if (VM.Correct.HasValue)
            {
                if (Question.Choices.Where(c => c.Id != VM.AnswerId && c.Correct).Count() == 0 && !VM.Correct.Value)
                    return NotFound($"Atleast one choice must be correct");
            }

            if ((VM.Picture is null) && string.IsNullOrEmpty(VM.Latex) && !VM.Correct.HasValue)
                return BadRequest("Please provide values ");

            //Update image if provided
            if (VM.Picture != null)
            {
                //Verify Extension
                var isExtensionValid = validateImageExtension(VM.Picture);

                if (!isExtensionValid)
                    return BadRequest("Picture extension not valid");

                //Try removing image
                if (Choice.ImageURL != null)
                {
                    RemoveFile(Choice.ImageURL);
                }

                //Save picture and generate a url
                var URL = await SaveFile(VM.Picture);

                Choice.ImageURL = URL;
            }

            Choice.Latex = VM.Latex;
            Choice.Correct = VM.Correct.Value;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> RemoveQuestionChoiceLatex([FromForm] UpdateChoiceViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Question Exists
            var Question = await _applicationDbContext.MultipleChoiceQuestions
                .Include(c => c.Choices)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound($"Question not found");

            var Choice = Question.Choices.FirstOrDefault(c => c.Id == VM.AnswerId);

            if (Choice is null)
                return NotFound($"Choice not found");

            if (Choice.ImageURL is null)
                return NotFound("Please add an image before clearing LaTeX text");

            Choice.Latex = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> RemoveQuestionChoiceImage([FromForm] UpdateChoiceViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Question Exists
            var Question = await _applicationDbContext.MultipleChoiceQuestions
                .Include(c => c.Choices)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound($"Question not found");

            var Choice = Question.Choices.FirstOrDefault(c => c.Id == VM.AnswerId);

            if (Choice is null)
                return NotFound($"Choice not found");

            if (string.IsNullOrEmpty(Choice.Latex))
                return NotFound("Please add LaTeX text before deleting the image");

            //Try removing image
            if(Choice.ImageURL != null)
            {
                RemoveFile(Choice.ImageURL);
            }

            Choice.ImageURL = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> RemoveQuestionChoice([FromBody] UpdateChoiceViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Question Exists
            var Question = await _applicationDbContext.MultipleChoiceQuestions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == QuestionVM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Get choice
            var Choice = Question.Choices.FirstOrDefault(a => a.Id == QuestionVM.AnswerId);

            if (Choice is null)
                return NotFound("Question answer not found");

            //Try removing image
            if (Choice.ImageURL != null)
            {
                RemoveFile(Choice.ImageURL);
            }

            //Check choices consistency
            if (Question.Choices.Count == 1)
                return BadRequest("Cannot delete this answer because it is the only answer left");

            if (!Question.Choices.Any(c => c.Correct && c.Id != Choice.Id))
                return BadRequest("Cannot delete this answer because it is the only correct answer left");

            //Remove choice
            Question.Choices.Remove(Choice);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));

        }

    }
}
