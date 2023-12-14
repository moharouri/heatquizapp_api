using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Mapping;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.Keyboard;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.SearchEngineRequests;
using heatquizapp_api.Models.Series;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace heatquizapp_api.Controllers.SearchEngineController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
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
        public async Task<IActionResult> GetQuestions([FromBody] SearchQuestionsViewModel RequestVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<QuestionBase, bool>> Criteria = q =>
                //Datapool
                (RequestVM.DataPoolId == q.DataPoolId)
                &&

                //Subtopic
                (RequestVM.Subtopic == -1 || q.SubtopicId == RequestVM.Subtopic)

                &&
                //Topic
                (RequestVM.Topic == -1 || q.Subtopic.TopicId == RequestVM.Topic)

                &&
                //LOD
                (RequestVM.LevelOfDifficulty == -1 || q.LevelOfDifficultyId == RequestVM.LevelOfDifficulty)
               
                &&
                //Code
                (string.IsNullOrEmpty(RequestVM.Code) || q.Code.ToLower().Contains(RequestVM.Code.ToLower()))

                //Question types
                && (!RequestVM.ShowClickableQuestions && RequestVM.SearchBasedOnQuestionTypes ? q.Type != Constants.CLICKABLE_QUESTION_PARAMETER : true)
                && (!RequestVM.ShowKeyboardQuestions && RequestVM.SearchBasedOnQuestionTypes ? q.Type != Constants.KEYBOARD_QUESTION_PARAMETER : true)
                && (!RequestVM.ShowMultipleChoiceQuestions && RequestVM.SearchBasedOnQuestionTypes ? q.Type != Constants.MUTLIPLE_CHOICE_QUESTION_PARAMETER : true)
                

                //Search based on median time
                && 
                (!RequestVM.SearchBasedOnMedianTime 
                ||
                (
                (RequestVM.MedianTime1 <= (q.QuestionStatistics.OrderBy(qs => qs.TotalTime).Skip(q.QuestionStatistics.Count() / 2).FirstOrDefault().TotalTime))
                &&
                (RequestVM.MedianTime2 >= (q.QuestionStatistics.Any() ? q.QuestionStatistics.OrderBy(qs => qs.TotalTime).ToList()[q.QuestionStatistics.Count() / 2].TotalTime : int.MaxValue))
                ))

                //Search based on play stats
                && 
                (!RequestVM.SearchBasedOnPlayStats 
                ||
                (
                q.QuestionStatistics.Any() 
                &&
                RequestVM.MinimumQuestionPlay <= q.QuestionStatistics.Count
                &&
                RequestVM.SuccessRate1 <= (100 * q.QuestionStatistics.Count(a => a.Correct) / q.QuestionStatistics.Count)
                &&
                RequestVM.SuccessRate2 >= (100 * q.QuestionStatistics.Count(a => a.Correct) / q.QuestionStatistics.Count)
                ));

            var CodesNumbers =  _applicationDbContext.QuestionBase
              //.Where(Criteria)
              .OrderBy(q => q.Code)
              .Select(q => q.Code[0])
              .ToList();

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

            var Questions = await _applicationDbContext.QuestionBase
                 //.Where(Criteria)
                 .Include(q => q.LevelOfDifficulty)
                 .Include(q => q.Subtopic)
                 .ThenInclude(s => s.Topic)

                 .Include(q => q.QuestionStatistics)
                 .Include(q => q.QuestionPDFStatistics)

                 .OrderBy(q => q.Code)
                 .Skip(RequestVM.Page * RequestVM.QperPage)
                 .Take(RequestVM.QperPage)
                 .Select(q => new {
                     Id = q.Id,
                     Code = q.Code,
                     Type = q.Type,

                     DateCreated = q.DateCreated,

                     AddedByName = q.AddedBy.Name,

                     ImageURL = MappingProfile.GetGeneralImageURL(q) ,
                     PDFURL = MappingProfile.GetQuestionPDFURL(q),

                     TotalGames = q.QuestionStatistics.Count,
                     TotalCorrectGames = q.QuestionStatistics.Count(s => s.Correct),
                     MedianPlayTime = q.QuestionStatistics.Any() ? q.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[(int)(q.QuestionStatistics.Count() / 2)].TotalTime : 0,

                     TotalPDFViews = q.QuestionPDFStatistics.Count,
                     TotalPDFViewsWrong = q.QuestionPDFStatistics.Count(a => !a.Correct),

                     LevelOfDifficulty = new
                     {
                         Name = q.LevelOfDifficulty.Name,
                         HexColor = q.LevelOfDifficulty.HexColor,
                         Id = q.LevelOfDifficulty.Id,
                     },

                     Subtopic = new
                     {
                         Name = q.Subtopic.Name,
                         Id = q.Subtopic.Id,
                         Topic = new
                         {
                             Name = q.Subtopic.Topic.Name,
                             Id = q.Subtopic.Topic.Id
                         },
                     },

                 })
                .ToListAsync();

            var QuestionsIdsTypes = new List<KeyValuePair<int, int>>();

            QuestionsIdsTypes = await _applicationDbContext.QuestionBase
                    //.Where(Criteria)
                    .OrderBy(q => q.Code)
                    .Select((q) => new KeyValuePair<int, int>(q.Id, q.Type))
                    .ToListAsync();


            return Ok(new
            {
                NumberOfQuestions = CodesNumbers.Count,
                Questions = (Questions),
                QuestionsIdsTypes = QuestionsIdsTypes,
                Codes = Codes,
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetQuestionsByIds([FromBody] SearchQuestionsByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");


            var Questions = await _applicationDbContext.QuestionBase
                .Where(q => VM.Ids.Any(Id => Id == q.Id))

                 .Include(q => q.LevelOfDifficulty)
                 .Include(q => q.Subtopic)
                 .ThenInclude(s => s.Topic)

                 .Include(q => q.QuestionPDFStatistics)

                 .OrderBy(q => q.Code)
                 .Select(q => new {
                     Id = q.Id,
                     Code = q.Code,
                     Type = q.Type,

                     DateCreated = q.DateCreated,

                     AddedByName = q.AddedBy.Name,

                     ImageURL = MappingProfile.GetGeneralImageURL(q),
                     PDFURL = MappingProfile.GetQuestionPDFURL(q),

                     TotalGames = q.QuestionStatistics.Count,
                     TotalCorrectGames = q.QuestionStatistics.Count(s => s.Correct),
                     MedianPlayTime = q.QuestionStatistics.Any() ? q.QuestionStatistics.OrderBy(a => a.TotalTime).ToList()[(int)(q.QuestionStatistics.Count() / 2)].TotalTime : 0,

                     TotalPDFViews = q.QuestionPDFStatistics.Count,
                     TotalPDFViewsWrong = q.QuestionPDFStatistics.Count(a => !a.Correct),

                     LevelOfDifficulty = new
                     {
                         Name = q.LevelOfDifficulty.Name,
                         HexColor = q.LevelOfDifficulty.HexColor,
                         Id = q.LevelOfDifficulty.Id,
                     },

                     Subtopic = new
                     {
                         Name = q.Subtopic.Name,
                         Id = q.Subtopic.Id,
                         Topic = new
                         {
                             Name = q.Subtopic.Topic.Name,
                             Id = q.Subtopic.Topic.Id
                         },
                     },
                 })
                .ToListAsync();


            return Ok(new
            {
                NumberOfQuestions = VM.NumberOfQuestions,
                Questions = (Questions),
                QuestionsIdsTypes = VM.QuestionsIdsTypes,
                Codes = VM.Codes,

            });
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> SearchSeries([FromBody] SeriesSearchViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<QuestionSeries, bool>> criteria = (s) =>
                (
                s.DataPoolId == VM.DataPoolId &&
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
                .Where(criteria)

                .Include(s => s.Elements)
                .Include(s => s.AddedBy)
                .Include(s => s.MapElements)
                .ThenInclude(me => me.Map)
                
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
        public async Task<IActionResult> SearchSeriesByIds([FromBody] SearchByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Series = await _applicationDbContext.QuestionSeries
                .Where(q => VM.Ids.Any(Id => Id == q.Id))
                .Include(s => s.Elements)
                .Include(s => s.AddedBy)

                .Include(s => s.MapElements)
                .ThenInclude(me => me.Map)
                
                .ToListAsync();

            return Ok(new
            {
                Series = _mapper.Map<List<QuestionSeries>, List<QuestionSeriesViewModel>>(Series),

                Codes = VM.Codes,
                NumberOfObjects = VM.NumberOfObjects,
                ObjectsIds = VM.Ids,
                ObjectsCodes = VM.Codes
            });

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchKeyoards([FromBody] KeyboardsSearchViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<Keyboard, bool>> Critera = (k) =>
            (
                k.DataPoolId == VM.DataPoolId &&
                (!string.IsNullOrEmpty(VM.Code) ? k.Name.ToLower().Contains(VM.Code.ToLower()) : true) &&
                (VM.KeyLists.Any() ? k.NumericKeys.Any(nk => VM.KeyLists.Any(kl => kl == nk.NumericKey.KeysListId)) : true) &&
               (VM.KeyLists.Any() ? k.VariableKeys.Any(vk => VM.KeyLists.Any(kl => kl == vk.VariableKey.KeysListId)) : true)
            );

            var CodesNumbers = await _applicationDbContext.Keyboards
              .Where(Critera)
              .OrderBy(q => q.Name)
              .Select(q => q.Name[0])
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

            var Keyboards = await _applicationDbContext.Keyboards
                .Where(Critera)

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)
                .ThenInclude(nkk => nkk.KeysList)

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)

                .Include(k => k.VariableKeys)
                .ThenInclude(vk => vk.VariableKey)

                .Include(k => k.KeyboardQuestions)
                
                .OrderBy(q => q.Name)
                .Skip(VM.Page * VM.QperPage)
                .Take(VM.QperPage)

                .ToListAsync();

            var Ids = await _applicationDbContext.Keyboards
              .Where(Critera)
              .OrderBy(q => q.Name)
              .Select(q => q.Id)
              .ToListAsync();

            return Ok(new
            {
                Keyboards = _mapper.Map<List<Keyboard>, List<KeyboardViewModel>>(Keyboards),

                Codes = Codes,
                NumberOfObjects = CodesNumbers.Count,
                ObjectsIds = Ids,
                ObjectsCodes = Codes

            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchKeyoardsByIds([FromBody] SearchByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<Keyboard, bool>> Critera = (k) => (VM.Ids.Any(id => id == k.Id));

            var Keyboards = await _applicationDbContext.Keyboards
                .Where(Critera)

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)
                .ThenInclude(nkk => nkk.KeysList)

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)

                .Include(k => k.VariableKeys)
                .ThenInclude(vk => vk.VariableKey)

                .Include(k => k.KeyboardQuestions)

                .OrderBy(q => q.Name)

                .ToListAsync();

            return Ok(new
            {
                Keyboards = _mapper.Map<List<Keyboard>, List<KeyboardViewModel>>(Keyboards),

                Codes = VM.Codes,
                NumberOfObjects = VM.NumberOfObjects,
                ObjectsIds = VM.Ids,
                ObjectsCodes = VM.Codes

            });

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchKeys([FromBody] KeysSearchViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            Expression<Func<KeyboardNumericKey, bool>> Criteria1 = m =>
                 m.DataPoolId == VM.DataPoolId &&
               VM.GetNumeric &&
               (!string.IsNullOrEmpty(VM.Code) ? (m.Code.ToLower().Contains(VM.Code.ToLower()) || m.TextPresentation.ToLower().Contains(VM.Code.ToLower())) : true)
               &&
               (VM.ListId.HasValue ? VM.ListId == m.KeysListId : true);


            Expression<Func<KeyboardVariableKey, bool>> Criteria2 = m =>
               m.DataPoolId == VM.DataPoolId &&
              !VM.GetNumeric &&
              (!string.IsNullOrEmpty(VM.Code) ? (m.Code.Contains(VM.Code) || m.TextPresentation.Contains(VM.Code)) : true)
              &&
              (VM.ListId.HasValue ? VM.ListId == m.KeysListId : true);

            var NCodesNumbers = await _applicationDbContext.NumericKeys
              .Where(Criteria1)
              .OrderBy(q => q.Code)
              .Select(q => q.Code[0])
              .ToListAsync();

            var VCodesNumbers = await _applicationDbContext.VariableKeys
              .Where(Criteria2)
              .OrderBy(q => q.Code)
              .Select(q => q.Code[0])
              .ToListAsync();

            var Codes = new List<CodeNumberViewModel>();

            foreach (var c in NCodesNumbers)
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

            foreach (var c in VCodesNumbers)
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

            var NKeys = await _applicationDbContext.NumericKeys
                .Where(Criteria1)
                .Include(k => k.Relations)
                .ThenInclude(r => r.AnswerElements)
                .Include(k => k.KeysList)
                .OrderBy(q => q.Code)
                .Skip(VM.Page * VM.QperPage)
                .Take(VM.QperPage)
                .Select(q => new
                {
                    DateCreated = q.DateCreated,
                    Id = q.Id,
                    Code = q.Code,
                    TextPresentation = q.TextPresentation,
                    Used = q.Relations.Select(r => r.AnswerElements.Count).Sum(),
                    Type = Constants.NUMERIC_KEY_TYPE,
                    List = q.KeysList.Code,
                    VarKeys = new List<KeyboardVariableKeyVariationViewModel>()
                })
                .ToListAsync();

            var VKeys = await _applicationDbContext.VariableKeys
                .Where(Criteria2)
                .Include(k => k.Variations)
                .ThenInclude(i => i.AnswerElements)
                .Include(k => k.KeysList)
                .OrderBy(q => q.Code)
                .Skip(VM.Page * VM.QperPage)
                .Take(VM.QperPage)
                .Select(q => new
                {
                    DateCreated = q.DateCreated,
                    Id = q.Id,
                    Code = q.Code,
                    TextPresentation = q.TextPresentation,

                    Used = q.Variations.Select(r => r.AnswerElements.Count).Sum(),

                    Type = Constants.VARIABLE_KEY_TYPE,

                    List = q.KeysList.Code,

                    VarKeys = _mapper.Map<List<KeyboardVariableKeyVariationViewModel>>(q.Variations)
                })
                .ToListAsync();

            var NIds = await _applicationDbContext.NumericKeys
                .Where(Criteria1)
              .OrderBy(q => q.Code)
              .Select(q => q.Id)
              .ToListAsync();

            var VIds = await _applicationDbContext.VariableKeys
              .Where(Criteria2)
              .OrderBy(q => q.Code)
              .Select(q => q.Id)
              .ToListAsync();

            NIds.AddRange(VIds);
            NKeys.AddRange(VKeys);

            return Ok(new
            {
                Keys = NKeys,

                Codes = Codes,
                NumberOfObjects = NCodesNumbers.Count + VCodesNumbers.Count,
                ObjectsIds = NIds,
                ObjectsCodes = Codes
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchKeysByIds([FromBody] SearchByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var NKeys = await _applicationDbContext.NumericKeys
                .Where(q => VM.Type == Constants.NUMERIC_KEY_TYPE && VM.Ids.Any(Id => Id == q.Id))
                 .Include(k => k.Relations)
                 .ThenInclude(r => r.AnswerElements)
                 .Include(k => k.KeysList)
                 .OrderBy(q => q.Code)
                 .Select(q => new
                 {
                     DateCreated = q.DateCreated,
                     Id = q.Id,
                     Code = q.Code,
                     TextPresentation = q.TextPresentation,

                     Used = q.Relations.Select(r => r.AnswerElements.Count).Sum(),

                     Type = Constants.NUMERIC_KEY_TYPE,
                     List = q.KeysList.Code,

                     VarKeys = new List<KeyboardVariableKeyVariationViewModel>()
                 })
                 .ToListAsync();

            var VKeys = await _applicationDbContext.VariableKeys
                .Where(q => VM.Type == Constants.VARIABLE_KEY_TYPE && VM.Ids.Any(Id => Id == q.Id))
                .Include(k => k.Variations)
                .ThenInclude(i => i.AnswerElements)
                .Include(k => k.KeysList)
                .OrderBy(q => q.Code)
                .Select(q => new
                {
                    DateCreated = q.DateCreated,
                    Id = q.Id,
                    Code = q.Code,
                    TextPresentation = q.TextPresentation,

                    Used = q.Variations.Select(r => r.AnswerElements.Count).Sum(),

                    Type = Constants.VARIABLE_KEY_TYPE,

                    List = q.KeysList.Code,
                    VarKeys = _mapper.Map<List<KeyboardVariableKeyVariationViewModel>>(q.Variations)
                })
                .ToListAsync();

            NKeys.AddRange(VKeys);

            return Ok(new
            {
                Keys = NKeys,

                Codes = VM.Codes,
                NumberOfObjects = VM.NumberOfObjects,
                ObjectsIds = VM.Ids,
                ObjectsCodes = VM.Codes
            });

        }

    }
}
