using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.SearchEngineRequests;
using heatquizapp_api.Models.Series;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace heatquizapp_api.Controllers.SearchEngineController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchEngineController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public SearchEngineController(
            ApplicationDbContext applicationDbContext,
            IMapper mapper
            )
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        //Change in vs code -- original: SearchSeries_ADVANCED
        public async Task<IActionResult> SearchSeries([FromBody] SeriesSearchViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<QuestionSeries, bool>> criteria = (s) =>
                (
                s.DataPoolId == VM.DataPoolId &&
                //&&(!VM.Used ? s.MapElements.Count == 0 : true)
                //&&
                (
                !string.IsNullOrEmpty(VM.Adder) ? s.AddedBy.Name == VM.Adder : true
                ) &&
                (
                !string.IsNullOrEmpty(VM.Code) ? s.Code.ToLower().Contains(VM.Code.ToLower()) : true
                )
                );

            var CodesNumbers = await _applicationDbContext.QuestionSeries
                .Where(criteria)
               .OrderBy(q => q.Code)
               .Select(q => q.Code[0])
               .ToListAsync();

            var Codes = new List<CodeNumberViewModel>();

            foreach (var c in CodesNumbers)
            {
                if (Codes.Count == 0)
                {
                    Codes.Add(new CodeNumberViewModel()
                    {
                        Code = c,
                        Number = 1
                    });

                    continue;
                }

                if (Codes.Last().Code == c)
                {
                    Codes.Last().Number += 1;
                }
                else
                {
                    Codes.Add(new CodeNumberViewModel()
                    {
                        Code = c,
                        Number = 1
                    });
                }
            }

            var SeriesIds = new List<int>();

            SeriesIds = await _applicationDbContext.QuestionSeries
              .Where(criteria)
            .OrderBy(q => q.Code)
            .Select((q) => q.Id)
               .ToListAsync();

            var SeriesCodes = await _applicationDbContext.QuestionSeries
                 .Where(criteria)
                .OrderBy(q => q.Code)
            .Select((q) => q.Code)
               .ToListAsync();

            var Series = await _applicationDbContext.QuestionSeries
                .Include(s => s.Elements)
                .Include(s => s.AddedBy)
                //.Include(s => s.MapElements)
                //.ThenInclude(me => me.Map)
                

                .Where(criteria)

                .OrderBy(s => s.Code)
                .Skip(VM.Page * VM.QperPage)
                .Take(VM.QperPage)
                .ToListAsync();

            return Ok(new
            {
                Series = _mapper.Map<List<QuestionSeries>, List<QuestionSeriesViewModel>>(Series),
                Codes = Codes,
                NumberOfObjects = CodesNumbers.Count,
                ObjectsIds = SeriesIds,
                ObjectsCodes = SeriesCodes
            });
        }

        [HttpPost("[action]")]
        //Change in vs code -- original SearchSeriesByIds_ADVANCED
        public async Task<IActionResult> SearchSeriesByIds([FromBody] SearchByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Series = await _applicationDbContext.QuestionSeries
                .Where(q => VM.Ids.Any(Id => Id == q.Id))
                .Include(s => s.Elements)
                .Include(s => s.AddedBy)

                //.Include(s => s.MapElements)
                //.ThenInclude(me => me.Map)
                
                .ToListAsync();

            return Ok(new
            {
                Series = _mapper.Map<List<QuestionSeries>, List<QuestionSeriesViewModel>>(Series),
                Codes = VM.Codes,
                NumberOfSeries = VM.NumberOfObjects,
                SeriesIds = VM.ObjectsIds,
                SeriesCodes = VM.Codes
            });

        }

    }
}
