using HeatQuizAPI.Database;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using heatquizapp_api.Models.ClickImageTrees;
using heatquizapp_api.Models.InterpretedTrees;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;
using heatquizapp_api.Models.Questions.MultipleChoiceQuestion;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using HeatQuizAPI.Utilities;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using static heatquizapp_api.Utilities.Utilities;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Series;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using heatquizapp_api.Models.Topics;
using heatquizapp_api.Models.QuestionInformation;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using heatquizapp_api.Models.CourseMapElementImages;
using heatquizapp_api.Models.Keyboard;
using System.Runtime.Intrinsics.Arm;

namespace heatquizapp_api.Controllers.StudentsController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly string AdderId = "7d630ef7-de69-4f2b-b320-65ec9ae2426a";
        public StudentsController(
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

        private T executeHTTPRequest<T>(string url)
        {
            var client = new RestClient("http://167.86.98.171:6001/api/");
            var request = new RestRequest(url);
            var response = client.ExecuteGet<T>(request);

            return response.Data;
        }

        private async Task<Stream> ConvertToStream(string fileUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            try
            {
                MemoryStream mem = new MemoryStream();
                Stream stream = response.GetResponseStream();

                stream.CopyTo(mem, 4096);

                return mem;
            }
            finally
            {
                response.Close();
            }
        }

        private async Task<string> readSaveFileFromURL(string url, string fileType)
        {
            var stream = await ConvertToStream(url);

            var file = new FormFile(stream, 0, stream.Length, null, "file" + fileType);

            if (file != null)
            {
                string fileURL = await SaveFile(file);

                return fileURL;
            }

            return null;

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadTopics()
        {
            var topics = executeHTTPRequest<List<Topic>>("Topic/GetAllTopics/2");

            foreach(var topic in topics)
            {
                var newTopic = new Topic()
                {
                    AddedById = AdderId,
                    Name = topic.Name,
                    DataPoolId = 1,
                };

                _applicationDbContext.Topics.Add(newTopic);
                await _applicationDbContext.SaveChangesAsync();

                foreach(var s in topic.Subtopics)
                {
                    var newSubtopic = new Subtopic()
                    {
                        Name = s.Name,
                        DataPoolId = 1,
                        TopicId = newTopic.Id
                    };

                    _applicationDbContext.Subtopics.Add(newSubtopic);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadInformation()
        {
            var info = executeHTTPRequest<List<Information>>("Information/GetAllInformation/2");

            foreach (var i in info)
            {
                var newInfo = new Information()
                {
                    AddedById = AdderId,
                    Code = i.Code,
                    Latex = i.Latex,
                    DataPoolId = 1,
                    
                };

                _applicationDbContext.Information.Add(newInfo);
                await _applicationDbContext.SaveChangesAsync(); 
            }

            return Ok();
        }

        public class InterpretedImageGroupViewModelLocal
        {
            public string Name { get; set; }

            public List<InterpretedImageViewModelLocal> Images { get; set; } = new List<InterpretedImageViewModelLocal>();

        }

        public class InterpretedImageViewModelLocal
        {
            public string Code { get; set; }

            //Left
            public int LeftId { get; set; }

            //Right
            public int RightId { get; set; }

            //Ratio
            public int RationOfGradientsId { get; set; }

            //Jump
            public int JumpId { get; set; }

            //Image
            public string URL { get; set; }
            public long Size { get; set; }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadInterpretedTrees()
        {
            var trees = executeHTTPRequest<List<InterpretedImageGroupViewModelLocal>>("InterpretedImages/GetGroups/2");

            foreach (var t in trees)
            {
                var newTree = new InterpretedImageGroup()
                {
                    AddedById = AdderId,
                    Name = t.Name,
                    DataPoolId = 1,
                };

                _applicationDbContext.InterpretedImageGroups.Add(newTree);
                await _applicationDbContext.SaveChangesAsync();

                foreach(var image in t.Images)
                {
                    var imageURL = await readSaveFileFromURL(image.URL, ".png");

                    var newImage = new InterpretedImage()
                    {
                        Code = image.Code,
                        DataPoolId = 1,
                        GroupId = newTree.Id,
                        
                        LeftId = image.LeftId,
                        RationOfGradientsId = image.RationOfGradientsId,
                        JumpId = image.JumpId,
                        RightId = image.RightId,

                        ImageURL = imageURL
                    };

                    _applicationDbContext.InterpretedImages.Add(newImage);

                    await _applicationDbContext.SaveChangesAsync();
                }
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadClickTrees()
        {
            var trees = executeHTTPRequest<List<ImageAnswerGroupViewModel>>("ImageAnswers/GetImageAnswerGroups/2");

            foreach (var t in trees)
            {
                var newTree = new ImageAnswerGroup()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,
                    Name = t.Name,
                };

                _applicationDbContext.ImageAnswerGroups.Add(newTree);
                await _applicationDbContext.SaveChangesAsync();

                foreach(var img1  in t.Images)
                {
                    var imageURL1 = await readSaveFileFromURL(img1.URL, ".png");

                    var newImage = new ImageAnswer()
                    {
                        Name = img1.Name,
                        DataPoolId = 1,
                        Size = img1.Size,
                        ImageURL = imageURL1,
                        GroupId = newTree.Id  
                    };

                    _applicationDbContext.ImageAnswers.Add(newImage);   
                    await _applicationDbContext.SaveChangesAsync();

                    foreach(var img2 in img1.Leafs)
                    {
                        var imageURL2 = await readSaveFileFromURL(img2.URL, ".png");

                        var newImage2 = new ImageAnswer()
                        {
                            Name = img2.Name,
                            DataPoolId = 1,
                            Size = img2.Size,
                            ImageURL = imageURL2,
                            RootId = newImage.Id,
                            GroupId = newTree.Id
                            
                        };

                        _applicationDbContext.ImageAnswers.Add(newImage2);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                }
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadMapPopup()
        {
            var imgs = executeHTTPRequest<List<CourseMapElementImagesViewModel>>("CourseMapElementImages/GetAllImages/2");
            
            foreach(var img in imgs)
            {
                var playURL = await readSaveFileFromURL(img.Play, ".png");
                var linkURL = await readSaveFileFromURL(img.Link, ".png");
                var pdfURL = await readSaveFileFromURL(img.PDF, ".png");
                var videoURL = await readSaveFileFromURL(img.Video, ".png");

                var newImage = new CourseMapElementImages()
                {
                    DataPoolId = 1,
                    Code = img.Code,
                    AddedById = AdderId,

                    Play = playURL,
                    Link = linkURL,
                    Video = videoURL,
                    PDF = pdfURL,
                    
                };

                _applicationDbContext.CourseMapElementImages.Add(newImage);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadCourses()
        {
            var courses = executeHTTPRequest<List<CourseViewModel>>("Course/GetAllCourses_PORTAL/2");

            foreach (var c in courses)
            {
                var url = await readSaveFileFromURL(c.URL, ".png");

                var newCourse = new Course()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = c.Code,
                    Name = c.Name,

                    ImageSize = c.Size,
                    ImageURL = url
                };

                _applicationDbContext.Courses.Add(newCourse);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadKeyLists()
        {
            var lists = executeHTTPRequest<List<KeysListViewModel>>("KeysList/GetAllKeyLists/2");

            foreach (var l in lists)
            {

                var newCourse = new KeysList()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = l.Code,

                };

                _applicationDbContext.KeysLists.Add(newCourse);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadNKeys()
        {
            var nKeys = executeHTTPRequest<List<KeyboardNumericKeyViewModel>>("Keyboard/GetAllNumericKeys/2");

            foreach(var n in nKeys)
            {
                var list = await _applicationDbContext.KeysLists.FirstOrDefaultAsync(k => k.Code == n.KeysList.Code);

                var newKey = new KeyboardNumericKey()
                {
                    Code = n.Code,
                    DataPoolId = 1,
                    IsInteger = n.IsInteger,
                    TextPresentation = n.TextPresentation,
                    KeysListId = list.Id,
                };

                _applicationDbContext.NumericKeys.Add(newKey);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadVKeys()
        {
            var vKeys = executeHTTPRequest<List<KeyboardVariableKeyViewModel>>("Keyboard/GetAllVariableKeys/2");

            foreach (var v in vKeys)
            {
                var list = await _applicationDbContext.KeysLists.FirstOrDefaultAsync(k => k.Code == v.KeysList.Code);

                var newKey = new KeyboardVariableKey()
                {
                    Code = v.Code,
                    DataPoolId = 1,
                    KeysListId = list.Id,

                    TextPresentation = v.TextPresentation,

                    Variations = v.VImages.Select(x => new KeyboardVariableKeyVariation()
                    {
                        DataPoolId = 1,

                        TextPresentation = x
                    }).ToList()
                };

                _applicationDbContext.VariableKeys.Add(newKey); 

                await _applicationDbContext.SaveChangesAsync(); 

            }

            return Ok();
        }

        public class KeyboardViewModelLOCAL : BaseEntityViewModel
        {
            public string Name { get; set; }

            public List<KeyValuePair<int, string>> NumericKeys { get; set; } = new List<KeyValuePair<int, string>>();

            //Variable Keys
            public List<KeyValuePair<int, string>> VariableKeys { get; set; } = new List<KeyValuePair<int, string>>();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadKeyboards()
        {
            var keyboards = executeHTTPRequest<List<KeyboardViewModelLOCAL>>("Keyboard/GetAllKeyboards/2");

            foreach (var keyboard in keyboards)
            {
                var newKeyboard = new Keyboard()
                {
                    Name = keyboard.Name,
                    DataPoolId = 1, 
                    AddedById = AdderId,
                };

                //Try to get replacement char
                var uniqueReplacementChar = GetKeyboardReplacementChars();

                var index = 0;
                foreach (var key in keyboard.NumericKeys)
                {
                    var k = await _applicationDbContext.NumericKeys.FirstOrDefaultAsync(a => a.Code == key.Value);

                    string uniqueChar = null;

                    try
                    {
                        uniqueChar = uniqueReplacementChar[index];
                    }
                    catch { }

                    if (uniqueChar is null)
                        return BadRequest($"Failed to find a replacement character for a key");

                    newKeyboard.NumericKeys.Add(new KeyboardNumericKeyRelation()
                    {
                        NumericKeyId = k.Id,
                        Order = key.Key,

                        KeySimpleForm = uniqueChar,

                        DataPoolId = 1,
                    });

                    //Update index;
                    index += 1;
                }

                index = keyboard.NumericKeys.Count;

                foreach (var key in keyboard.VariableKeys)
                {
                    var k = await _applicationDbContext.VariableKeys
                        .Include(a => a.Variations)
                        .FirstOrDefaultAsync(a => a.Code == key.Value);

                    newKeyboard.VariableKeys.Add(new KeyboardVariableKeyRelation()
                    {
                        VariableKeyId = k.Id,
                        Order = key.Key,
                        DataPoolId = 1
                    });

                    var VKey = k;

                    foreach (var variation in VKey.Variations)
                    {
                        string uniqueChar = null;

                        try
                        {
                            uniqueChar = uniqueReplacementChar[index];
                        }
                        catch { }

                        if (uniqueChar is null)
                            return BadRequest($"Failed to find a replacement character for a key");

                        newKeyboard.VariableKeyImages.Add(new KeyboardVariableKeyImageRelation()
                        {
                            VariationId = variation.Id,
                            ReplacementCharacter = uniqueChar,
                            DataPoolId = 1
                        });

                        //Update index;
                        index += 1;
                    }

                }

                _applicationDbContext.Keyboards.Add(newKeyboard);

                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadMCQs()
        {
            var allqs = await _applicationDbContext.MultipleChoiceQuestions.ToListAsync();

            _applicationDbContext.MultipleChoiceQuestions.RemoveRange(allqs);
            await _applicationDbContext.SaveChangesAsync();

            var qs = executeHTTPRequest<List<MultipleChoiceQuestionViewModel>>("MultipleChoiceQuestion/GetAllQuestions/2");

            foreach(var q in qs)
            {
                var subtopic = await _applicationDbContext.Subtopics.FirstOrDefaultAsync(s => s.Name == q.Subtopic.Name);
                var lod = await _applicationDbContext.LevelsOfDifficulty.FirstOrDefaultAsync(l => l.Name == q.LevelOfDifficulty.Name);

                var imageURL = await readSaveFileFromURL(q.ImageURL, ".png");
                var pdfURL = "";

                if (!string.IsNullOrEmpty(q.PDFURL))
                {
                    pdfURL = await readSaveFileFromURL(q.PDFURL, ".pdf");
                }

                var newQ = new MultipleChoiceQuestion()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = q.Code,
                    LevelOfDifficultyId = lod.Id,
                    SubtopicId = subtopic.Id,

                    Latex = q.Latex,
                    Type = Constants.MUTLIPLE_CHOICE_QUESTION_PARAMETER,

                    ImageURL = imageURL,
                    PDFURL = pdfURL,
                    
                };

                foreach(var c in q.Choices)
                {
                    var imgURL = "";

                    if (!string.IsNullOrEmpty(c.ImageURL))
                    {
                        imgURL = await readSaveFileFromURL(c.ImageURL, ".png");
                    };


                    var newChoice = new MultipleChoiceQuestionChoice()
                    {
                        Correct = c.Correct,
                        Latex = c.Latex,
                        DataPoolId = 1,
                        ImageURL = imgURL
                    };

                    newQ.Choices.Add(newChoice);
                };

                _applicationDbContext.MultipleChoiceQuestions.Add(newQ);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FixMCQs()
        {
            var allqs = await _applicationDbContext.MultipleChoiceQuestions
                .Include(q => q.Choices)
                .ToListAsync();

            foreach(var q in allqs)
            {
                foreach(var c in q.Choices)
                {
                    if (string.IsNullOrEmpty(c.ImageURL))
                    {
                        c.ImageURL = null;
                    }
                }
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadCIQs()
        {
            var allqs = await _applicationDbContext.SimpleClickableQuestions.ToListAsync();

            _applicationDbContext.SimpleClickableQuestions.RemoveRange(allqs);
            await _applicationDbContext.SaveChangesAsync();

            var qs = executeHTTPRequest<List<SimpleClickableQuestionViewModel>>("SimpleClickable/GetAllQuestions/2");

            foreach (var q in qs)
            {
                var subtopic = await _applicationDbContext.Subtopics.FirstOrDefaultAsync(s => s.Name == q.Subtopic.Name);
                var lod = await _applicationDbContext.LevelsOfDifficulty.FirstOrDefaultAsync(l => l.Name == q.LevelOfDifficulty.Name);
                int? infoId = null;

                if(q.Information != null)
                {
                    infoId = (await _applicationDbContext.Information.FirstOrDefaultAsync(a => a.Code == q.Information.Code)).Id;
                }

                var imageURL = await readSaveFileFromURL(q.ImageURL, ".png");
                var pdfURL = "";

                if (!string.IsNullOrEmpty(q.PDFURL))
                {
                    pdfURL = await readSaveFileFromURL(q.PDFURL, ".pdf");
                }

                var newQuestion = new SimpleClickableQuestion()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = q.Code,
                    SubtopicId = subtopic.Id,
                    LevelOfDifficultyId = lod.Id,

                    Type = Constants.CLICKABLE_QUESTION_PARAMETER,

                    ImageWidth = q.ImageWidth,
                    ImageHeight = q.ImageHeight,

                    ImageURL = imageURL,
                    PDFURL = pdfURL,
                };

                foreach(var ci in q.ClickImages)
                {
                    var answer = await _applicationDbContext.ImageAnswers
                        .FirstOrDefaultAsync(a => a.Name == ci.Answer.Name && a.Group.Name == a.Group.Name);

                    var newCI = new ClickImage()
                    {
                        DataPoolId = 1,

                        X = ci.X,
                        Y = ci.Y,

                        Height = ci.Height,
                        Width = ci.Width,

                        AnswerId = answer.Id

                    };

                    newQuestion.ClickImages.Add(newCI);
                }

                foreach (var ci in q.ClickCharts)
                {
                    var answer = await _applicationDbContext.InterpretedImages.FirstOrDefaultAsync(a => a.Code == ci.Answer.Code && a.Group.Name == ci.Answer.Group.Name);

                    var newCC = new ClickChart()
                    {
                        DataPoolId = 1,

                        X = ci.X,
                        Y = ci.Y,

                        Height = ci.Height,
                        Width = ci.Width,

                        AnswerId = answer.Id

                    };

                    newQuestion.ClickCharts.Add(newCC);

                }

                _applicationDbContext.SimpleClickableQuestions.Add(newQuestion);
                await _applicationDbContext.SaveChangesAsync();

            }

            return Ok();
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> ReadKQs()
        {
            var allqs = await _applicationDbContext.KeyboardQuestion.ToListAsync();

            _applicationDbContext.KeyboardQuestion.RemoveRange(allqs);
            await _applicationDbContext.SaveChangesAsync();

            var qs = executeHTTPRequest<List<KeyboardQuestionViewModel>>("KeyboardQuestion/GetAllQuestions/2");

            foreach (var q in qs)
            {
                var subtopic = await _applicationDbContext.Subtopics.FirstOrDefaultAsync(s => s.Name == q.Subtopic.Name);
                var lod = await _applicationDbContext.LevelsOfDifficulty.FirstOrDefaultAsync(l => l.Name == q.LevelOfDifficulty.Name);
               
                var keyboard = await _applicationDbContext.Keyboards
                     .Include(k => k.NumericKeys)
                     .ThenInclude(nk => nk.NumericKey)

                     .Include(k => k.VariableKeys)
                     .ThenInclude(vk => vk.VariableKey)
                     .ThenInclude(vk => vk.Variations)

                     .Include(k => k.VariableKeyImages)
                     .ThenInclude(vk => vk.Variation)
                    .FirstOrDefaultAsync(k => k.Name == q.Keyboard.Name);

                var imageURL = await readSaveFileFromURL(q.ImageURL, ".png");
                var pdfURL = "";

                if (!string.IsNullOrEmpty(q.PDFURL))
                {
                    pdfURL = await readSaveFileFromURL(q.PDFURL, ".pdf");
                }

                var newKQuestion = new KeyboardQuestion()
                {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = q.Code,
                    SubtopicId = subtopic.Id,
                    LevelOfDifficultyId = lod.Id,

                    
                    ImageURL = imageURL,
                    PDFURL = pdfURL,

                    Type = Constants.KEYBOARD_QUESTION_PARAMETER,

                    Latex = q.Latex,
                   
                    IsEnergyBalance = q.IsEnergyBalance,
                    DisableDivision = q.DisableDivision,
                    
                    KeyboardId = keyboard.Id,

                };

                _applicationDbContext.KeyboardQuestion.Add(newKQuestion);
                await _applicationDbContext.SaveChangesAsync();

                foreach(var answer in q.Answers)
                {
                    var newAnswer = new KeyboardQuestionAnswer()
                    {
                        DataPoolId = 1,

                    };

                    newKQuestion.Answers.Add(newAnswer);
                    await _applicationDbContext.SaveChangesAsync();

                    foreach (var ae in answer.AnswerElements)
                    {
                        if(ae.NumericKey.Id != 0)
                        {
                            var nk = keyboard.NumericKeys.FirstOrDefault(nk => nk.NumericKey.Code == ae.NumericKey.NumericKey.Code);

                            newAnswer.AnswerElements.Add(new KeyboardQuestionAnswerElement()
                            {
                                NumericKeyId = nk.Id,

                                Value = nk.KeySimpleForm,

                                DataPoolId = 1,

                                
                            });

                            await _applicationDbContext.SaveChangesAsync();

                        }

                        else if (ae.Image.Id != 0)
                        {
                            var vkImage = keyboard.VariableKeyImages.FirstOrDefault(a => a.Variation.TextPresentation == ae.Image.Variation.TextPresentation);

                            newAnswer.AnswerElements.Add(new KeyboardQuestionAnswerElement()
                            {
                                ImageId = vkImage.Id,

                                DataPoolId = 1,

                                Value = vkImage.ReplacementCharacter

                            });

                            await _applicationDbContext.SaveChangesAsync();
                        }
                        else
                        {
                            newAnswer.AnswerElements.Add(new KeyboardQuestionAnswerElement()
                            {
                                DataPoolId = 1,

                                Value = ae.Value

                            });

                            await _applicationDbContext.SaveChangesAsync();
                        }

                    }
                }

                await _applicationDbContext.SaveChangesAsync();

            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadSeries()
        {

            var allSeries = await _applicationDbContext.QuestionSeries.ToListAsync();

            _applicationDbContext.QuestionSeries.RemoveRange(allSeries);
            await _applicationDbContext.SaveChangesAsync();

            var Series = executeHTTPRequest<List<QuestionSeriesViewModel>>("QuestionSeries/GetAllSeries/2");


            foreach(var s in Series)
            {
                var newSeries = new QuestionSeries() {
                    DataPoolId = 1,
                    AddedById = AdderId,

                    Code = s.Code,
                    IsRandom = s.IsRandom,
                    NumberOfPools = s.NumberOfPools,
                    RandomSize = s.RandomSize,
                };

                foreach(var e in s.Elements)
                {
                    var Q = await _applicationDbContext.QuestionBase.FirstOrDefaultAsync(q => q.Code == e.Question.Code);

                    if (Q is null) continue;

                    var newElement = new QuestionSeriesElement()
                    {
                        DataPoolId = 1,
                        PoolNumber = e.PoolNumber,
                        Order = e.Order,
                        QuestionId = Q.Id,
                        
                    };

                    newSeries.Elements.Add(newElement); 
                }

                _applicationDbContext.QuestionSeries.Add(newSeries);
                await  _applicationDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReadMaps()
        {
            var Maps = executeHTTPRequest<List<CourseMapViewModel>>("CourseMap/GetAllMaps/2");

            foreach(var map in Maps)
            {
                var imageURL = await readSaveFileFromURL(map.LargeMapURL, ".png");

                var course = await _applicationDbContext.Courses.FirstOrDefaultAsync(c => c.Name == map.Course.Name);

                var newMap = new CourseMap()
                {
                    DataPoolId = 1,
                    CourseId = course.Id,

                    Disabled = map.Disabled,
                    ShowBorder = map.ShowBorder,
                    ShowSolutions = map.ShowBorder,

                    ImageURL = imageURL,
                    ImageWidth = map.LargeMapWidth,
                    ImageHeight = map.LargeMapLength,

                    Title = map.Title,
                };

                //Elements
                foreach(var element in map.Elements)
                {
                    int? seriesId = null;
                    string pdfURL = null;

                    if (element.QuestionSeries != null)
                    {
                        seriesId = (await _applicationDbContext.QuestionSeries.FirstOrDefaultAsync(s => s.Code == element.QuestionSeries.Code)).Id;
                    }

                    if (!string.IsNullOrEmpty(element.PDFURL))
                    {
                        pdfURL = await readSaveFileFromURL(element.PDFURL, ".pdf");
                    }

                    var newElement = new CourseMapElement()
                    {
                        DataPoolId = 1,

                        Title = element.Title,

                        ExternalVideoLink = element.ExternalVideoLink,
                        
                        QuestionSeriesId = seriesId,

                        PDFURL = pdfURL,

                        X = element.X,
                        Y = element.Y,
                        Width = element.Width,
                        Length = element.Length,

                        BadgeLength = element.Badge_Length,
                        BadgeWidth = element.Badge_Width,
                        BadgeX = element.Badge_X,
                        BadgeY = element.Badge_Y,

                        Threshold = element.Threshold,

                        
                    };

                    //Badges 
                    foreach(var b in element.Badges)
                    {
                        var bURL = await readSaveFileFromURL(b.URL, ".png");

                        var newBadgeE = new CourseMapElementBadge()
                        {
                            DataPoolId = 1,
                            Progress = b.Progress,
                            ImageURL = bURL
                        };

                        newElement.Badges.Add(newBadgeE);
                    }

                    newMap.Elements.Add(newElement);
                }

                //Badge systems 
                foreach(var bs in map.BadgeSystems)
                {
                    var newBS = new CourseMapBadgeSystem()
                    {
                        DataPoolId = 1,

                        Title = bs.Title
                    };

                    foreach(var e in bs.Entities)
                    {
                        var eURL = await readSaveFileFromURL(e.URL, ".png");

                        var newEntity = new CourseMapBadgeSystemEntity()
                        {
                            DataPoolId = 1,
                            Progress = e.Progress,
                            ImageURL = eURL
                        };

                        newBS.Entities.Add(newEntity);
                    }

                    newMap.BadgeSystems.Add(newBS);
                }

                _applicationDbContext.CourseMap.Add(newMap);
                await _applicationDbContext.SaveChangesAsync();

                foreach(var e in map.Elements.Where(e => e.RequiredElement != null))
                {
                    var addedElement = map.Elements.FirstOrDefault(ae => ae.Title == e.Title);

                    var requiredElement = map.Elements.FirstOrDefault(ae => ae.Title == e.RequiredElement.Title);

                    addedElement.RequiredElementId = requiredElement.Id;
                }

                await _applicationDbContext.SaveChangesAsync();

            }



            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> AddMapRelations()
        {
            var Maps = executeHTTPRequest<List<CourseMapViewModel>>("CourseMap/GetAllMaps/2");

            foreach (var map in Maps)
            {
                var addedMap = await _applicationDbContext.CourseMap.
                     Include(m => m.Elements)
                     .FirstOrDefaultAsync(m => m.Title == map.Title);

                foreach (var e in map.Elements.Where(e => e.RequiredElement != null))
                {
                    var addedElement = addedMap.Elements.FirstOrDefault(ae => ae.Title == e.Title);

                    var requiredElement = addedMap.Elements.FirstOrDefault(ae => ae.Title == e.RequiredElement.Title);

                    addedElement.RequiredElementId = requiredElement.Id;
                }

                await _applicationDbContext.SaveChangesAsync();

            }



            return Ok();
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetClickableQuestion(int Id)
        {
            //Get Question
            var Question = await _applicationDbContext.SimpleClickableQuestions

                .Include(q => q.ClickImages)
                .ThenInclude(ci => ci.Answer)
                .ThenInclude(ci => ci.Root)

                .Include(q => q.ClickImages)
                .ThenInclude(ci => ci.Answer)

                .Include(q => q.ClickCharts)
                .ThenInclude(cc => cc.Answer)

                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");


            Question.ClickImages = Question.ClickImages.OrderBy(a => a.Id).ToList();
            Question.ClickCharts = Question.ClickCharts.OrderBy(a => a.Id).ToList();

            //Click trees
            var UsedClickTreesIds = Question.ClickImages.Where(a => a.Answer.GroupId.HasValue).Select(i => i.Answer.GroupId).ToList();
            UsedClickTreesIds.AddRange(Question.ClickImages.Where(a => a.Answer.RootId.HasValue).Select(i => i.Answer.Root.GroupId).ToList());

            var ImageAnswerGroups = await _applicationDbContext.ImageAnswerGroups
                .Where(g => UsedClickTreesIds.Any(Id => Id == g.Id))
                .Include(g => g.Images)
                .ToListAsync();

            //Interpreted trees
            var UsedChartTreesIds = Question.ClickCharts.Select(i => i.Answer.GroupId).ToList();

            var InterpretedImageGroups = await _applicationDbContext.InterpretedImageGroups
                .Where(g => UsedChartTreesIds.Any(Id => Id == g.Id))
                .Include(g => g.Images)
                .ToListAsync();

            return Ok(
                new
                {
                    Question = _mapper.Map<SimpleClickableQuestion, SimpleClickableQuestionViewModel>(Question),

                    ClickTrees = _mapper.Map<List<ImageAnswerGroup>, List<ImageAnswerGroupViewModel>>(ImageAnswerGroups),

                    ChartTrees = _mapper.Map<List<InterpretedImageGroup>, List<InterpretedImageGroupViewModel>>(InterpretedImageGroups)
                });
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetKeyboardQuestion(int Id)
        {
            var Question = await _applicationDbContext.KeyboardQuestion

                .Include(q => q.Keyboard)
                .ThenInclude(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)

                .Include(q => q.Keyboard)
                .ThenInclude(k => k.VariableKeyImages)
                .ThenInclude(vk => vk.Variation)

                .Include(q => q.Answers)
                .ThenInclude(a => a.AnswerElements)
                .ThenInclude(e => e.NumericKey)
                .ThenInclude(k => k.NumericKey)

                .Include(q => q.Answers)
                .ThenInclude(a => a.AnswerElements)
                .ThenInclude(e => e.Image)
                .ThenInclude(e => e.Variation)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");

            foreach (var a in Question.Answers)
            {
                a.AnswerElements = a.AnswerElements.OrderBy(e => e.Id).ToList();
            }

            return Ok(_mapper.Map<KeyboardQuestion, KeyboardQuestionViewModel>(Question));
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetMultipleChoiceQuestion(int Id)
        {
            //Get question
            var Question = await _applicationDbContext.MultipleChoiceQuestions

                .Include(q => q.Choices)

                .Include(q => q.Information)

                .FirstOrDefaultAsync(q => q.Id == Id);

            if (Question is null)
                return NotFound("Question not found");

            return Ok(_mapper.Map<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>(Question));
        }

        [HttpGet("[action]/{Code}")]
        public async Task<IActionResult> GetSeriesPlayByCode(string Code)
        {
            var Series = await _applicationDbContext.QuestionSeries

                .Include(s => s.Elements)
                .ThenInclude(e => e.Question)
                .ThenInclude(q => q.Information)

                .FirstOrDefaultAsync(s => s.Code == Code);

            if (Series is null)
                return NotFound("Series not found");

            //Send elements in order
            Series.Elements = Series.Elements.OrderBy(e => e.Order).ToList();

            return Ok(_mapper.Map<QuestionSeries, QuestionSeriesViewModel>(Series));
        }

        [HttpGet("[action]/{Id}")]
        public async Task<IActionResult> GetCourseMapPlayById(int Id)
        {
            var Course = await _applicationDbContext.CourseMap

                .Include(c => c.Elements)
                .ThenInclude(e => e.QuestionSeries)
                .ThenInclude(s => s.Elements)

                .Include(c => c.Elements)
                .ThenInclude(e => e.RequiredElement)
                .ThenInclude(e => e.QuestionSeries)
                .ThenInclude(s => s.Elements)

                .Include(c => c.Elements)
                .ThenInclude(e => e.Badges)

                 .Include(c => c.Elements)
                .ThenInclude(e => e.CourseMapElementImages)

                .Include(c => c.Elements)
                .ThenInclude(e => e.MapAttachment)
                .ThenInclude(a => a.Map)
                .FirstOrDefaultAsync(c => c.Id == Id && !c.Disabled);

            if (Course is null)
                return NotFound("Map not found");

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Course));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionStatistic([FromBody] AddQuestionStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var Question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");

            //Add statistic
            Question.QuestionStatistics.Add(new QuestionStatistic()
            {
                Correct = VM.Correct,
                Score = VM.Score,
                TotalTime = VM.TotalTime,
                Key = VM.Key,
                Player = VM.Player,
                DataPoolId = Question.DataPoolId
            });

             
            //Try adding linked key if the user is registed 
            //Check if the user wants to have his statistics posted
            try
            {
                var registered_user = await getCurrentUser(_contextAccessor, _userManager);
                var player_key = VM.Player;

                var key_exists = await _applicationDbContext.UserLinkedPlayerKeys
                    .AnyAsync(k => k.UserId == registered_user.Id && k.PlayerKey == player_key);

                if (!key_exists)
                {
                    _applicationDbContext.UserLinkedPlayerKeys.Add(new UserLinkedPlayerKey()
                    {
                        PlayerKey = player_key,
                        UserId = registered_user.Id,
                        DateCreated = DateTime.Now
                    });
                }
            }
            catch
            {

            }

            if (Question.Type == Constants.KEYBOARD_QUESTION_PARAMETER && !VM.Correct)
            {
                var KQuestion = await _applicationDbContext.KeyboardQuestion
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

                if(KQuestion != null)
                {
                    KQuestion.WrongAnswers.Add(new KeyboardQuestionWrongAnswer()
                    {
                        AnswerLatex = VM.Latex,

                        DataPoolId = KQuestion.DataPoolId
                    });
                }
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddQuestionPDFStatistic([FromBody] AddQuestionPDFStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check question exists
            var question = await _applicationDbContext.QuestionBase
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (question is null)
                return NotFound("Question not found");

            //Add statistc
            question.QuestionPDFStatistics.Add(new QuestionPDFStatistic()
            {
                Player = VM.Player,
                Correct = VM.Correct,
                DataPoolId = question.DataPoolId

            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> AddStudentFeedback([FromForm] AddQuestionFeedbackViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Question = await _applicationDbContext.QuestionBase
                .Include(q => q.StudentFeedback)
                .FirstOrDefaultAsync(q => q.Id == VM.QuestionId);

            if (Question is null)
                return NotFound("Question not found");


            if (string.IsNullOrEmpty(VM.Feedback) || VM.Feedback.Length > 500)
                return BadRequest("Please provide proper feedback");

            var player_exists = await _applicationDbContext.QuestionStatistic
                .AnyAsync(s => s.Player == VM.Player);

            if (!player_exists)
                return BadRequest("Player never played a game");

            var feedback = new QuestionStudentFeedback()
            {
                Player = VM.Player,
                QuestionId = Question.Id,
                FeedbackContent = VM.Feedback,
                DataPoolId = Question.DataPoolId
            };

            _applicationDbContext.QuestionStudentFeedback.Add(feedback);
            await _applicationDbContext.SaveChangesAsync();

            return Ok("Success");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddSeriesStatistic([FromForm] AddSeriesStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check series exists
            var Series = await _applicationDbContext.QuestionSeries
               .FirstOrDefaultAsync(q => q.Id == VM.SeriesId);

            if (Series is null)
                return NotFound("Series not found");

            //Add statistic
            Series.Statistics.Add(new QuestionSeriesStatistic()
            {
                Player = VM.Player,
                MapKey = VM.MapKey,
                MapName = VM.MapName,
                MapElementName = VM.MapElementName,
                SuccessRate = VM.SuccessRate,

                TotalTime = VM.TotalTime,
                DataPoolId = Series.DataPoolId,
                OnMobile = VM.OnMobile,
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetRecentlyVisitedCourseMapsByIds([FromBody] GetMapsByIdsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Maps = await _applicationDbContext.CourseMap
                .Where(c => VM.Ids.Any(a => a == c.Id))
                .ToListAsync();

            var MapsOrdered = new List<CourseMap>();

            foreach (var Id in VM.Ids)
            {
                var map = Maps.FirstOrDefault(a => a.Id == Id);

                if (map != null)
                {
                    MapsOrdered.Add(map);
                }
            }

            MapsOrdered.Reverse();

            return Ok(_mapper.Map<List<CourseMapViewModel>>(MapsOrdered));
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddMapPDFStatistic([FromForm] AddMapPDFStatisticViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var element = await _applicationDbContext.CourseMapElement
                .FirstOrDefaultAsync(q => q.Id == VM.ElementId);

            if (element is null)
                return NotFound("Element not found");

            element.PDFStatistics.Add(new CourseMapPDFStatistics()
            {
                Player = VM.Player,
                OnMobile = VM.OnMobile,

                DataPoolId = element.DataPoolId
            });

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
