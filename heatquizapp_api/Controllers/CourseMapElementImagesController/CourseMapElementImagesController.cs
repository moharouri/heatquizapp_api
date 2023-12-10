using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.CourseMapElementImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;
using System;
using heatquizapp_api.Models.Courses;

namespace heatquizapp_api.Controllers.CourseMapElementImagesController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseMapElementImagesController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public CourseMapElementImagesController(
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

            var BIs = await _applicationDbContext.CourseMapElementImages
                .Include(m => m.AddedBy)
                .OrderBy(q => q.Code)
                .Where(i => i.DataPoolId == VM.DatapoolId)
                .ToListAsync();

            return Ok(_mapper.Map<List<CourseMapElementImages>, List<CourseMapElementImagesViewModel>>(BIs));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddImage([FromForm] AddMapElementImagesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            //Check datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check code already exists
            var codeExists = await _applicationDbContext.CourseMapElementImages
                .AnyAsync(i => i.Code == VM.Code && i.DataPoolId == VM.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            //Get adder
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            var cmpei = new CourseMapElementImages()
            {
                Code = VM.Code,
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

            //Check valid extension
            var ImagesList = new List<IFormFile>
            {
                VM.Play,
                VM.PDF,
                VM.Video,
                VM.Link
            };

            var validExtenstions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

            if (!ImagesList.Any() || ImagesList.Any(i => i != null && !validExtenstions.Any(ve => i.FileName.EndsWith(ve))))
                return BadRequest("Please provide proper image files");

            if (VM.Play != null)
            {
                //Save picture and generate url
                var URL = await SaveFile(VM.Play);

                cmpei.Play = URL;
            }

            if (VM.PDF != null)
            {
                //Save picture and generate url
                var URL = await SaveFile(VM.PDF);

                cmpei.PDF = URL;
            }

            if (VM.Video != null)
            {
                //Save picture and generate url
                var URL = await SaveFile(VM.Video);

                cmpei.Video = URL;
            }

            if (VM.Link != null)
            {
                //Save picture and generate url
                var URL = await SaveFile(VM.Link);

                cmpei.Link = URL;
            }

            _applicationDbContext.CourseMapElementImages.Add(cmpei);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditCode([FromForm] UpdateMapElementImagesCodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var bi = await _applicationDbContext.CourseMapElementImages
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (bi is null)
                return BadRequest("Data not found");

            var codeExists = await _applicationDbContext.CourseMapElementImages
                .AnyAsync(i => i.Code == VM.Code && i.Id != VM.Id && i.DataPoolId == bi.DataPoolId);

            if (codeExists)
                return BadRequest("Code already exists");

            bi.Code = VM.Code;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditImage([FromForm] UpdateMapElementImagesImageViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var bi = await _applicationDbContext.CourseMapElementImages
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (bi is null)
                return BadRequest("List not found");

            //Picture
            if (VM.Picture is null)
                return BadRequest("Please provide image");

            //Verify Extension
            var isExtensionValid = validateImageExtension(VM.Picture);

            if (!isExtensionValid)
                return BadRequest("Image file extenstion is not valid");
            
            //Save image and generate url
            var URL = await SaveFile(VM.Picture);

            switch (VM.EditType)
            {
                case EDIT_TYPE.PLAY:
                    {
                        bi.Play = URL;

                        break;
                    }

                case EDIT_TYPE.PDF:
                    {
                        bi.PDF = URL;

                        break;
                    }

                case EDIT_TYPE.VIDEO:
                    {
                        bi.Video = URL;

                        break;
                    }

                case EDIT_TYPE.LINK:
                    {
                        bi.Link = URL;

                        break;
                    }

                default:
                    {
                        return BadRequest("Edit type not correct");
                    }
            }


            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> SelectImageGroup([FromForm] AssignListToMapElementsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get list
            var List = await _applicationDbContext.CourseMapElementImages
                .FirstOrDefaultAsync(l => l.Id == VM.ListId);

            if (List is null)
                return NotFound("List not found");

            //Get elements
            if (!VM.ElementIds.Any())
                return NotFound("Please provide elements");

            var MapElements = await _applicationDbContext.CourseMapElement
                .Where(e => VM.ElementIds.Any(Id => Id == e.Id))
                .Include(e => e.Map)
                .ToListAsync();

            if (MapElements.Count != VM.ElementIds.Distinct().Count())
                return NotFound("Some map elements not found");

            //Update
            foreach (var e in MapElements)
            {
                e.CourseMapElementImagesId = List.Id;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapViewModel>(MapElements.FirstOrDefault().Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code and request method !!!!
        public async Task<IActionResult> DeassignImagesList([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var MapElement = await _applicationDbContext.CourseMapElement
                .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (MapElement is null)
                return NotFound("Map element not found");

            MapElement.CourseMapElementImagesId = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapElementViewModel>(MapElement));
        }

    }
}
