using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.ClickImageTrees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;
using HeatQuizAPI.Utilities;
using System.Xml.Linq;
using heatquizapp_api.Models;
using System.Text.RegularExpressions;

namespace heatquizapp_api.Controllers.ClickImageTreesController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class ImageAnswersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public ImageAnswersController(
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
        public async Task<IActionResult> GetClickTrees([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(d => d.Id == VM.DatapoolId);

            if (DP is null)
                return NotFound("Datapool does not exist");

            var Groups = await _applicationDbContext.ImageAnswerGroups
                .Where(g => g.DataPoolId == VM.DatapoolId)

                .Include(g => g.AddedBy)
                .Include(g => g.Images)
                .ThenInclude(l => l.ClickImages)

                .Include(g => g.Images)
                .ThenInclude(ia => ia.Leafs)
                .ThenInclude(l => l.ClickImages)

                .OrderBy(g => g.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<ImageAnswerGroup>, List<ImageAnswerGroupViewModel>>(Groups));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddTree([FromBody] AddImageAnswerGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be null");

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check name not taken
            var nameTaken = await _applicationDbContext.ImageAnswerGroups
                .AnyAsync(g => g.Name == VM.Name && g.DataPoolId == DP.Id);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Tree
            var Tree = new ImageAnswerGroup()
            {
                Name = VM.Name,
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

            _applicationDbContext.ImageAnswerGroups.Add(Tree);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<ImageAnswerGroup, ImageAnswerGroupViewModel>(Tree));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditTree([FromBody] UpdateImageAnswerGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check tree exists
            var Tree = await _applicationDbContext.ImageAnswerGroups
                .FirstOrDefaultAsync(g => g.Id == VM.Id);

            if (Tree is null)
                return NotFound($"Tree not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name Can't Be Null");

            //Check name not taken
            var nameTaken = await _applicationDbContext.ImageAnswerGroups
                .AnyAsync(g => g.Id != VM.Id && g.Name == VM.Name && g.DataPoolId == Tree.DataPoolId);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Update tree
            Tree.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<ImageAnswerGroup, ImageAnswerGroupViewModel>(Tree));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteTree([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Tree = await _applicationDbContext.ImageAnswerGroups

                .Include(g => g.Images)
                .ThenInclude(ia => ia.Leafs)
                .ThenInclude(l => l.ClickImages)

                .Include(g => g.Images)
                .ThenInclude(ia => ia.ClickImages)

                .FirstOrDefaultAsync(ia => ia.Id == VM.Id);

            if (Tree is null)
                return NotFound("Tree not found");

            if (Tree.Images.Any(a => a.ClickImages.Count != 0 || a.Leafs.Any(l => l.ClickImages.Count != 0)))
                return BadRequest("Tree used in questions, can't be deleted");

            //Remove
            foreach (var i in Tree.Images)
            {
                foreach (var ai in i.Leafs)
                {
                    //Try remove image
                    if(ai.ImageURL != null)
                    {
                        RemoveFile(ai.ImageURL);
                    }

                    _applicationDbContext.ImageAnswers.Remove(ai);
                }

                //Try remove image
                if (i.ImageURL != null)
                {
                    RemoveFile(i.ImageURL);
                }

                _applicationDbContext.ImageAnswers.Remove(i);
            }

            _applicationDbContext.ImageAnswerGroups.Remove(Tree);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddAnswerImageOneStep([FromForm] AddNodeViewModel VM)
        {
            if (!ModelState.IsValid) 
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Please Provide Name");

            //Check tree exists
            var Tree = await _applicationDbContext.ImageAnswerGroups
                .Include(g => g.Images)
                .ThenInclude(i => i.Leafs)
                .FirstOrDefaultAsync(g => g.Id == VM.GroupId);

            if (Tree is null)
                return NotFound("Tree not found");

            var OtherImages = new List<ImageAnswer>();

            OtherImages.AddRange(Tree.Images);
            OtherImages.AddRange(Tree.Images.Select(a => a.Leafs).SelectMany(r => r));

            if (OtherImages.Any(i => i.Name == VM.Name))
                return BadRequest("Name already used within the tree");

            
            //Create Answer
            var Answer = new ImageAnswer()
            {
                Name = VM.Name,
                DataPoolId = Tree.DataPoolId
            };

            if (VM.RootId.HasValue)
            {
                //Check root exists
                var Root = await _applicationDbContext.ImageAnswers
                    .Include(r => r.Leafs)
                    .FirstOrDefaultAsync(r => r.Id == VM.RootId);

                if (Root is null)
                    return NotFound($"Root not found");

                //Check root exists in this tree
                if (Root.GroupId != Tree.Id)
                    return BadRequest($"Root does not belong to tree");

                if (Root.Leafs.Any(l => l.Name == VM.Name))
                    return BadRequest("Name already used within the node");

                Answer.RootId = Root.Id;
            }
            else
            {
                Answer.GroupId = Tree.Id;
            }

            if (VM.Picture is null)
                return BadRequest("Please provide picture");

            //Verify Extension
            var isExtenstionValid = validateImageExtension(VM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extenstion not valid");

            //Save image and generate url for it
            var URL = await SaveFile(VM.Picture);

            Answer.ImageURL = URL;
            Answer.Size = VM.Picture.Length;

            _applicationDbContext.ImageAnswers.Add(Answer);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<ImageAnswer, ImageAnswerViewModel>(Answer));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditAnswerImageOneStep([FromForm] EditNodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Answer Exists
            var Answer = await _applicationDbContext.ImageAnswers
                .Include(aa => aa.Group)
                .ThenInclude(g => g.Images)
                .ThenInclude(i => i.Leafs)

                .Include(aa => aa.Root)
                .ThenInclude(g => g.Group)
                .ThenInclude(g => g.Images)
                .ThenInclude(i => i.Leafs)

                .FirstOrDefaultAsync(a => a.Id == VM.AnswerId);

            if (Answer is null)
                return NotFound("Image not found");

            //Check name not 
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Please provide name");
          
            var OtherImages = new List<ImageAnswer>();

            ImageAnswerGroup Tree;

            if (Answer.RootId.HasValue)
            {
                Tree = Answer.Root.Group;
            }
            else
            {
                Tree = Answer.Group;
            }

            OtherImages.AddRange(Tree.Images);
            OtherImages.AddRange(Tree.Images.Select(a => a.Leafs).SelectMany(r => r));

            if (OtherImages.Any(i => i.Name == VM.Name && i.Id != VM.AnswerId))
                return BadRequest("Name already used within the group");

            

            if (VM.SameImage.HasValue && !VM.SameImage.Value)
            {
                if (VM.Picture is null)
                    return BadRequest("Please provide picture");

                //Verify Extension
                var isExtenstionValid = validateImageExtension(VM.Picture);

                if (!isExtenstionValid)
                    return BadRequest("Picture extenstion not valid");

                //Try remove image
                if (Answer.ImageURL != null)
                {
                    RemoveFile(Answer.ImageURL);
                }

                //Save image and generate url for it
                var URL = await SaveFile(VM.Picture);

                Answer.ImageURL = URL;
                Answer.Size = VM.Picture.Length;
            }    


            Answer.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<ImageAnswer, ImageAnswerViewModel>(Answer));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteNode([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check node exists
            var IA = await _applicationDbContext.ImageAnswers
                .Include(ia => ia.Leafs)
                .ThenInclude(l => l.ClickImages)
                .Include(ia => ia.ClickImages)
                .FirstOrDefaultAsync(ia => ia.Id == VM.Id);

            if (IA is null)
                return NotFound("Node not found");

            if (IA.ClickImages.Count != 0 || IA.Leafs.Any(l => l.ClickImages.Count != 0))
                return BadRequest("Node used in questions, can't be deleted");

            //Try remove image
            if(IA.ImageURL != null)
            {
                RemoveFile(IA.ImageURL);
            }

            //Remove
            foreach (var ai in IA.Leafs)
            {
                if (ai.ImageURL != null)
                {
                    RemoveFile(ai.ImageURL);
                }
                _applicationDbContext.ImageAnswers.Remove(ai);
            }

            _applicationDbContext.ImageAnswers.Remove(IA);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
