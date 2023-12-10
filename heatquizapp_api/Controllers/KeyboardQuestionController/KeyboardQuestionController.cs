using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using System.Xml;
using heatquizapp_api.Models.DefaultQuestionImages;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.KeyboardQuestionController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KeyboardQuestionController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public KeyboardQuestionController(
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
        //Change name and type in vscode -- original: GetQuestion_Portal
        public async Task<IActionResult> GetQuestion([FromBody] QuestionBaseViewModel VM)
        {
            var Question = await _applicationDbContext.KeyboardQuestion

                .Include(q => q.LevelOfDifficulty)

                .Include(q => q.Subtopic)
                .ThenInclude(st => st.Topic)

                .Include(q => q.AddedBy)

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

                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

            foreach (var a in Question.Answers)
            {
                a.AnswerElements = a.AnswerElements.OrderBy(e => e.Id).ToList();
            }

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionSingleStep([FromForm] AddKeyboardQuestionSingleStepViewModel QuestionVM)
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
                var isExtensionValid = validateImageExtension(QuestionVM.Picture);

                if (!isExtensionValid)
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

            //Keyboard
            var Keyboard = await _applicationDbContext.Keyboards
                .Include(k => k.NumericKeys)
                .Include(k => k.VariableKeyImages)
                .FirstOrDefaultAsync(k => k.Id == QuestionVM.KeyboardId && k.DataPoolId == DP.Id);

            if (Keyboard is null)
                return BadRequest("Keyboard not found");

            var ParsedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseKeyboardQuestionAnswersViewModel>(QuestionVM.AnswersString);

            if (ParsedModel is null)
                return BadRequest("Invalid answers data");

            if (ParsedModel.Answers.Count == 0)
                return BadRequest("Please provide answers");

            
            //Adder
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Question
            var Question = new KeyboardQuestion()
            {
                Code = QuestionVM.Code,

                Type = Constants.KEYBOARD_QUESTION_PARAMETER,

                LevelOfDifficultyId = LOD.Id,

                SubtopicId = Subtopic.Id,

                IsEnergyBalance = QuestionVM.IsEnergyBalance,
                DisableDivision = QuestionVM.DisableDivision,

                Latex = QuestionVM.AnswerForLatex,


                KeyboardId = Keyboard.Id,
                AddedById = adder.Id,

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
                pathToImage = Path.Combine("wwwroot", pathToImage);

                URL = await CopyFile(pathToImage);

                Question.ImageURL = URL;
                Question.ImageSize = DefaultImage.ImageSize;
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

             _applicationDbContext.KeyboardQuestion.Add(Question);
            await _applicationDbContext.SaveChangesAsync();

            //Add answers
            foreach (var a in ParsedModel.Answers)
            {
                var answer = new KeyboardQuestionAnswer()
                {
                    DataPoolId = DP.Id
                };

                Question.Answers.Add(answer);
                await _applicationDbContext.SaveChangesAsync();

                foreach (var e in a.Answer.OrderBy(e => e.Order))
                {
                    var element = new KeyboardQuestionAnswerElement()
                    {
                        ImageId = e.VariationId.HasValue ? new Nullable<int>(Keyboard.VariableKeyImages.FirstOrDefault(r => r.VariationId == e.VariationId).Id) : null,

                        Value = e.Value,

                        NumericKeyId = e.NumericKeyId.HasValue ? new Nullable<int>(Keyboard.NumericKeys.FirstOrDefault(r => r.NumericKeyId == e.NumericKeyId).Id) : null,
                        
                        DataPoolId = DP.Id

                    };

                    answer.AnswerElements.Add(element);

                    await _applicationDbContext.SaveChangesAsync();
                }

            }

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionAnswer([FromBody] AddEditKeyboardQuestionAnswerViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get question
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

                .FirstOrDefaultAsync(q => q.Id == QuestionVM.Id);

            if (Question is null)
                return NotFound("Question not found");

            //Check Answer Has Elements
            if (QuestionVM.Answer.Count == 0)
                return BadRequest("Answer can't be empty");

            if (QuestionVM.Answer.All(a => a.VariationId is null && a.NumericKeyId is null))
                return BadRequest("All answers should include numeric/variable Values");

            //Check Answer Elements Exist in Keyboard
            var ImageIds = QuestionVM.Answer.Where((ae) => ae.VariationId.HasValue)
                .Select((ae) => ae.VariationId).ToList();

            var NumericIds = QuestionVM.Answer.Where((ae) => ae.NumericKeyId.HasValue)
                .Select((ae) => ae.NumericKeyId).ToList();

            if (ImageIds.Any(id => Question.Keyboard.VariableKeyImages.All(i => i.VariationId != id)))
                return BadRequest("Some variable keys used do not exist in keyboard");

            if (NumericIds.Any(id => Question.Keyboard.NumericKeys.All(i => i.NumericKeyId != id)))
                return BadRequest("Some numeric keys used do not exist in keyboard");

            //Add answer
            var Answer = new KeyboardQuestionAnswer()
            {

            };

            Question.Answers.Add(Answer);

            foreach (var e in QuestionVM.Answer.OrderBy(a => a.Order))
            {
                var AddEelemnt = new KeyboardQuestionAnswerElement()
                {
                    ImageId = e.VariationId.HasValue ? new Nullable<int>(Question.Keyboard.VariableKeyImages.FirstOrDefault(r => r.VariationId == e.VariationId).Id) : null,

                    Value = e.Value,

                    NumericKeyId = e.NumericKeyId.HasValue ?
                    new Nullable<int>(Question.Keyboard.NumericKeys.FirstOrDefault(r => r.NumericKeyId == e.NumericKeyId).Id) : null

                };

                Answer.AnswerElements.Add(AddEelemnt);
                await _applicationDbContext.SaveChangesAsync();
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));
        }

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> RemoveQuestionAnswer([FromBody] RemoveKeyboardQuestionAnswerViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.KeyboardQuestion
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == QuestionVM.Id);

            if (Question is null)
                return NotFound("Question not found");

            //Get answer
            var Answer = Question.Answers.FirstOrDefault(a => a.Id == QuestionVM.AnswerId);

            if (Answer is null)
                return NotFound("Question answer not found");

            if (Question.Answers.Count == 1)
                return BadRequest("Cannot delete answer because it is the only answer left");

            _applicationDbContext.KeyboardQuestionAnswer.Remove(Answer);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));

        }

        [HttpPost("[action]")]
        //Change type in vs code change name -- original: GetKeyboardQuestionWrongAnswers_PORTAL
        public async Task<IActionResult> GetKeyboardQuestionWrongAnswers([FromBody] KeyboardQuestionViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get question
            var Question = await _applicationDbContext.KeyboardQuestion
                .Include(q => q.WrongAnswers)
                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

            var Data = Question.WrongAnswers
                .GroupBy((a) => a.AnswerLatex)
                .Select(g =>
                new {
                    Latex = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending((e) => e.Count)
                .ToList();

            return Ok(Data);
        }


    }
}
