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

namespace heatquizapp_api.Controllers.ClickImageTreesController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
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
        //Change type change name original: GetImageAnswerGroups
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
                .Include(g => g.AddedBy)

                .OrderBy(g => g.Name)

                .Where(g => g.DataPoolId == VM.DatapoolId)
                .ToListAsync();

            var IAs = await _applicationDbContext.ImageAnswers
                .Where(ia => Groups.Any(g => g.Id == ia.GroupId) && !ia.RootId.HasValue)
                .Include(ia => ia.Leafs)
                //.ThenInclude(l => l.ClickImages)
                //.Include(ia => ia.ClickImages)
                .Where(g => g.DataPoolId == VM.DatapoolId)

                .ToListAsync();

            foreach (var g in Groups)
            {
                g.Images = IAs
                    .Where(ia => ia.GroupId == g.Id)
                    .ToList();
            }

            return Ok(_mapper.Map<List<ImageAnswerGroup>, List<ImageAnswerGroupViewModel>>(Groups));
        }

        [HttpPost("[action]")]
        //Change name in vs code AddGroup
        public async Task<IActionResult> AddTree([FromBody] ImageAnswerGroupViewModel VM)
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
        //Change name and type in vs code original EditGroup
        public async Task<IActionResult> EditTree([FromBody] ImageAnswerGroupViewModel VM)
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

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> DeleteTree([FromBody] ImageAnswerGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Tree = await _applicationDbContext.ImageAnswerGroups

                .Include(g => g.Images)
                .ThenInclude(ia => ia.Leafs)
                //.ThenInclude(l => l.ClickImages)

                .Include(g => g.Images)
                //.ThenInclude(ia => ia.ClickImages)

                .FirstOrDefaultAsync(ia => ia.Id == VM.Id);

            if (Tree is null)
                return NotFound("Tree not Found");

            /*if (Tree.Images.Any(a => a.ClickImages.Count != 0 || a.Leafs.Any(l => l.ClickImages.Count != 0)))
                return BadRequest("Tree Used in Questions, Can't Be Deleted");*/

            //Remove
            foreach (var i in Tree.Images)
            {
                foreach (var ai in i.Leafs)
                {
                    _applicationDbContext.ImageAnswers.Remove(ai);
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

                .FirstOrDefaultAsync(g => g.Id == VM.GroupId);

            if (Tree is null)
                return NotFound($"Tree not found");

            if (Tree.Images.Any(i => i.Name == VM.Name))
                return BadRequest("Name already used within the tree");

            
            //Create Answer
            var Answer = new ImageAnswer()
            {
                Name = VM.Name,
                Choosable = false,
                GroupId = Tree.Id,
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
        //Change type in vs code
        public async Task<IActionResult> EditAnswerImageOneStep([FromForm] EditNodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Answer Exists
            var Answer = await _applicationDbContext.ImageAnswers

                .Include(aa => aa.Group)
                .ThenInclude(g => g.Images)
                .ThenInclude(i => i.Leafs)
                .Include(aa => aa.Group)

                .FirstOrDefaultAsync(a => a.Id == VM.AnswerId);

            if (Answer is null)
                return NotFound("Image not found");

            //Check name not 
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Please provide name");
          
            var Group = Answer.Group;

            if (Group.Images.Any(i => i.Name == VM.Name && i.Id != VM.AnswerId))
                return BadRequest("Name already used within the group");

            if (!VM.SameImage)
            {
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
            }    


            Answer.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<ImageAnswer, ImageAnswerViewModel>(Answer));
        }

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> DeleteNode([FromBody] ImageAnswerViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check node exists
            var IA = await _applicationDbContext.ImageAnswers
                .Include(ia => ia.Leafs)
                //.ThenInclude(l => l.ClickImages)
                //.Include(ia => ia.ClickImages)
                .FirstOrDefaultAsync(ia => ia.Id == VM.Id);

            if (IA is null)
                return NotFound("Node not found");

            /*if (IA.ClickImages.Count != 0 || IA.Leafs.Any(l => l.ClickImages.Count != 0))
                return BadRequest("Node Used in Questions, Can't Be Deleted");*/

            //Remove
            foreach (var ai in IA.Leafs)
            {
                _applicationDbContext.ImageAnswers.Remove(ai);
            }

            _applicationDbContext.ImageAnswers.Remove(IA);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
