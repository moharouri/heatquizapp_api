using HeatQuizAPI.Database;
using HeatQuizAPI.Mapping;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace heatquizapp_api.Controllers.ReportsController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReportsController(
            ApplicationDbContext applicationDbContext
         )
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetStudentReport([FromBody] ReportQueryViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            DateTime From;
            DateTime To;

            try
            {
                From = DateTime.ParseExact(VM.From, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                To = DateTime.ParseExact(VM.To, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch 
            {
                return BadRequest("Invalid date format");
            }

            //Question stats
            var Statsitics = await _applicationDbContext.QuestionStatistic
                .Include((s) => s.Question)
                .ThenInclude(q => q.Subtopic)
                .ThenInclude(q => q.Topic)
                .Include((s) => s.Question)
                .ThenInclude(q => q.LevelOfDifficulty)
                .Where(s =>
                (string.IsNullOrEmpty(VM.Code) ? true : (s.Key == VM.Code || s.Player == VM.Code))
                &&
                ((s.DateCreated >= From) && (s.DateCreated <= To))
                && s.DataPoolId == VM.SearchDatapoolId
                )
                .ToListAsync();

            //Series stats
            var SeriesStastics = await _applicationDbContext.QuestionSeriesStatistic
               .Include(s => s.Series)
               .ThenInclude(sr => sr.Elements)
               .Where(s =>
               (string.IsNullOrEmpty(VM.Code) ? true : (s.MapKey == VM.Code || s.Player == VM.Code))
               &&
               ((s.DateCreated >= From) && (s.DateCreated <= To))
               && s.DataPoolId == VM.SearchDatapoolId
               ).ToListAsync();

            //Map pdf click stats
            var MapPDFStatistics = await _applicationDbContext.CourseMapPDFStatistics
                .Include(s => s.Element)
                .ThenInclude(e => e.Map)
                .Where(s =>
                (string.IsNullOrEmpty(VM.Code) ? true : (s.Player == VM.Code))
                &&
                ((s.DateCreated >= From) && (s.DateCreated <= To))
                && s.Element.Map.DataPoolId == VM.SearchDatapoolId
                ).ToListAsync();

            //Map keys
            var SKeys = await _applicationDbContext.CourseMapKeys
                .Include(sk => sk.Map)
                .Where(s =>
                ((s.DateCreated >= From) && (s.DateCreated <= To) && s.DataPoolId == VM.SearchDatapoolId)
                )
                .ToListAsync();

            return Ok(new
            {
                Statsitics = Statsitics
                .OrderBy(s => s.DateCreated)
                .Select(s => new
                {
                    Key = s.Key,

                    Player = s.Player,
                    Correct = s.Correct,

                    QuestionCode = s.Question.Code,
                    QuestionId = s.Question.Id,

                    DateCreated = s.DateCreated.Value.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),

                    Topic = s.Question.Subtopic.Topic.Name,

                    Subtopic = s.Question.Subtopic.Name,

                    LevelOfDifficulty = s.Question.LevelOfDifficulty.Name,

                    TotalTime = s.TotalTime
                }),

                Keys = Statsitics.Where(s => !string.IsNullOrEmpty(s.Key)).Select((s) => s.Key).Distinct(),

                Players = Statsitics.Where(s => !string.IsNullOrEmpty(s.Player)).GroupBy(s => s.Player)
                .Select((g) => new
                {
                    Player = g.Key,

                    TotalGames = g.Count(),
                    TotalGamesCorrect = g.Where(s => s.Correct).Count(),

                    TotalPlayTime = g.Sum(s => s.TotalTime)

                }).OrderByDescending(r => r.TotalGames),

                SuccessfulPlayers = Statsitics
                .Where(s => !string.IsNullOrEmpty(s.Player))
                .GroupBy(s => s.Player)
                .Select((g) => new
                {
                    Player = g.Key,

                    TotalGames = g.Count(),
                    TotalGamesCorrect = g.Where(s => s.Correct).Count(),

                    TotalPlayTime = g.Sum(a => a.TotalTime)

                }).OrderByDescending(r => r.TotalGamesCorrect),

                SuccessfulGames = Statsitics.GroupBy(s => s.Question).Select((g) => new
                {
                    Id = g.Key.Id,
                    QuestionCode = g.Key.Code,
                    Type = g.Key.Type,

                    QuestionImage = MappingProfile.GetGeneralImageURL(g.Key),

                    TotalGames = g.Count(),
                    TotalGamesCorrect = g.Count(s => s.Correct),

                }).Where(q => q.TotalGames > 0).OrderByDescending(r => r.TotalGamesCorrect).Take(10),

                UnsuccessfulGames = Statsitics.GroupBy(s => s.Question).Select((g) => new
                {
                    Id = g.Key.Id,
                    QuestionCode = g.Key.Code,
                    Type = g.Key.Type,

                    QuestionImage = MappingProfile.GetGeneralImageURL(g.Key),

                    TotalGames = g.Count(),
                    TotalGamesIncorrect = g.Count(s => !s.Correct),

                }).Where(q => q.TotalGames > 0).OrderByDescending(r => r.TotalGamesIncorrect).Take(10),

                SolutionKeys = SKeys,

                SeriesStastics = SeriesStastics
                .OrderBy(s => s.DateCreated)
                .Select(s => new
                {
                    Series = s.Series.Code,

                    Key = s.MapKey,
                    Player = s.Player,

                    Map = s.MapName,
                    MapElement = s.MapElementName,

                    SuccessRate = s.SuccessRate,
                    TotalTime = s.TotalTime,
                    OnMobile = s.OnMobile,

                    DateCreated = s.DateCreated.Value.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                }),

                SeriesStasticsGrouped = SeriesStastics
                .GroupBy(s => s.Series)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Id = g.Key.Id,
                    Series = g.Key.Code,
                    Count = g.Count(),

                    NumberOfQuestions = g.Key.IsRandom ? g.Key.RandomSize : g.Key.Elements.Count,

                    PlayTime = g.Select(sg => sg.TotalTime),

                    TotalPlayTime = g.Sum(sg => sg.TotalTime),
                    MedianPlayTime = g.Select(sg => sg.TotalTime).OrderBy(a => a).Skip(g.Select(s => s.TotalTime).Count() / 2).FirstOrDefault()
                }),

                SeriesStasticsUniqueCount = SeriesStastics.GroupBy(s => s.Series).Count(),
                SeriesStasticsOnMobileCount = SeriesStastics.Count(s => s.OnMobile),


                MapPDFStastics = MapPDFStatistics
                .OrderBy(s => s.DateCreated)
                .Select(s => new
                {
                    Player = s.Player,
                    Map = s.Element.Map.Title,
                    MapElement = s.Element.Title,
                    OnMobile = s.OnMobile,

                    DateCreated = s.DateCreated.Value.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                }),

                MapPDFStasticsGrouped = MapPDFStatistics
                .GroupBy(s => s.Element.Map)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Id = g.Key.Id,

                    Map = g.Key.Title,
                    Count = g.Count()
                }),

                MapPDFStasticsUniqueMapCount = MapPDFStatistics.GroupBy(s => s.Element.Map).Count(),

                To = To.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                From = From.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),

                DataPoolId = VM.SearchDatapoolId

            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetGraphicalStudentReport([FromBody] ReportQueryViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            DateTime From;
            DateTime To;

            try
            {
                From = DateTime.ParseExact(VM.From, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                To = DateTime.ParseExact(VM.To, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                return BadRequest("Invalid date format");
            }

            var DailyQuestionStats = await _applicationDbContext.QuestionStatistic
                .Where(s =>
                ((s.DateCreated >= From) && (s.DateCreated <= To))
                && s.DataPoolId == VM.SearchDatapoolId)
                .GroupBy(s => s.DateCreated.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Date = g.Key,
                    Count = g.Count(),
                    CountCorrect = g.Count(s => s.Correct),

                })
                .ToListAsync();

            var HourlyQuestionStats = await _applicationDbContext.QuestionStatistic
                .Where(s =>
                ((s.DateCreated >= From) && (s.DateCreated <= To))
                && s.DataPoolId == VM.SearchDatapoolId)
                .GroupBy(s => s.DateCreated.Value.Hour)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Hour = g.Key,
                    Count = g.Count(),
                    CountCorrect = g.Count(s => s.Correct),

                })
                .ToListAsync();

            //Populate all dates
            var allDates = new List<DateTime>();

            for (var dt = From; dt <= To; dt = dt.AddDays(1))
            {
                allDates.Add(dt);
            }

            var DailyQuestionStatsEveryDay = allDates.Select(d =>
            {
                var countGroup = DailyQuestionStats.FirstOrDefault(g => g.Date == d);

                return new
                {
                    Date = d,
                    Count = countGroup != null ? countGroup.Count : 0,
                    CountCorrect = countGroup != null ? countGroup.CountCorrect : 0
                };
            });

            var allHours = new List<DateTime>();
            var Today_12am = DateTime.Today;
            var Tomorrow_12am = Today_12am.AddDays(1);

            for (var dt = Today_12am; dt < Tomorrow_12am; dt = dt.AddHours(1))
            {
                allHours.Add(dt);
            }

            var FinalHourlyQuestionStats = allHours.Select(h =>
            {
                var countGroup = HourlyQuestionStats.FirstOrDefault(g => g.Hour == h.Hour);

                return new
                {
                    Hour = h.Hour,
                    Count = countGroup != null ? countGroup.Count : 0,
                    CountCorrect = countGroup != null ? countGroup.CountCorrect : 0
                };
            });

            return Ok(new
            {
                DailyQuestionStatsEveryDay = DailyQuestionStatsEveryDay,
                HourlyQuestionStatsAllDays = FinalHourlyQuestionStats,
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSpecificStudentReportTimeline([FromBody] ReportQueryViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            DateTime From;
            DateTime To;

            try
            {
                From = DateTime.ParseExact(VM.From, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                To = DateTime.ParseExact(VM.To, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                return BadRequest("Invalid date format");
            }

            //Get question stats

            var qStats = await _applicationDbContext.QuestionStatistic
                .Where(s => s.DateCreated >= From && s.DateCreated <= To && s.Player == VM.Code)
                .Select((s) => new
                {
                    Type = Constants.STUDENT_TIMELINE_REPORT_OBJECT_TYPE_QUESTION,

                    QuestionId = s.QuestionId,
                    QuestionType = s.Question.Type,
                    QuestionCode = s.Question.Code,

                    ImageURL = MappingProfile.GetGeneralImageURL(s.Question),

                    DateCreated = s.DateCreated,

                    Correct = s.Correct,

                    TotalTime = s.TotalTime,
                    Score = s.Score,

                    SeriesId = 0,
                    SeriesCode = "",

                    Map = "",
                    MapElement = "",
                    MapKey = s.Key,
                })
                .ToListAsync();

            var sStats = await _applicationDbContext.QuestionSeriesStatistic
                .Where(s => s.DateCreated >= From && s.DateCreated <= To && s.Player == VM.Code)
                .Select((s) => new
                {
                    Type = Constants.STUDENT_TIMELINE_REPORT_OBJECT_TYPE_SERIES,

                    QuestionId = 0,
                    QuestionType = 0,

                    QuestionCode = "",
                    ImageURL = "",

                    DateCreated = s.DateCreated,
                    Correct = true,

                    TotalTime = s.TotalTime,
                    Score = s.SuccessRate,

                    SeriesId = s.SeriesId,
                    SeriesCode = s.Series.Code,

                    Map = s.MapName,
                    MapElement = s.MapElementName,
                    MapKey = s.MapKey,

                })
                .ToListAsync();


            qStats.AddRange(sStats);
            return Ok(qStats.OrderBy(a => a.DateCreated));
        }

    }
}
