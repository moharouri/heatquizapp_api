using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.ClickImageTrees;
using heatquizapp_api.Models.InterpretedTrees;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.SimpleClickableQuestionController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class SimpleClickableQuestionController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public SimpleClickableQuestionController(
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

            //Get Question
            var Question = await _applicationDbContext.SimpleClickableQuestions
                .Include(q => q.LevelOfDifficulty)
                .Include(q => q.Subtopic)
                .ThenInclude(s => s.Topic)

                .Include(q => q.ClickImages)
                .ThenInclude(ci => ci.Answer)

                .Include(q => q.ClickCharts)
                .ThenInclude(cc => cc.Answer)
                .ThenInclude(a => a.Jump)
                
                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == VM.Id);

            if (Question is null)
                return NotFound("Question not found");

           
            return Ok(
                new
                {
                    Question = _mapper.Map<SimpleClickableQuestion, SimpleClickableQuestionViewModel>(Question),

                    /*ImageAnswerGroups = _mapper.Map<List<ImageAnswerGroup>, List<ImageAnswerGroupViewModel>>
                    (ImageAnswerGroups),

                    InterpretedImageGroups = _mapper.Map<List<InterpretedImageGroup>, List<InterpretedImageGroupViewModel>>
                    (InterpretedImageGroups)*/
                });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionSingleStep([FromForm] AddClickQuestionSingleStepViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Code not Null
            if (string.IsNullOrEmpty(QuestionVM.Code))
                return BadRequest("Code can't be empty");

            //Check picture
            if (QuestionVM.Picture is null)
                return Ok("Please provide picture");

            //Verify extension
            var isExtenstionValid = validateImageExtension(QuestionVM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extension not valid");

            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == QuestionVM.DataPoolId);

            if (DP is null)
                return BadRequest("Datapool not found");

            //Check code not taken
            var codeTaken = await _applicationDbContext.QuestionBase
                .AnyAsync(q => q.Code == QuestionVM.Code && q.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code taken, choose different code");

            //Get Level Of Difficulty
            var LOD = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.Id == QuestionVM.LODId);

            if (LOD is null)
                return NotFound("Level of difficulty not found");

            //Subtopic
            var Subtopic = await _applicationDbContext.Subtopics
                .FirstOrDefaultAsync(s => s.Id == QuestionVM.SubtopicId && s.DataPoolId == DP.Id);

            if (Subtopic is null)
                return BadRequest("Subtopic not found");

            //Parse clickable parts
            var ParsedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseClickablePartsViewModel>(QuestionVM.ClickParts);

            if (ParsedModel is null)
                return BadRequest("Invalid data Of click parts");

            if (ParsedModel.ClickImages.Count == 0 && ParsedModel.ClickCharts.Count == 0)
                return BadRequest("Please provide clickable parts");

            //Images 
            var UsedImagesIds = ParsedModel.ClickImages.Select(ci => ci.AnswerId);

            var ClickableImages = (await _applicationDbContext.ImageAnswers
                .Where(i => i.DataPoolId == DP.Id && UsedImagesIds.Any(Id => Id == i.Id))
                .ToListAsync())
                .ToDictionary(ci => ci.Id);

            if (ClickableImages.Count != ParsedModel.ClickImages.GroupBy(ci => ci.AnswerId).Count())
                return NotFound("Atleast one image answer not found");

            //Charts
            var UsedChartsIds = ParsedModel.ClickCharts.Select(ci => ci.AnswerId);

            var InterpretedImages = (await _applicationDbContext.InterpretedImages
                .Where(ii => UsedChartsIds.Any(Id => Id == ii.Id) && ii.DataPoolId == DP.Id)
                .ToListAsync())
                .ToDictionary(ci => ci.Id);

            if (InterpretedImages.Count != ParsedModel.ClickCharts.GroupBy(cc => cc.AnswerId).Count())
                return NotFound("Atleast one interpreted image not found");

            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Question
            var Question = new SimpleClickableQuestion()
            {
                Code = QuestionVM.Code,

                Type = Constants.CLICKABLE_QUESTION_PARAMETER,

                LevelOfDifficultyId = LOD.Id,
                SubtopicId = Subtopic.Id,
               
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

          
            Question.ClickImages.AddRange(ParsedModel.ClickImages.Select(ci => new ClickImage()
            {
                X = ci.X,
                Y = ci.Y,

                Width = ci.Width,
                Height = ci.Height,
                AnswerId = ci.AnswerId,

                DataPoolId = DP.Id
            }));

            Question.ClickCharts.AddRange(ParsedModel.ClickCharts.Select(cc => new ClickChart()
            {
                X = cc.X,
                Y = cc.Y,

                Width = cc.Width,
                Height = cc.Height,
                AnswerId = cc.AnswerId,
               
                DataPoolId = DP.Id

            }));

            //Picture
            var URL = await SaveFile(QuestionVM.Picture);
            Question.ImageURL = URL;
            Question.ImageSize = QuestionVM.Picture.Length;

            Question.ImageWidth = QuestionVM.PictureWidth;
            Question.ImageHeight = QuestionVM.PictureHeight;


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

            _applicationDbContext.SimpleClickableQuestions.Add(Question);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> UpdateClickableImageAnswer([FromBody] RemoveUpdateClickablePartViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            if (VM.IsImage)
            {
                //Get image
                var Image = await _applicationDbContext.ClickImage
                    .FirstOrDefaultAsync(i => i.Id == VM.Id);

                if (Image is null)
                    return NotFound("Part not found");

                //Get answer
                var Answer = await _applicationDbContext.ImageAnswers
                           .FirstOrDefaultAsync(a => a.Id == VM.AnswerId);

                if (Answer is null)
                    return NotFound("Answer not found");

                //Check image and answer have same datapoolId
                if (Image.DataPoolId != Answer.DataPoolId)
                    return BadRequest("Datapool mismatch");

                Image.AnswerId = Answer.Id;
            }
            else
            {
                //Get image
                var Image = await _applicationDbContext.ClickChart
                   .FirstOrDefaultAsync(i => i.Id == VM.Id);

                if (Image is null)
                    return NotFound("Part not found");

                //Get answer
                var Answer = await _applicationDbContext.InterpretedImages
                           .FirstOrDefaultAsync(a => a.Id == VM.AnswerId);

                if (Answer is null)
                    return NotFound("Answer not found");

                //Check image and answer have same datapoolId
                if (Image.DataPoolId != Answer.DataPoolId)
                    return BadRequest("Datapool mismatch");

                Image.AnswerId = Answer.Id;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("[action]")]
        //Change type and name in vs code -- original RemoveClickable
        public async Task<IActionResult> RemoveClickablePart([FromBody] RemoveUpdateClickablePartViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            if (VM.IsImage)
            {
                //Get image
                var Image = await _applicationDbContext.ClickImage
                    .FirstOrDefaultAsync(i => i.Id == VM.Id);

                if (Image is null)
                    return NotFound("Part not found");

                _applicationDbContext.ClickImage.Remove(Image);
            }
            else
            {
                var Image = await _applicationDbContext.ClickChart
                   .FirstOrDefaultAsync(i => i.Id == VM.Id);

                if (Image is null)
                    return NotFound("Part not found");

                _applicationDbContext.ClickChart.Remove(Image);

            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddNewQuestionParts([FromForm] AddClickQuestionNewPartsSingleStepViewModel QuestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Question = await _applicationDbContext.SimpleClickableQuestions
                .FirstOrDefaultAsync(q => q.Id == QuestionVM.Id);

            if (Question is null)
                return NotFound("Question not found");

            //Parse clickable parts
            var ParsedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseClickablePartsViewModel>(QuestionVM.ClickParts);

            if (ParsedModel is null)
                return BadRequest("Invalid data Of clickable parts");

            if (ParsedModel.ClickImages.Count == 0 && ParsedModel.ClickCharts.Count == 0)
                return BadRequest("Please provide clickable parts");

            //Images 
            var UsedImagesIds = ParsedModel.ClickImages.Select(ci => ci.AnswerId);

            var ClickableImages = (await _applicationDbContext.ImageAnswers
                .Where(i => UsedImagesIds.Any(Id => Id == i.Id) && i.DataPoolId == Question.DataPoolId)
                .ToListAsync())
                .ToDictionary(ci => ci.Id);

            if (ClickableImages.Count != ParsedModel.ClickImages.GroupBy(ci => ci.AnswerId).Count())
                return NotFound("Atleast one image answer not found");

            //Charts 
            var UsedChartsIds = ParsedModel.ClickCharts.Select(ci => ci.AnswerId);

            var InterpretedImages = (await _applicationDbContext.InterpretedImages
                .Where(ii => UsedChartsIds.Any(Id => Id == ii.Id) && ii.DataPoolId == Question.DataPoolId)
                .ToListAsync()).ToDictionary(ci => ci.Id);

            if (InterpretedImages.Count != ParsedModel.ClickCharts.GroupBy(cc => cc.AnswerId).Count())
                return NotFound("Atleast one interpreted image not found");

            Question.ClickImages.AddRange(ParsedModel.ClickImages.Select(ci => new ClickImage()
            {
                X = ci.X,
                Y = ci.Y,

                Width = ci.Width,
                Height = ci.Height,
                AnswerId = ci.AnswerId,

                DataPoolId = Question.DataPoolId,
            }));

            Question.ClickCharts.AddRange(ParsedModel.ClickCharts.Select(cc => new ClickChart()
            {
                X = cc.X,
                Y = cc.Y,

                Width = cc.Width,
                Height = cc.Height,
                AnswerId = cc.AnswerId,

                DataPoolId = Question.DataPoolId,
            }));

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Populate later, edit type and name in vs code -- original: UpdateClickablePartBackgroundImage 
        public async Task<IActionResult> UpdateClickablePartBackgroundImage()
        {
            return Ok();
        }

    }
}
