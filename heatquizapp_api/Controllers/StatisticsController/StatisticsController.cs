using HeatQuizAPI.Database;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Series;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using heatquizapp_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace heatquizapp_api.Controllers.StatisticsController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsStartDateStorage _statsStartDateStorage;
        private readonly ApplicationDbContext _applicationDbContext;

        public StatisticsController(
            IStatisticsStartDateStorage statsStartDateStorage,
            ApplicationDbContext applicationDbContext
         )
        {
            _statsStartDateStorage = statsStartDateStorage;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetQuestionStatistics([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var startDate = _statsStartDateStorage.StartDate;

            Expression<Func<QuestionBase, dynamic>> queryExpression;

            if (startDate.HasValue)
            {
                queryExpression = q => new {
                    Id = q.Id,

                    TotalPlay = q.QuestionStatistics.Count(s => s.DateCreated >= startDate),
                    CorrectPlay = q.QuestionStatistics.Count(s => s.Correct && s.DateCreated >= startDate),
                    WrongPlay = q.QuestionStatistics.Count(s => !s.Correct && s.DateCreated >= startDate),

                    MedianPlayTime = q.QuestionStatistics.Any(s => s.DateCreated >= startDate) ? q.QuestionStatistics.Where(s =>  s.DateCreated >= startDate).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(s => s.DateCreated >= startDate) / 2].TotalTime : 0,
                    MedianPlayTimeWrong = q.QuestionStatistics.Any(s => !s.Correct) ? q.QuestionStatistics.Where(s => !s.Correct && s.DateCreated >= startDate).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => !a.Correct && a.DateCreated >= startDate) / 2].TotalTime : 0,
                    MedianPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct) ? q.QuestionStatistics.Where(s => s.Correct && s.DateCreated >= startDate).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => a.Correct && a.DateCreated >= startDate) / 2].TotalTime : 0,

                    TotalPDFViews = q.QuestionPDFStatistics.Count(s => s.DateCreated >= startDate),
                    TotalPDFViewsWrong = q.QuestionPDFStatistics.Count(a => !a.Correct && a.DateCreated >= startDate),
                };
            }
            else
            {
                queryExpression = q => new {
                    Id = q.Id,

                    TotalPlay = q.QuestionStatistics.Count,
                    CorrectPlay = q.QuestionStatistics.Count(s => s.Correct),
                    WrongPlay = q.QuestionStatistics.Count(s => !s.Correct),

                    MedianPlayTime = q.QuestionStatistics.Any() ? q.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count() / 2].TotalTime : 0,
                    MedianPlayTimeWrong = q.QuestionStatistics.Any(s => !s.Correct) ? q.QuestionStatistics.Where(s => !s.Correct).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => !a.Correct) / 2].TotalTime : 0,
                    MedianPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct) ? q.QuestionStatistics.Where(s => s.Correct).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => a.Correct) / 2].TotalTime : 0,

                    TotalPDFViews = q.QuestionPDFStatistics.Count,
                    TotalPDFViewsWrong = q.QuestionPDFStatistics.Count(a => !a.Correct),
                };
            }

            //Get Question & Stats
            var Question = await _applicationDbContext.QuestionBase
                .Include(q => q.QuestionStatistics)
                .Include(q => q.QuestionPDFStatistics)

                .Where(q => q.Id == VM.Id)

                .Select(queryExpression)
                .FirstOrDefaultAsync();

            if (Question is null)
                return NotFound("Question not found");

            return Ok(Question);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSeriesStatistics([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var startDate = _statsStartDateStorage.StartDate;

            Expression<Func<QuestionSeries, dynamic>> queryExpression;

            if (startDate == null)
            {
                queryExpression = s => new
                {
                    Code = s.Code,
                    Id = s.Id,
                    TotalPlay = s.Statistics.Count,
                    TotalPlayOnMobile = s.Statistics.Count(a => a.OnMobile),

                    MedianPlayTime = s.Statistics.Any() ? s.Statistics.OrderBy(a => a.TotalTime).ToList()[(int)(s.Statistics.Count() / 2)].TotalTime : 0,

                    ElementStats = s.Elements
                    .OrderBy(e => e.Order)
                    .Select(e => new
                    {
                        Id = e.Id,

                        TotalPlay = e.Question.QuestionStatistics.Count,
                        TotalSuccessPlay = e.Question.QuestionStatistics.Count(st => st.Correct),

                        MedianPlayTime = e.Question.QuestionStatistics.Any() ? e.Question.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[e.Question.QuestionStatistics.Count() / 2].TotalTime : 0,

                    })

                };
            }
            else
            {
                queryExpression = s => new
                {
                    Code = s.Code,
                    Id = s.Id,

                    TotalPlay = s.Statistics.Count(s => s.DateCreated >= startDate),
                    TotalPlayOnMobile = s.Statistics.Count(s => s.OnMobile && s.DateCreated >= startDate),

                    MedianPlayTime = s.Statistics.Any(s => s.DateCreated >= startDate) ? s.Statistics.OrderBy(a => a.TotalTime).ToList()[(int)(s.Statistics.Count(s => s.DateCreated >= startDate) / 2)].TotalTime : 0,

                    ElementStats = s.Elements
                    .OrderBy(e => e.Order)
                    .Select(e => new
                    {
                        Id = e.Id,

                        TotalPlay = e.Question.QuestionStatistics.Count,
                        TotalSuccessPlay = e.Question.QuestionStatistics.Count(st => st.Correct),

                        MedianPlayTime = e.Question.QuestionStatistics.Any(s => s.DateCreated >= startDate) ? e.Question.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[e.Question.QuestionStatistics.Count(s => s.DateCreated >= startDate) / 2].TotalTime : 0,

                    })

                };
            }

            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var SeriesStats = await _applicationDbContext.QuestionSeries
                .Where(s => s.Id == VM.Id)

                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.QuestionStatistics)

                .Select(queryExpression)
                .FirstOrDefaultAsync();

            return Ok(SeriesStats);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetQuestionMedianTimeSpectrum([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var startDate = _statsStartDateStorage.StartDate;

            Expression<Func<QuestionBase, dynamic>> queryExpression;

            if (startDate == null)
            {
                queryExpression = q => new
                {
                    Id = q.Id,

                    MedianPlayTime = q.QuestionStatistics.Any() ? q.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count / 2].TotalTime : 0,
                    MedianPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct) ? q.QuestionStatistics.Where(s => s.Correct).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => a.Correct) / 2].TotalTime : 0,

                    AvgPlayTime = q.QuestionStatistics.Any() ? q.QuestionStatistics.Sum(a => a.TotalTime) / q.QuestionStatistics.Count() : '-',
                    AvgPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct) ? q.QuestionStatistics.Where(s => s.Correct).Sum(a => a.TotalTime) / q.QuestionStatistics.Count(s => s.Correct) : '-',
                    
                    TimeSpectrum = q.QuestionStatistics
                    .GroupBy(a => a.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count()
                    }),

                    TimeSpectrumCorrect = q.QuestionStatistics
                    .Where(s => s.Correct)
                    .GroupBy(a => a.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count()
                    }),

                    TotalPlay = q.QuestionStatistics.Count,
                    TotalPlaySuccess = q.QuestionStatistics.Count(a => a.Correct),
                };
            }
            else
            {
                queryExpression = q => new
                {
                    Id = q.Id,

                    MedianPlayTime = q.QuestionStatistics.Any(s => s.DateCreated >= startDate) ? q.QuestionStatistics.Where(s => s.DateCreated >= startDate).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(s => s.DateCreated >= startDate) / 2].TotalTime : 0,
                    MedianPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct && s.DateCreated >= startDate) ? q.QuestionStatistics.Where(s => s.Correct && s.DateCreated >= startDate).OrderBy(a => a.TotalTime).ToList()[q.QuestionStatistics.Count(a => a.Correct && a.DateCreated >= startDate) / 2].TotalTime : 0,

                    AvgPlayTime = q.QuestionStatistics.Any(s => s.DateCreated >= startDate) ? q.QuestionStatistics.Where(s => s.DateCreated >= startDate).Sum(a => a.TotalTime) / q.QuestionStatistics.Count(s => s.DateCreated >= startDate) : '-',
                    AvgPlayTimeCorrect = q.QuestionStatistics.Any(s => s.Correct && s.DateCreated >= startDate) ? q.QuestionStatistics.Where(s => s.Correct && s.DateCreated >= startDate).Sum(a => a.TotalTime) / q.QuestionStatistics.Count(s => s.Correct && s.DateCreated >= startDate) : '-',

                    TimeSpectrum = q.QuestionStatistics
                    .Where(s => s.DateCreated >= startDate)
                    .GroupBy(a => a.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count()
                    }),

                    TimeSpectrumCorrect = q.QuestionStatistics
                    .Where(s => s.Correct && s.DateCreated >= startDate)
                    .GroupBy(a => a.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count()
                    }),

                    TotalPlay = q.QuestionStatistics.Count,
                    TotalPlaySuccess = q.QuestionStatistics.Count(a => a.Correct),
                };
            }

            //Get Question
            var Data = await _applicationDbContext.QuestionBase
                .Include(q => q.QuestionStatistics)
                .Where(q => q.Id == VM.Id)
                .Select(queryExpression)
                .FirstOrDefaultAsync();

            if (Data is null)
                return NotFound("Question not found");

            return Ok(Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSeriesMedianTimeSpectrum([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var startDate = _statsStartDateStorage.StartDate;

            Expression<Func<QuestionSeries, dynamic>> queryExpression;

            if (startDate == null)
            {
                queryExpression = s => new
                {
                    Id = s.Id,

                    MedianPlayTime = s.Statistics.Any() ? s.Statistics.OrderBy(a => a.TotalTime).ToList()[(int)(s.Statistics.Count() / 2)].TotalTime : 0,

                    AvgPlayTime = s.Statistics.Any() ? s.Statistics.Sum(a => a.TotalTime) / s.Statistics.Count : '-',

                    MedianPlayTimeMobile = s.Statistics.Any(s => s.OnMobile) ? s.Statistics.OrderBy(a => a.TotalTime).ToList()[(int)(s.Statistics.Count(s => s.OnMobile) / 2)].TotalTime : 0,

                    AvgPlayTimeMobile = s.Statistics.Any(s => s.OnMobile) ? s.Statistics.Where(s => s.OnMobile).Sum(a => a.TotalTime) / s.Statistics.Where(s => s.OnMobile).Count() : '-',

                    TimeSpectrum = s.Statistics
                    .GroupBy(a => a.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count(),
                    }),


                    TotalPlay = s.Statistics.Count,
                    TotalPlayMobile = s.Statistics.Count(s => s.OnMobile)
                };
            }
            else
            {
                queryExpression = s => new
                {
                    Id = s.Id,

                    MedianPlayTime = s.Statistics.Any(a => a.DateCreated >= startDate) ? s.Statistics.Where(a => a.DateCreated >= startDate).OrderBy(a => a.TotalTime).Skip(s.Statistics.Count(a => a.DateCreated >= startDate) / 2).FirstOrDefault().TotalTime : '-',

                    AvgPlayTime = s.Statistics.Any(a => a.DateCreated >= startDate) ? s.Statistics.Where(a => a.DateCreated >= startDate).Sum(a => a.TotalTime) / s.Statistics.Count(a => a.DateCreated >= startDate) : '-',

                    MedianPlayTimeMobile = s.Statistics.Any(s => s.OnMobile && s.DateCreated >= startDate) ? s.Statistics.Where(s => s.OnMobile && s.DateCreated >= startDate).OrderBy(s => s.TotalTime).Skip(s.Statistics.Count(s => s.OnMobile && s.DateCreated >= startDate) / 2).FirstOrDefault().TotalTime : '-',

                    AvgPlayTimeMobile = s.Statistics.Any(s => s.OnMobile && s.DateCreated >= startDate) ? s.Statistics.Where(s => s.OnMobile).Sum(a => a.TotalTime) / s.Statistics.Count(s => s.OnMobile && s.DateCreated >= startDate) : '-',


                    TimeSpectrum = s.Statistics
                    .Where(s => s.DateCreated >= startDate)
                    .GroupBy(s => s.TotalTime)
                    .Select(r => new
                    {
                        Time = r.Key,
                        Count = r.Count(),
                    }),


                    TotalPlay = s.Statistics.Count(s => s.DateCreated >= startDate),
                    TotalPlayMobile = s.Statistics.Count(s => s.OnMobile && s.DateCreated >= startDate)
                };
            }

            //Get Question
            var Data = await _applicationDbContext.QuestionSeries
                .Include(s => s.Statistics)
                .Where(s => s.Id == VM.Id)
                .Select(queryExpression)
                .FirstOrDefaultAsync();

            if (Data is null)
                return NotFound("Series not found");

            return Ok(Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCourseMapStatisticsById([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Stats = await _applicationDbContext.CourseMap

                .Select(c => new
                {
                    Id = c.Id,
                    Elements = c.Elements.Select(e => new {
                        Id = e.Id,
                        PDFStatisticsCount = e.PDFStatistics.Count,
                        PDFStatisticsCountOnMobile = e.PDFStatistics.Count(s => s.OnMobile),

                        SeriesPlayCount = e.QuestionSeries != null ? e.QuestionSeries.Statistics.Count : 0,
                        SeriesPlayCountOnMobile = e.QuestionSeries != null ? e.QuestionSeries.Statistics.Count(s => s.OnMobile) : 0,

                        SeriesPlayMedianTime = e.QuestionSeries != null && e.QuestionSeries.Statistics.Any() ? e.QuestionSeries.Statistics.OrderBy(a => a.TotalTime).ToList()[(int)(e.QuestionSeries.Statistics.Count() / 2)].TotalTime : 0
                    }).ToList()
                })
               .FirstOrDefaultAsync(c => c.Id == VM.Id);

            if (Stats is null)
                return NotFound("Map not found");

            return Ok(Stats);
        }

    }
}
