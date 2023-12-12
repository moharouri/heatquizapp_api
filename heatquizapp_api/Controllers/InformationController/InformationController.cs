using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.QuestionInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HeatQuizAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using heatquizapp_api.Models.Questions;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.InformationController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class InformationController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public InformationController(
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
        public async Task<IActionResult> GetAllInformation([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Information = await _applicationDbContext.Information
                .Where(k => k.DataPoolId == VM.DatapoolId)
                .Include(m => m.AddedBy)
                .OrderBy(q => q.Code)
                .ToListAsync();

            return Ok(_mapper.Map<List<InformationViewModel>>(Information));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchInformationQuestions([FromBody] InformationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check it exists
            var Information = await _applicationDbContext.Information
               .FirstOrDefaultAsync(info => info.Id == VM.Id);

            if (Information is null)
                return NotFound("Explanation not found");

            //Get questions
            var Qs = await _applicationDbContext.QuestionBase
                 .Where(m => m.InformationId == VM.Id)
                 .Include(q => q.LevelOfDifficulty)
                 .Include(q => q.Subtopic)
                 .ThenInclude(s => s.Topic)
                 .OrderBy(q => q.Code)
                 .ToListAsync();

            return Ok(_mapper.Map<List<QuestionBaseViewModel>>(Qs));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddInfo([FromForm] AddExplanationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeExists = await _applicationDbContext.Information
                .AnyAsync(i => i.Code == VM.Code && i.DataPoolId == DP.Id);

            if (codeExists)
                return BadRequest("Code already exists");

            if (string.IsNullOrEmpty(VM.Latex) && VM.PDF is null)
                return BadRequest("Please provide a latex or pdf file");

            //Get adder
            var Adder = await getCurrentUser(_contextAccessor, _userManager);

            var information = new Information()
            {
                Code = VM.Code,
                Latex = VM.Latex,
                AddedById = Adder.Id,
                DataPoolId = DP.Id
            };

            //PDF
            if (VM.PDF != null)
            {
                //Verify Extension
                var isPDFExtensionValid = validatePDFExtension(VM.PDF);

                if (!isPDFExtensionValid)
                    return BadRequest("PDF extension not valid");

                //PDF
                var PDFURL = await SaveFile(VM.PDF);
                information.PDFURL = PDFURL;
                information.PDFSize = VM.PDF.Length;

            }

            _applicationDbContext.Information.Add(information);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<InformationViewModel>(information));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditCode([FromForm] UpdateExplanationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            //Check it exists
            var Info = await _applicationDbContext.Information
               .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Info is null)
                return BadRequest("Data not found");

            //Check code is used
            var codeExists = await _applicationDbContext.Information
                .AnyAsync(i => i.Code == VM.Code && i.Id != Info.Id && i.DataPoolId == Info.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            Info.Code = VM.Code;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditLatex([FromForm] UpdateExplanationViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check it exists
            var Info = await _applicationDbContext.Information
               .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Info is null)
                return BadRequest("Data not found");

            if (string.IsNullOrEmpty(Info.PDFURL) && string.IsNullOrEmpty(VM.Latex))
                return BadRequest("Information should have PDF or Latex text atleast");

            Info.Latex = VM.Latex;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditPDF([FromForm] UpdateExplanationViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check it exists
            var Info = await _applicationDbContext.Information
               .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Info is null)
                return BadRequest("Data not found");
            
            //Remove the already existing file if it exists
            if(Info.PDFURL != null)
            {
                RemoveFile(Info.PDFURL);
            }

            //PDF
            if (VM.PDF is null)
                return BadRequest("Please provide a PDF file");

            var isPDFExtensionValid = validatePDFExtension(VM.PDF);

            if (!isPDFExtensionValid)
                return BadRequest("PDF extension not valid");

            //Save and generate a url
            var PDFURL = await SaveFile(VM.PDF);
            Info.PDFURL = PDFURL;
            Info.PDFSize = VM.PDF.Length;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> RemovePDF([FromForm] UpdateExplanationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check it exists
            var Info = await _applicationDbContext.Information
               .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Info is null)
                return BadRequest("Data not found");
           
            if (string.IsNullOrEmpty(Info.Latex))
                return BadRequest("Information should have PDF or Latex text atleast");

            //Remove the already existing file if it exists
            if (Info.PDFURL != null)
            {
                RemoveFile(Info.PDFURL);
            }

            Info.PDFURL = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AssignQuestions([FromForm] AssignQuestionsToExplanationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check it exists
            var Info = await _applicationDbContext.Information
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Info is null)
                return BadRequest("Data not found");

            var Questions = await _applicationDbContext.QuestionBase
                .Where(q => VM.QuestionIds.Any(id => q.Id == id))
                .ToListAsync();

            if (Questions.Count == 0)
                return BadRequest("Please provide questions to assign");

            if (Questions.Count != VM.QuestionIds.Distinct().Count())
                return BadRequest("Some questions not found");

            //Assign
            foreach (var q in Questions)
            {
                q.InformationId = Info.Id;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DeassignQuestions([FromForm] DeassignQuestionsToExplanationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get questions
            var Questions = await _applicationDbContext.QuestionBase
                .Where(q => VM.QuestionIds.Any(id => q.Id == id))
                .ToListAsync();

            if (Questions.Count == 0)
                return BadRequest("Please provide questions to unassign");

            if (Questions.Count != VM.QuestionIds.Distinct().Count())
                return BadRequest("Some questions not found");

            //Deassign
            foreach (var q in Questions)
            {
                q.InformationId = null;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }



    }
}
