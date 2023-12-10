using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.DefaultQuestionImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;
using System.Reflection.Emit;

namespace heatquizapp_api.Controllers.DefaultQuestionImageController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DefaultQuestionImageController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public DefaultQuestionImageController(
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
        public async Task<IActionResult> GetAllImages([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var List = await _applicationDbContext.DefaultQuestionImages
                .Where(i => i.DataPoolId == VM.DatapoolId)
                .Include(m => m.AddedBy)
                .OrderBy(q => q.Code)
                .ToListAsync();

            return Ok(_mapper.Map<List<DefaultQuestionImage>, List<DefaultQuestionImageViewModel>>(List));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddImage([FromForm] AddDefaultQuestionImageViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeExists = await _applicationDbContext.DefaultQuestionImages
                .AnyAsync(i => i.Code == VM.Code && i.DataPoolId == VM.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            //Get adder
            var Adder = await getCurrentUser(_contextAccessor, _userManager);

            //Check Code Not Taken
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            var QI = new DefaultQuestionImage()
            {
                Code = VM.Code,
                AddedById = Adder.Id,

                DataPoolId = DP.Id
            };

            //Picture
            if (VM.Picture == null)
                return BadRequest("Please provide an image");

            //Validate extension
            var isExtenstionValid = validateImageExtension(VM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extension not valid");

            //Save image and generate url
            var URL = await SaveFile(VM.Picture);

            QI.ImageURL = URL;
            QI.ImageSize = VM.Picture.Length;

            _applicationDbContext.DefaultQuestionImages.Add(QI);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditCode([FromForm] UpdateDefaultImageCodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var qi = await _applicationDbContext.DefaultQuestionImages
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (qi is null)
                return BadRequest("Image not found");

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeExists = await _applicationDbContext.DefaultQuestionImages
                .AnyAsync(i => i.Code == VM.Code && i.Id != qi.Id && i.DataPoolId != qi.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            qi.Code = VM.Code;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditImage([FromForm] UpdateDefaultImageCodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);


            var QI = await _applicationDbContext.DefaultQuestionImages
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (QI is null)
                return BadRequest("Image not found");

            //Picture
            if (VM.Picture == null)
                return BadRequest("Please provide an image");

            //Validate extension
            var isExtenstionValid = validateImageExtension(VM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extension not valid");

            //Save image and generate url
            var URL = await SaveFile(VM.Picture);

            QI.ImageURL = URL;
            QI.ImageSize = VM.Picture.Length;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
