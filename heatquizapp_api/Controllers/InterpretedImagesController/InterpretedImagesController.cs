using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.InterpretedTrees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace heatquizapp_api.Controllers.InterpretedImagesController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class InterpretedImagesController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public InterpretedImagesController(
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
        [AllowAnonymous]
        public async Task<IActionResult> GetInterpretedTrees([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(d => d.Id == VM.DatapoolId);

            if (DP is null)
                return NotFound("Datapool does not exist");

            var Groups = await _applicationDbContext.InterpretedImageGroups

                .Include(g => g.Images)
                .ThenInclude(i => i.Left)

                .Include(g => g.Images)
                .ThenInclude(i => i.RationOfGradients)

                .Include(g => g.Images)
                .ThenInclude(i => i.Right)

                .Include(g => g.Images)
                .ThenInclude(i => i.Jump)

                .Include(g => g.Images)
                .ThenInclude(i => i.ClickCharts)

                .OrderBy(g => g.Name)
                .Where(g => g.DataPoolId == VM.DatapoolId)
                .ToListAsync();

            return Ok(_mapper.Map<List<InterpretedImageGroup>, List<InterpretedImageGroupViewModel>>(Groups));
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetValues()
        {
            var Left = await _applicationDbContext.LeftGradientValues
                .ToListAsync();

            var Right = await _applicationDbContext.RightGradientValues
                .ToListAsync();

            var RatioOfGradients = await _applicationDbContext.RationOfGradientsValues
                .ToListAsync();

            var Jump = await _applicationDbContext.JumpValues
                .ToListAsync();

            return Ok(new
            {
                Left = _mapper.Map<List<LeftGradientValue>,
                                   List<InterpretationValueViewModel>>
                                   (Left),

                Right = _mapper.Map<List<RightGradientValue>,
                                    List<InterpretationValueViewModel>>
                                    (Right),

                RatioOfGradients = _mapper.Map<List<RationOfGradientsValue>,
                                               List<InterpretationValueViewModel>>
                                               (RatioOfGradients),

                Jump = _mapper.Map<List<JumpValue>,
                                   List<InterpretationValueViewModel>>
                                   (Jump)
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddTree([FromBody] AddInterpretedImageGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check name not taken
            var NameTaken = await _applicationDbContext.InterpretedImageGroups
                .AnyAsync(g => g.Name == VM.Name && g.Id == DP.Id);

            if (NameTaken)
                return BadRequest("Name taken, choose different name");
           
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Add tree
            var Tree = new InterpretedImageGroup()
            {
                Name = VM.Name,
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

            _applicationDbContext.InterpretedImageGroups.Add(Tree);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<InterpretedImageGroup, InterpretedImageGroupViewModel>(Tree));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditTree([FromBody] UpdateInterpretedImageGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check tree exists
            var Tree = await _applicationDbContext.InterpretedImageGroups
                .FirstOrDefaultAsync(g => g.Id == VM.Id);

            if (Tree is null)
                return NotFound("Tree not found");

            //Check Name Not Null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check name not taken
            var nameTaken = await _applicationDbContext.InterpretedImageGroups
                .AnyAsync(g => g.Id != VM.Id && g.Name == VM.Name);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Update
            Tree.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<InterpretedImageGroup, InterpretedImageGroupViewModel>(Tree));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteTree([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Group
            var Tree = await _applicationDbContext.InterpretedImageGroups
               .Include(g => g.Images)
               .ThenInclude(i => i.ClickCharts)
               .FirstOrDefaultAsync(g => g.Id == VM.Id);

            if (Tree is null)
                return NotFound($"Tree not found");

            if (Tree.Images.Any(i => i.ClickCharts.Count != 0))
                return BadRequest("Tree used in questions, can't be deleted");

            //Remove
            foreach (var i in Tree.Images)
            {
                //Try removing image
                if (i.ImageURL != null)
                {
                    RemoveFile(i.ImageURL);
                }

                _applicationDbContext.InterpretedImages.Remove(i);
            }

            _applicationDbContext.InterpretedImageGroups.Remove(Tree);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddImageSingleStep([FromForm] AddInterpretedNodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check tree exists
            var Tree = await _applicationDbContext.InterpretedImageGroups
                .Include(g => g.Images)

                .FirstOrDefaultAsync(g => g.Id == VM.GroupId);

            if (Tree is null)
                return NotFound($"Tree not found");

            //Check Code Not Null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code can't be null");

            //Check code not taken
            var codeTaken = Tree.Images
                .Any(i => i.Code == VM.Code);

            if (codeTaken)
                return BadRequest("Code is already taken within the tree");

            //Check interpretation values exist
            var Left = await _applicationDbContext.LeftGradientValues
                .FirstOrDefaultAsync(l => l.Id == VM.LeftId);

            if (Left is null)
                return BadRequest("Interpretation value not found");

            var Right = await _applicationDbContext.RightGradientValues
                .FirstOrDefaultAsync(l => l.Id == VM.RightId);

            if (Right is null)
                return BadRequest("Interpretation value not found");


            var Ratio = await _applicationDbContext.RationOfGradientsValues
                .FirstOrDefaultAsync(l => l.Id == VM.RatioId);

            if (Ratio is null)
                return BadRequest("Interpretation value not found");


            var Jump = await _applicationDbContext.JumpValues
                .FirstOrDefaultAsync(l => l.Id == VM.JumpId);

            if (Jump is null)
                return BadRequest("Interpretation value not found");

            //Check picture
            if (VM.Picture is null)
                return BadRequest("Please provide picture");

            //Verify Extension
            var isExtensionValid = validateImageExtension(VM.Picture);

            if (!isExtensionValid)
                return BadRequest("Picture extenstion not valid");
            

            //Check if Image Already Exists [Values]
            if (Tree.Images.Any(i => i.JumpId == VM.JumpId && i.LeftId == VM.LeftId && i.RightId == VM.RightId && i.RationOfGradientsId == VM.RatioId))
                return BadRequest("Values already used within the tree");

            //Create Image
            var Image = new InterpretedImage()
            {
                Code = VM.Code,
                RightId = Right.Id,
                LeftId = Left.Id,
                RationOfGradientsId = Ratio.Id,
                JumpId = Jump.Id,
                GroupId = Tree.Id,
                DataPoolId = Tree.DataPoolId
            };

            //save picture and generate url
            var URL = await SaveFile(VM.Picture);

            Image.ImageURL = URL;
            Image.Size = VM.Picture.Length;

            //Add
            _applicationDbContext.InterpretedImages.Add(Image);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteNode([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Image
            var Image = await _applicationDbContext.InterpretedImages
                .Include(i => i.ClickCharts)
               .FirstOrDefaultAsync(g => g.Id == VM.Id);

            if (Image is null)
                return NotFound($"Group Not Found");

            if (Image.ClickCharts.Count != 0)
              return BadRequest("Image Used in Questions, Can't Be Deleted");

            //Try removing image
            if(Image.ImageURL != null)
            {
                RemoveFile(Image.ImageURL); 
            }

            //Remove 
            _applicationDbContext.InterpretedImages.Remove(Image);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditInterpretedImageName([FromForm] UpdateInterpretedNodeViewModel VM)
        {
            //Check image exists
            var Image = await _applicationDbContext.InterpretedImages
                .Include(i => i.Group)
                .ThenInclude(g => g.Images)

                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Image is null)
                return NotFound("Node not found");

            //Check code 
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code can't be empty");

            var Tree = Image.Group;

            //Check code not taken
            var codeTaken = Tree.Images
                .Any(i => i.Code == VM.Code && i.Id != Image.Id);

            if (codeTaken)
                return BadRequest("Code taken within the tree");

            //Update
            Image.Code = VM.Code;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditInterpretedImagePicture([FromForm] UpdateInterpretedNodeViewModel VM)
        {
            //Check image exists
            var Image = await _applicationDbContext.InterpretedImages
                .Include(i => i.Group)
                .ThenInclude(g => g.Images)
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Image is null)
                return NotFound("Node not found");

            var Tree = Image.Group;

            //Check picture
            if (VM.Picture is null)
                return BadRequest("Please provide picture");

            //Verify Extension
            var isExtensionValid = validateImageExtension(VM.Picture);

            if (!isExtensionValid)
                return BadRequest("Picture extenstion not valid");

            //Try removing image
            if (Image.ImageURL != null)
            {
                RemoveFile(Image.ImageURL);
            }

            //Save picture and generate url
            var URL = await SaveFile(VM.Picture);

            Image.ImageURL = URL;
            Image.Size = VM.Picture.Length;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditInterpretedImageValues([FromForm] UpdateInterpretedNodeViewModel VM)
        {
            //Check image exists
            var Image = await _applicationDbContext.InterpretedImages
                .Include(i => i.Group)
                .ThenInclude(g => g.Images)
                .FirstOrDefaultAsync(i => i.Id == VM.Id);

            if (Image is null)
                return NotFound("Node not found");

            var Group = Image.Group;

            //Check interpretation values exist
            var Left = await _applicationDbContext.LeftGradientValues
                .FirstOrDefaultAsync(l => l.Id == VM.LeftId);

            if (Left is null)
                return BadRequest("Interpretation value not found");

            var Right = await _applicationDbContext.RightGradientValues
                .FirstOrDefaultAsync(l => l.Id == VM.RightId);

            if (Right is null)
                return BadRequest("Interpretation value not found");


            var Ratio = await _applicationDbContext.RationOfGradientsValues
                .FirstOrDefaultAsync(l => l.Id == VM.RatioId);

            if (Ratio is null)
                return BadRequest("Interpretation value not found");


            var Jump = await _applicationDbContext.JumpValues
                .FirstOrDefaultAsync(l => l.Id == VM.JumpId);

            if (Jump is null)
                return BadRequest("Interpretation value not found");

            //Check if image already exists [Values]
            if (Group.Images.Any(i => i.Id != Image.Id && (i.JumpId == VM.JumpId && i.LeftId == VM.LeftId && i.RightId == VM.RightId && i.RationOfGradientsId == VM.RatioId)))
                return BadRequest("Values already used within the tree");

            //Update
            Image.RightId = Right.Id;
            Image.LeftId = Left.Id;
            Image.RationOfGradientsId = Ratio.Id;
            Image.JumpId = Jump.Id;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
