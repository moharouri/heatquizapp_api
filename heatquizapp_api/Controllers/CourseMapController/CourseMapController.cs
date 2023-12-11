using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;
using System.Runtime.Intrinsics.Arm;

namespace heatquizapp_api.Controllers.CourseMapController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseMapController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public CourseMapController(
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
        //Change type and name in vs code -- original: GetCourseMapPlayById_PORTAL
        public async Task<IActionResult> GetCourseMapViewEditById([FromBody] CourseMapViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Course = await _applicationDbContext.CourseMap
                .Include(m => m.Course)

                .Include(c => c.Elements)

                .Include(c => c.Elements)
                .ThenInclude(e => e.QuestionSeries)

                .Include(c => c.Elements)
                .ThenInclude(e => e.RequiredElement)

                .Include(c => c.Elements)
                .ThenInclude(e => e.Badges)

                .Include(m => m.BadgeSystems)
                .ThenInclude(s => s.Entities)

                .Include(c => c.Elements)
                .ThenInclude(e => e.CourseMapElementImages)
           
                .Include(c => c.Elements)
                .ThenInclude(e => e.MapAttachment)
                .ThenInclude(a => a.Map)

                .FirstOrDefaultAsync(c => c.Id == VM.Id);

            if (Course is null)
                return NotFound("Map Not Found");

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Course));
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
        public async Task<IActionResult> AddMap([FromForm] AddCourseMapSingleStepViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Dataopool not found");

            //Check Course Exists
            var Course = await _applicationDbContext.Courses
                .Include(c => c.CourseMaps)
                .FirstOrDefaultAsync(c => c.Id == VM.CourseId && c.DataPoolId == DP.Id);

            if (Course is null)
                return NotFound("Course not found");

            if (Course.CourseMaps.Any(m => m.Title == VM.Title))
                return BadRequest("Course already has a map with same name");
           
            var Elements = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CourseMapElementViewModel>>(VM.ElementsString);

            if (Elements is null)
                return BadRequest("Invalid data of map elements");

            if (Elements.Count == 0)
                return BadRequest("Please add elements");

            //Check selected series exist
            var Series = await _applicationDbContext.QuestionSeries
                .Where(s => Elements.Any(e => e.QuestionSeriesId == s.Id && s.DataPoolId == DP.Id))
                .ToListAsync();

            if (Series.Count != Elements.Where(e => e.QuestionSeriesId != null).Select(e => e.QuestionSeriesId).Distinct().Count())
                return BadRequest("Some questions series do not exist");

            //Picture
            if (VM.Picture is null)
                return Ok("Please provide picture");

            //Verify extension
            var isExtenstionValid = validateImageExtension(VM.Picture);

            if (!isExtenstionValid)
                return BadRequest("Picture extension not valid");

            //Save image and generate url
            var URL = await SaveFile(VM.Picture);

            var Map = new CourseMap()
            {
                Title = VM.Title,

                ShowBorder = VM.ShowBorder,
                CourseId = Course.Id,

                ImageURL = URL,
                ImageSize = VM.Picture.Length,

                ImageWidth = (int)VM.LargeMapWidth,
                ImageHeight = (int)VM.LargeMapLength,

                DataPoolId = DP.Id
            };

            Map.Elements.AddRange(Elements.Select(e => new CourseMapElement()
            {
                Title = e.Title,

                QuestionSeriesId = e.QuestionSeriesId,
                ExternalVideoLink = e.ExternalVideoLink,

                X = e.X,
                Y = e.Y,

                Width = e.Width,
                Length = e.Length,

                DataPoolId = DP.Id
            }));


            Course.CourseMaps.Add(Map);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditMapBasicInfo([FromBody] CourseMapViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get map
            var Map = await _applicationDbContext.CourseMap
                .Include(m => m.Course)
                .ThenInclude(c => c.CourseMaps)
                .Include(c => c.Elements)
                .FirstOrDefaultAsync(m => m.Id == VM.Id);

            if (Map is null)
                return NotFound();

            //Check name not repeated
            if (Map.Course.CourseMaps.Any(m => m.Id != Map.Id && m.Title == VM.Title))
                return BadRequest($"Title already exists in course {Map.Course.Name}");

            //Update
            Map.Title = VM.Title;
            Map.ShowBorder = VM.ShowBorder;
            Map.Disabled = VM.Disabled;
            Map.ShowSolutions = VM.ShowSolutions;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> ReassignMap([FromForm] ReassignCourseMapViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get map
            var Map = await _applicationDbContext.CourseMap
              .FirstOrDefaultAsync(m => m.Id == VM.MapId);

            if (Map is null)
                return NotFound("Map not found");

            //Get Course
            var Course = await _applicationDbContext.Courses
                .FirstOrDefaultAsync(c => c.Id == VM.CourseId);

            if (Course is null)
                return NotFound("Course not found");

            if (Course.DataPoolId != Map.DataPoolId)
                return BadRequest("Datapool mismatch");

            Map.CourseId = Course.Id;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Map));
        }


        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> DeleteElement([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                 .Include(e => e.Badges)
                 .Include(e => e.Map)
                .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");

            _applicationDbContext.CourseMapElement.Remove(Element);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AttachMapToElement([FromForm] MapElementLinkViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .Include(e => e.MapAttachment)
                .FirstOrDefaultAsync(e => e.Id == VM.ElementId);

            if (Element is null)
                return NotFound("Element not found");

            //Get map
            var Map = await _applicationDbContext.CourseMap
              .FirstOrDefaultAsync(m => m.Id == VM.MapId && m.DataPoolId == Element.DataPoolId);

            if (Map is null)
                return NotFound("Map not found");

            if (Map.Id == Element.MapId)
                return NotFound("Cannot attach the element's parent map to it");

            if (Element.MapAttachment != null)
            {
                _applicationDbContext.MapElementLink.Remove(Element.MapAttachment);

                await _applicationDbContext.SaveChangesAsync();
            }

            Element.MapAttachment = new MapElementLink()
            {
                ElementId = Element.Id,
                MapId = Map.Id
            };

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapElementViewModel>(Element));
        }

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> DeattachMapToElement([FromForm] MapElementLinkViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .Include(e => e.MapAttachment)
                .FirstOrDefaultAsync(e => e.Id == VM.ElementId);

            if (Element is null)
                return NotFound("Element not found");

            if (Element.MapAttachment != null)
            {
                _applicationDbContext.MapElementLink.Remove(Element.MapAttachment);

                await _applicationDbContext.SaveChangesAsync();
            }


            return Ok(_mapper.Map<CourseMapElementViewModel>(Element));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddBadgeGroup([FromForm] AddCourseMapBadgeGroupSingleStepViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get map
            var Map = await _applicationDbContext.CourseMap
                .Include(m => m.BadgeSystems)
                .FirstOrDefaultAsync(m => m.Id == VM.MapId);

            if (Map is null)
                return NotFound("Map not found");

            //Check title unique
            if (Map.BadgeSystems.Any(s => s.Title.ToUpper() == VM.Title.ToUpper()))
                return BadRequest("Title should be unique");

            if (!VM.Pictures.Any())
                return BadRequest("Please provide images");

            if (!VM.ProgressList.Any())
                return BadRequest("Please provide progress data");

            //Check data consistency
            if (VM.ProgressList.Any(p => p < 0))
                return BadRequest("Progress must be zero or positive");

            if (VM.ProgressList.Distinct().Count() != VM.ProgressList.Count)
                return BadRequest("Repeated progress value!");

            if (VM.ProgressList.Distinct().Count() != VM.Pictures.Count)
                return BadRequest("Please provide picture for each progress value and vice versa!");

            //Create badge system
            var System = new CourseMapBadgeSystem()
            {
                Title = VM.Title,
                DataPoolId = Map.DataPoolId
            };

            var index = 0;
            foreach (var Picture in VM.Pictures)
            {
                var isExtesionValid = validateImageExtension(Picture);

                if (!isExtesionValid)
                    return BadRequest("All files should be pictures be extensions of .jpg, .jpeg, .png, .gif");

                var URL = await SaveFile(Picture);
                var progress = VM.ProgressList[index];

                System.Entities.Add(new CourseMapBadgeSystemEntity()
                {
                    ImageURL = URL,
                    Progress = progress,
                    DataPoolId = Map.DataPoolId
                });

                index += 1;
            }
       
            Map.BadgeSystems.Add(System);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapViewModel>(Map));
        }

        [HttpPost("[action]")]
        //Edit request from in vs code
        public async Task<IActionResult> AddBadgeGroupEntities([FromForm] AddBadgeGroupEntitiesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get badge system
            var Group = await _applicationDbContext.CourseMapBadgeSystem
               .Include(s => s.Entities)

               .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Group is null)
                return NotFound("Badge system not found");

            //Check data consistencies
            if (!VM.Pictures.Any())
                return BadRequest("Please provide images");

            if (!VM.ProgressList.Any())
                return BadRequest("Please provide progress data");

            if (VM.ProgressList.Any(p => p < 0))
                return BadRequest("Progress must be zero or positive");

            if (VM.ProgressList.Distinct().Count() != VM.ProgressList.Count)
                return BadRequest("Repeated progress value !");

            if (VM.ProgressList.Distinct().Count() != VM.Pictures.Count)
                return BadRequest("Please provide picture for each progress value and vice versa!");

            if (Group.Entities.Any(e => VM.ProgressList.Any(p => e.Progress == p)))
                return BadRequest("Progress value already exists!");

            var index = 0;
            foreach (var Picture in VM.Pictures)
            {
                var isExtesionValid = validateImageExtension(Picture);

                if (!isExtesionValid)
                    return BadRequest("All files should be pictures be extensions of .jpg, .jpeg, .png, .gif");

                var URL = await SaveFile(Picture);
                var progress = VM.ProgressList[index];

                Group.Entities.Add(new CourseMapBadgeSystemEntity()
                {
                    ImageURL = URL,
                    Progress = progress,
                    DataPoolId = Group.DataPoolId
                });

                index += 1;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapBadgeSystemViewModel>(Group));
        }

        [HttpPost("[action]")]
        //Edit request from in vs code
        public async Task<IActionResult> RemoveBadgeGroup([FromBody] CourseMapBadgeSystemViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get badge system
            var Group = await _applicationDbContext.CourseMapBadgeSystem
               .Include(s => s.Entities)
               .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Group is null)
                return NotFound("Badge system not found");

            _applicationDbContext.CourseMapBadgeSystem.Remove(Group);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditBadgeGroup([FromForm] UpdateCourseMapBadgeGroupViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get Badge System
            var Group = await _applicationDbContext.CourseMapBadgeSystem
               .Include(s => s.Map)
               .ThenInclude(m => m.BadgeSystems)
               .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Group is null)
                return NotFound("Badge system not found");

            if (string.IsNullOrEmpty(VM.Title))
                return BadRequest("Title can't be empty");

            //Check if title is unique
            if (Group.Map.BadgeSystems.Any(s => s.Title == VM.Title))
                return BadRequest("Repeated title");

            //Update
            Group.Title = VM.Title;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapBadgeSystemViewModel>(Group));
        }

        [HttpDelete("[action]")]
        //Change type in vs code and request form
        public async Task<IActionResult> RemoveBadgeGroupEntity([FromBody] CourseMapBadgeSystemEntityViewModel VM) 
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get badge entity
            var Entity = await _applicationDbContext.CourseMapBadgeSystemEntity
                .Include(e => e.System)
               .FirstOrDefaultAsync(s => s.Id == VM.Id);

            if (Entity is null)
                return NotFound("Badge entity Not Found");

            var Group = Entity.System;

            _applicationDbContext.CourseMapBadgeSystemEntity.Remove(Entity);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapBadgeSystemViewModel>(Group));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditBadgeGroupEntity([FromForm] AddEditMapBadgeEntityViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get badge system
            var Group = await _applicationDbContext.CourseMapBadgeSystem
               .Include(s => s.Entities)
               .FirstOrDefaultAsync(s => s.Entities.Any(e => e.Id == VM.BadgeEntityId));

            if (Group is null)
                return NotFound("Badge entity not found");

            var Entity = Group.Entities.FirstOrDefault(e => e.Id == VM.BadgeEntityId);

            if (VM.Picture is null && !VM.Progress.HasValue)
                return BadRequest("Provide an Image or progress Value");

            if (VM.Progress.HasValue)
            {
                //Check progress consistencies
                if (VM.Progress < 0 || VM.Progress > 100)
                    return BadRequest("Provide correct progress value");

                if (Group.Entities.Any(e => e.Id != Entity.Id && e.Progress == VM.Progress.Value))
                    return BadRequest("Progress already exists");

                //Update
                Entity.Progress = VM.Progress.Value;

                await _applicationDbContext.SaveChangesAsync();
            }

            if (VM.Picture != null)
            {
                //Check extension
                var isExtensionValid = validateImageExtension(VM.Picture);  
                var validExtenstions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

                if (!isExtensionValid)
                    return BadRequest("Picture has an invalid extension");

                //Save image and generate url
                var URL = await SaveFile(VM.Picture);

                Entity.ImageURL = URL;
                Entity.ImageSize = VM.Picture.Length;

                await _applicationDbContext.SaveChangesAsync();

                //Try remove existing file
                var removalResult = true;

                if (Entity.ImageURL != null)
                {
                    removalResult = RemoveFile(Entity.ImageURL);
                }

                if (!removalResult)
                    return Ok("Updated, but failed to delete existing image");
            }


            return Ok(_mapper.Map<CourseMapBadgeSystemViewModel>(Group));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> SetElementSeries([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");

            //Get series
            var Series = await _applicationDbContext.QuestionSeries
                .FirstOrDefaultAsync(s => (VM.QuestionSeriesId.HasValue ? s.Id == VM.QuestionSeriesId.Value : false) && s.DataPoolId == Element.DataPoolId);

            if (Series is null)
                return NotFound("Series not found");

            //Update
            Element.QuestionSeriesId = Series.Id;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> DeleteElementSeries([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");
            
            //Update
            Element.QuestionSeriesId = null;
            Element.QuestionSeries = null;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> AddEditElementPDF([FromForm] AddEditMapElementPDFViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                 .Include(e => e.Map)
                 .FirstOrDefaultAsync(e => e.Id == VM.ElementId);

            if (Element is null)
                return NotFound("Element not found");

            if (VM.PDF != null)
            {
                //Verify Extension
                var isPDFExtensionValid = validatePDFExtension(VM.PDF);

                if (!isPDFExtensionValid)
                    return BadRequest("PDF extension not valid");

                //Try delete the existing file
                var removalResult = true;

                if (Element.PDFURL != null)
                {
                    removalResult = RemoveFile(Element.PDFURL);

                }
                //PDF
                var PDFURL = await SaveFile(VM.PDF);
                Element.PDFURL = PDFURL;
                Element.PDFSize = VM.PDF.Length;

                await _applicationDbContext.SaveChangesAsync();

                if (removalResult)
                {
                    return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
                }
                else
                {
                    return Ok("Update PDF file but failed to remove existing file");
                }
            }
            else
            {
                return BadRequest("Please Provide PDF File");
            }            
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> RemoveElementPDF([FromForm] AddEditMapElementPDFViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                 .Include(e => e.Map)
                 .FirstOrDefaultAsync(e => e.Id == VM.ElementId);

            if (Element is null)
                return NotFound("Element not found");

            //Try delete the existing file
            var removalResult = true;

            if (Element.PDFURL != null)
            {
                removalResult = RemoveFile(Element.PDFURL);
            }

            Element.PDFURL = null;
            Element.PDFSize = 0;

            await _applicationDbContext.SaveChangesAsync();
            
            if (!removalResult)
                return Ok("Deattached file but failed to remove the file from storage");

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> AddEditElementRelation([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                 .Include(e => e.Map)
                 .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");

            if (!VM.RequiredElementId.HasValue)
                return BadRequest("Please provide required element");

            //Check required element exists
            var RElement = await _applicationDbContext.CourseMapElement
                .Include(e => e.RequiredElement)
                 .FirstOrDefaultAsync(e => e.Id == VM.RequiredElementId.Value);

            if (RElement is null)
                return NotFound();

            if (Element.Id == RElement.Id)
                return BadRequest("Can't assign element to itself");

            if (Element.RequiredElementId == RElement.Id)
                return BadRequest("Can't have mutual reliance of relationship");

            if (VM.Threshold <= 0)
                return BadRequest("Threshold should be positive");

            if (VM.Threshold > 100)
                return BadRequest("Threshold can't be more than 100%");

            //Update
            Element.RequiredElementId = RElement.Id;
            Element.Threshold = VM.Threshold;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> RemoveElementRelation([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                 .Include(e => e.Map)
                 .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");
            
            //Update
            Element.RequiredElementId = null;
            Element.Threshold = 0;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapElement, CourseMapElementViewModel>(Element));
        }

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> RemoveElementBadge([FromBody] CourseMapElementBadgeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Check if badge exists
            var Badge = await _applicationDbContext.CourseMapElementBadge
                .Include(b => b.CourseMapElement)
                .FirstOrDefaultAsync(b => b.Id == VM.Id);

            if (Badge is null)
                return NotFound("Badge not found");

            var Element = Badge.CourseMapElement;

            _applicationDbContext.CourseMapElementBadge.Remove(Badge);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapElement, CourseMapElementViewModel>(Element));

        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditElementBadgePercentage([FromBody] CourseMapElementBadgeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Check badge exists
            var Badge = await _applicationDbContext.CourseMapElementBadge
                .Include(e => e.CourseMapElement)
                .ThenInclude(e => e.Badges)
                .FirstOrDefaultAsync(b => b.Id == VM.Id);

            if (Badge is null)
                return NotFound("Badge not found");

            if (VM.Progress < 0)
                return BadRequest("Threshold should be zero or positive");

            if (VM.Progress > 100)
                return BadRequest("Threshold can't be more than 100%");

            if (Badge.CourseMapElement.Badges.Any(b => b.Progress == VM.Progress && b.Id != Badge.Id))
                return BadRequest("A badge with same progress target exists for this element");

            //Update
            Badge.Progress = VM.Progress;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMapElement, CourseMapElementViewModel>(Badge.CourseMapElement));

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EditElementBadge([FromForm] AddEditMapElementBadgeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Element Exists
            var Badge = await _applicationDbContext.CourseMapElementBadge
              .FirstOrDefaultAsync(b => b.Id == VM.BadgeId);

            if (Badge is null)
                return NotFound("Badge not found");

            //Picture
            if (VM.Picture is null)
                return BadRequest("Please provide an image file");

            //Verify Extension
            var isExtensionValid = validateImageExtension(VM.Picture);
            if (!isExtensionValid)
                return BadRequest("Picture extension not valid");

            //Try removing existing file
            var removalResult = true;

            if (Badge.ImageURL != null)
            {
                removalResult = RemoveFile(Badge.ImageURL);
            }

            //Save image and generate url
            var URL = await SaveFile(VM.Picture);
            Badge.ImageURL = URL;

            await _applicationDbContext.SaveChangesAsync();

            if (!removalResult)
                return Ok("Image updated but failed to delete existing image");

            return Ok(_mapper.Map<CourseMapElementBadge, CourseMapElementBadgeViewModel>(Badge));

        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditMapElementBasicInfo([FromBody] CourseMapElementViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .Include(e => e.Map)
               .FirstOrDefaultAsync(e => e.Id == VM.Id);

            if (Element is null)
                return NotFound("Element not found");

            if (string.IsNullOrEmpty(VM.Title))
                return BadRequest("Please provide title");

            //Update
            Element.Title = VM.Title;
            Element.ExternalVideoLink = VM.ExternalVideoLink;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<CourseMap, CourseMapViewModel>(Element.Map));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CopyBadgeGroupEntities([FromForm] CopyBadgesViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            //Get element
            var Element = await _applicationDbContext.CourseMapElement
                .Include(e => e.Badges)
                .FirstOrDefaultAsync(e => e.Id == VM.MapElementId);

            if (Element is null)
                return NotFound("Map element not found");

            //Get badges
            var Badges = await _applicationDbContext.CourseMapBadgeSystemEntity
                .Where(b => VM.BadgeEntityIds.Any(e => e == b.Id))
                .ToListAsync();

            if (Element.Badges.Any(b => Badges.Any(be => be.Progress == b.Progress)))
                return BadRequest("Repeated progress!");

            //Assign badges
            foreach (var Badge in Badges)
            {

                //Copy Image
                var URL = await CopyFile(Badge.ImageURL);
                
                Element.Badges.Add(new CourseMapElementBadge()
                {
                    Progress = Badge.Progress,
                    ImageURL = URL
                });
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
