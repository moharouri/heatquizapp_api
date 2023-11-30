using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Series;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;


namespace heatquizapp_api.Controllers.QuestionSeriesController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionSeriesController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public QuestionSeriesController(
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

        [HttpGet("[action]/{Code}")]
        public async Task<IActionResult> GetSeries(string Code)
        {
            var Series = await _applicationDbContext.QuestionSeries

                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.Information)

                .FirstOrDefaultAsync(s => s.Code == Code);

            if (Series is null)
                return NotFound("Not found");

            //Send elements in order
            Series.Elements = Series.Elements.OrderBy(e => e.Order).ToList();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpGet("[action]/{Code}")]
        //Change its name in vs code
        public async Task<IActionResult> GetSeriesExtended(string Code)
        {
            var Series = await _applicationDbContext.QuestionSeries


                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.LevelOfDifficulty)

                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.Subtopic)
                .ThenInclude(st => st.Topic)

                /*.Include(s => s.MapElements)
                .ThenInclude(me => me.Map)
                .ThenInclude(m => m.Course)*/

                .FirstOrDefaultAsync(s => s.Code == Code);

            if (Series is null)
                return NotFound("Not Found");

            Series.Elements = Series.Elements.OrderBy(e => e.Order).ToList();

            if (Series is null)
                return NotFound("Not Found");

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSeriesAdders([FromBody] DatapoolCarrierViewModel VM)
        {
            if(!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Adders = await _applicationDbContext.QuestionSeries
                .Where(s => s.DataPoolId == VM.DatapoolId)
                .Select(s => s.AddedBy)
                .Distinct()
                .Select(a => a.Name)
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(Adders);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddSeries([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
                .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check code is unique 
            var CodeTaken = await _applicationDbContext.QuestionSeries
                .AnyAsync(s => s.Code == VM.Code); // && s.DataPoolId == DP.Id -- should be global since series is accessed on website through code

            if (CodeTaken)
                return BadRequest("Code exists a choose different code");

            //Check questions exist
            var Questions = await _applicationDbContext.QuestionBase
                .Where(q => VM.Elements.Any(e => e.QuestionId == q.Id) && q.DataPoolId == DP.Id)
                .ToListAsync();

            if (Questions.Count != VM.Elements.Count)
                return BadRequest("Some questions do not exist");

            //Check randomness consistency
            if (VM.RandomSize < 1 && VM.IsRandom)
                return BadRequest("Random size should be positive");

            if (VM.RandomSize > VM.Elements.Count)
                return BadRequest("Random size is greater than number of questions");

            //Add elements
            var Elements = new List<QuestionSeriesElement>();

            Elements.AddRange(VM.Elements
                .Select(e => new QuestionSeriesElement()
                {
                    QuestionId = e.QuestionId,
                    Order = e.Order,
                    DataPoolId = DP.Id,
                    PoolNumber = 1
                }));

            //Get adder
            var Adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create series
            var Series = new QuestionSeries()
            {
                Code = VM.Code,
                Elements = Elements,
                AddedBy = Adder,
                IsRandom = VM.IsRandom,
                RandomSize = VM.RandomSize,
                DataPoolId = DP.Id,
                NumberOfPools = 1
            };
          
            _applicationDbContext.QuestionSeries.Add(Series);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddSeriesElements([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                 .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Series is null)
                return NotFound("Series not found");

            //Check questions already included
            if (Series.Elements.Any(e =>VM.Elements.Any(evm => evm.QuestionId == e.QuestionId)))
                return NotFound("Some questions are already included");

            var Questions = await _applicationDbContext.QuestionBase
                .Where(q => VM.Elements.Any(e => e.QuestionId == q.Id) && q.DataPoolId == Series.DataPoolId)
                .ToListAsync();

            if (Questions.Count != VM.Elements.Count)
                return BadRequest("Some questions do not exist");

            //Add elements
            var Elements = new List<QuestionSeriesElement>();

            foreach (var e in VM.Elements.OrderBy(e => e.Order))
            {
                var element = new QuestionSeriesElement()
                {
                    Order = e.Order + Series.Elements.Count
                };

                element.QuestionId = e.QuestionId;
                element.PoolNumber = 1;

                Elements.Add(element);
            }


            Series.Elements.AddRange(Elements);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPut("[action]")]
        //Change api type, change name in vs code -- original: EditSeriesCode
        public async Task<IActionResult> EditSeriesInfo([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Series is null)
                return NotFound("Series not found");

            //Check code is taken
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeTaken = await _applicationDbContext.QuestionSeries
                .AnyAsync(s => s.Code == VM.Code && s.Id != VM.Id);

            if (codeTaken)
                return BadRequest("Code exists choose different code");
           
            //Check randomness consistency
            if ((VM.IsRandom) && (VM.RandomSize > Series.Elements.Count))
                return BadRequest("Random size can't be bigger than series elements");

            if ((VM.IsRandom) && (VM.RandomSize < 1))
                return BadRequest("Random size should be positive");

            //Update
            Series.Code = VM.Code;
            Series.RandomSize = VM.RandomSize;
            Series.IsRandom = VM.IsRandom;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPut("[action]")]
        //Change api type
        public async Task<IActionResult> AssignElementsToPool([FromBody] AssignElementsToPoolViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get elements
            var Elements = await _applicationDbContext.QuestionSeriesElement
                .Include(e => e.Series)
                .ThenInclude(s => s.Elements)
                .Where(e => VM.SelectedElements.Any(se => se == e.Id))
                .ToListAsync();

            if (Elements.Count != VM.SelectedElements.Distinct().Count())
                return NotFound("Elements not found");

            //Check elements belong to same series
            var Series = Elements.Select(e => e.Series).Distinct().ToList();

            if (Series.Count > 1)
                return BadRequest("Elements do not belong to same series");

            //Check pool number is valid
            var NumberOfPools = Series.FirstOrDefault().NumberOfPools;

            if (VM.Pool > NumberOfPools)
                return BadRequest("Max pool exceeded");

            if (VM.Pool < 1)
                return BadRequest("Pool cannot be less than 1!");

            //Update
            foreach (var e in Elements)
            {
                e.PoolNumber = VM.Pool;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series.FirstOrDefault()));
        }

        [HttpPut("[action]")]
        //Change api type
        public async Task<IActionResult> DeselectElementSeries([FromBody] QuestionSeriesElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check element exists
            var Element = await _applicationDbContext.QuestionSeriesElement
                .Include(e => e.Series)
                .ThenInclude(s => s.Elements)
                .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Not Found");

            //Check removal possibility
            var Series = Element.Series;

            if (Series.Elements.Count < 1)
                return BadRequest("Series should include atleast one question");

            if (Series.IsRandom && (Series.Elements.Count - 1) < Series.RandomSize)
                return BadRequest("Series should have enough number of questions to be a random series");

            //Remove
            Series.Elements.Remove(Element);

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPut("[action]")]
        //Change api type
        public async Task<IActionResult> DecreasePoolsNumber([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Series is null)
                return NotFound("Series not found");

            //Check possibility of pool number update
            var NumberOfPools = Series.NumberOfPools;

            if (NumberOfPools == 1)
                return BadRequest("Number Of pools cannot be lower than 1");

            //Update
            foreach (var e in Series.Elements.Where(e => e.PoolNumber == NumberOfPools))
            {
                e.PoolNumber -= 1;
            }

            Series.NumberOfPools -= 1;
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPut("[action]")]
        //Change api type
        public async Task<IActionResult> IncreasePoolsNumber([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Series is null)
                return NotFound("Series not found");

            //Update
            Series.NumberOfPools += 1;
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpPut("[action]")]
        //Change api type
        public async Task<IActionResult> RearrangeSeries([FromBody] QuestionSeriesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Series is null)
                return NotFound("Series not found");

            //Check all elements of Series exist in VM
            if (Series.Elements.Any(e => !VM.Elements.Any(vme => vme.Id == e.Id)))
                return BadRequest("Data inconsistent");

            //map VM elements into a dictionary
            var VMElements = VM.Elements.ToDictionary(e => e.Id);

            foreach (var element in Series.Elements)
            {
                element.Order = VMElements[element.Id].Order;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddStatistic(
            int SeriesId,
            string Player,
            string MapKey,
            string MapName,
            string MapElementName,
            string SuccessRate,
            int TotalTime,
            bool OnMobile)
        {
            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
                .FirstOrDefaultAsync(q => q.Id == SeriesId);

            if (Series is null)
                return NotFound("Series not found");

            //Add statistic
            Series.Statistics.Add(new QuestionSeriesStatistic()
            {
                Player = Player,
                MapKey = MapKey,
                MapName = MapName,
                MapElementName = MapElementName,
                SuccessRate = SuccessRate,

                TotalTime = TotalTime,
                DataPoolId = Series.DataPoolId,
                OnMobile = OnMobile,
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


    }
}
