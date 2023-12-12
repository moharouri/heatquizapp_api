using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Mapping;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using HeatQuizAPI.Utilities;
using heatquizapp_api.Models.LevelsOfDifficulty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace heatquizapp_api.Controllers.LevelOfDifficultyController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LevelOfDifficultyController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public LevelOfDifficultyController(
            ApplicationDbContext applicationDbContext,
            IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLevelsOfDifficulty()
        {
            var Levels = await _applicationDbContext.LevelsOfDifficulty
                .ToListAsync();

            return Ok(_mapper.Map<List<LevelOfDifficulty>, List<LevelOfDifficultyViewModel>>(Levels));
        }

        [HttpGet("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> GetLevelsOfDifficultyDetailed()
        {
            var Levels = await _applicationDbContext.LevelsOfDifficulty
                .Select(LOD => new
                {
                    Id = LOD.Id,
                    Name = LOD.Name,
                    HexColor = LOD.HexColor,
                    NUsedQuestions = LOD.Questions.Count,
                    CodeUsedQuestions = LOD.Questions.Select(q => q.Code)
                })
                .ToListAsync();

            return Ok(Levels);
        }


        [HttpGet("[action]/{LODId}")]
        [Authorize("admin")]
        public async Task<IActionResult> GetLevelOfDifficultyQuestions(int LODId)
        {
            //Check lod exists
            var level = await _applicationDbContext.LevelsOfDifficulty
                .Include(l => l.Questions)
                .ThenInclude(q => q.Subtopic)
                .ThenInclude(s => s.Topic)

                .Include(l => l.Questions)
                .ThenInclude(q => q.DataPool)

                .Include(l => l.Questions)
                .ThenInclude(q => q.AddedBy)

                .FirstOrDefaultAsync(l => l.Id == LODId);

            if (level is null)
                return NotFound("Level of difficulty not found");

            //Get questions
            var qs = level.Questions
                .Select(q => new {
                    Id = q.Id,
                    Code = q.Code,
                    Type = q.Type,

                    DatapoolName = q.DataPool.NickName,
                    DatapoolId = q.DataPool.Id,

                    DateCreated = q.DateCreated,

                    ImageURL = MappingProfile.GetQuestionImageURL(q),

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

                }).OrderBy(q => q.Code).ToList();

            return Ok(qs);
        }

        [HttpPost("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> AddLevel([FromBody] AddEditLevelOfDifficultyViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be Empty");

            //Check HexColor not null
            if (string.IsNullOrEmpty(VM.HexColor))
                return BadRequest("Color can't be Empty");

            //Check name not taken
            var nameTaken = await _applicationDbContext.LevelsOfDifficulty
                .AnyAsync(l => l.Name == VM.Name);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Check color not taken
            var colorTaken = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.HexColor == VM.HexColor);

            if (colorTaken != null)
                return BadRequest($"Color {VM.HexColor} taken by level {colorTaken.Name}, choose a different color");

            //Create and add
            var Level = new LevelOfDifficulty()
            {
                Name = VM.Name,
                HexColor = VM.HexColor
            };

            _applicationDbContext.LevelsOfDifficulty.Add(Level);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize("admin")]
        public async Task<IActionResult> EditLevel([FromBody] AddEditLevelOfDifficultyViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check level exists
            var Level = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.Id == VM.Id);

            if (Level is null)
                return NotFound("Level of difficulty not found");

            //Check name not null
            if (string.IsNullOrEmpty(VM.Name))
                return BadRequest("Name can't be empty");

            //Check HexColor not null
            if (string.IsNullOrEmpty(VM.HexColor))
                return BadRequest("Color can't be empty");

            //Check name not taken
            var nameTaken = await _applicationDbContext.LevelsOfDifficulty
                .AnyAsync(l => l.Id != VM.Id && l.Name == VM.Name);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Check color not taken
            var colorTaken = await _applicationDbContext.LevelsOfDifficulty
                .FirstOrDefaultAsync(l => l.Id != VM.Id && l.HexColor == VM.HexColor);

            if (colorTaken != null)
                return BadRequest($"Color {VM.HexColor} taken by level {colorTaken.Name}, choose a different color");
            
            //Update
            Level.Name = VM.Name;
            Level.HexColor = VM.HexColor;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("[action]/{Id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteLevel(int Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check level exists
            var Level = await _applicationDbContext.LevelsOfDifficulty
                .Include(l => l.Questions)
                .FirstOrDefaultAsync(l => l.Id == Id);

            if (Level is null)
                return NotFound("Level of difficulty not found");

            if (Level.Questions.Count != 0)
                return BadRequest("Cannot delete Level of difficulty since some questions assigned to it");

            _applicationDbContext.LevelsOfDifficulty.Remove(Level);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
