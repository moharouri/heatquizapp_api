using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Topics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.TopicController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public TopicController(
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
        public async Task<IActionResult> GetAllTopics([FromBody] DatapoolCarrierViewModel VM)
        {
            if(!ModelState.IsValid)
               return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Topics = await _applicationDbContext.Topics
                .Include(t => t.Subtopics)
               
                .Include(t => t.Subtopics)
                .ThenInclude(t => t.Questions)

                .OrderBy(t => t.Name)
                .Where(t => t.DataPoolId == VM.DatapoolId)
                .ToListAsync();

            return Ok(_mapper.Map<List<Topic>, List<TopicViewModel>>(Topics));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return BadRequest("Datapool not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check name unique in datapool
            var nameTaken = await _applicationDbContext.Topics
                .AnyAsync(t => t.Name == VM.Name && t.DataPoolId == DP.Id);

            if (nameTaken)
                return BadRequest("Name is taken already");

            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Topic
            var topic = new Topic()
            {
                Name = VM.Name,
                AddedById = adder.Id,
                DataPoolId = DP.Id

            };

            _applicationDbContext.Topics.Add(topic);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Topic, TopicViewModel>(topic));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddSubtopic([FromBody] AddSubtopicViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check topic exists
            var Topic = await _applicationDbContext.Topics
                .Include(t => t.Subtopics)
                .FirstOrDefaultAsync(t => t.Id == VM.TopicId);

            if (Topic is null)
                return NotFound("Topic not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check Name Unique in Topic 
            if (Topic.Subtopics.Any(st => st.Name == VM.Name))
                return BadRequest("Name is already taken");

            var Subtopic = new Subtopic()
            {
                Name = VM.Name,
                Topic = Topic,
                DataPoolId = Topic.DataPoolId
            };

            _applicationDbContext.Subtopics.Add(Subtopic);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Topic, TopicViewModel>(Topic));
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> EditTopic([FromBody] UpdateTopicViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Topic = await _applicationDbContext.Topics
                .FirstOrDefaultAsync(st => st.Id == VM.Id);

            if (Topic is null)
                return NotFound("Topic not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check name unique in datapool
            var nameTaken = await _applicationDbContext.Topics
                .AnyAsync(t => t.Name == VM.Name && t.Id != VM.Id && t.DataPoolId == Topic.DataPoolId);

            if (nameTaken)
                return BadRequest("Name is taken already");

            //Update
            Topic.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Topic, TopicViewModel>(Topic));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditSubtopic([FromBody] UpdateSubtopicViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Subtopic = await _applicationDbContext.Subtopics
                .Include(st => st.Topic)
                .ThenInclude(t => t.Subtopics)
                .FirstOrDefaultAsync(st => st.Id == VM.Id);

            if (Subtopic is null)
                return NotFound("Subtopic not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check name unique in datapool 
            if (Subtopic.Topic.Subtopics.All(st => st.Name == VM.Name))
                return BadRequest("Name is taken already");
            
            //Update
            Subtopic.Name = VM.Name;

            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Topic, TopicViewModel>(Subtopic.Topic));
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteTopic([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check topic exists
            var Topic = await _applicationDbContext.Topics
                .Include(t => t.Subtopics)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(st => st.Id == VM.Id);

            if (Topic is null)
                return NotFound("Topic not found");

            if (Topic.Subtopics.Any(s => s.Questions.Any()))
                return BadRequest("Topic has at least one subtopic with questions");

            _applicationDbContext.Topics.Remove(Topic);

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteSubtopic([FromBody] UniversalDeleteViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check subtopic exists
            var Subtopic = await _applicationDbContext.Subtopics
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(st => st.Id == VM.Id);

            if (Subtopic is null)
                return NotFound("Subtopic not found");

            if (Subtopic.Questions.Any())
                return BadRequest("Subtopic has questions");

            _applicationDbContext.Subtopics.Remove(Subtopic);

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
