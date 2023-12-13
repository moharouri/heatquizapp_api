using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.Keyboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HeatQuizAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using static heatquizapp_api.Utilities.Utilities;
using heatquizapp_api.Models;

namespace heatquizapp_api.Controllers.KeyListController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
    [ApiController]
    [Authorize]
    public class KeyListController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public KeyListController(
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
        public async Task<IActionResult> GetAllKeyLists([FromBody] DatapoolCarrierViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);


            var KeysLists = await _applicationDbContext.KeysLists
                .Where(kl => kl.DataPoolId == VM.DatapoolId)

                .Include(a => a.AddedBy)

                .Include(a => a.VariableKeys)
                .Include(a => a.NumericKeys)

                .ToListAsync();

            return Ok(_mapper.Map<List<KeysList>, List<KeysListViewModel>>(KeysLists));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetListAssignedKeys([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var KeysList = await _applicationDbContext.KeysLists
                .Include(a => a.AddedBy)

                .Include(a => a.VariableKeys)
                .Include(a => a.NumericKeys)

                .FirstOrDefaultAsync(a => a.Id == VM.Id);

            if (KeysList is null)
                return NotFound("List not found");

            return Ok(_mapper.Map<KeysList, KeysListViewModel>(KeysList));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddList([FromBody] AddKeysListViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeExists = await _applicationDbContext.KeysLists
                .AnyAsync(i => i.Code == VM.Code);

            if (codeExists)
                return BadRequest("Code already exists");

            //Get adder
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            var List = new KeysList()
            {
                Code = VM.Code,
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

            _applicationDbContext.KeysLists.Add(List);
            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("[action]")]
        //Change type in vs code
        public async Task<IActionResult> RemoveKeyList([FromBody] KeysListViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check key list exists
            var KeysList = await _applicationDbContext.KeysLists
                .Include(a => a.AddedBy)
                .Include(a => a.VariableKeys)
                .Include(a => a.NumericKeys)
                .FirstOrDefaultAsync(a => a.Id == VM.Id);

            if (KeysList is null)
                return BadRequest("List not found");

            if (KeysList.VariableKeys.Any() || KeysList.NumericKeys.Any())
                return BadRequest("Cannot delete keys list since it is assigned to alteast one key");

            _applicationDbContext.KeysLists.Remove(KeysList);

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ReassignKeys([FromBody] KeysListViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check keys list
            var KeysList = await _applicationDbContext.KeysLists
                .FirstOrDefaultAsync(a => a.Id == VM.Id);

            if (KeysList is null)
                return NotFound("List not found");

            //Check keys exist and datapools are consistent
            var NKeys = await _applicationDbContext.NumericKeys
                .Where(k => VM.NumericKeys.Any(a => a.Id == k.Id))
                .ToListAsync();

            var VKeys = await _applicationDbContext.VariableKeys
                .Where(k => VM.VariableKeys.Any(a => a.Id == k.Id))
                .ToListAsync();

            if (!NKeys.Any() && !VKeys.Any())
                return BadRequest("Please select keys");

            var KeyDatapools = NKeys.Select(a => a.DataPoolId).ToList();
            KeyDatapools.AddRange(VKeys.Select(a => a.DataPoolId));

            if (KeyDatapools.Distinct().Count() != 1)
                return BadRequest("Datapool inconsistency");

            if (KeyDatapools.FirstOrDefault() != KeysList.DataPoolId)
                return BadRequest("Datapool inconsistency");

            //Update
            foreach (var k in NKeys)
            {
                k.KeysListId = KeysList.Id;
            }

            foreach (var k in VKeys)
            {
                k.KeysListId = KeysList.Id;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCode([FromForm] UpdateKeysListCodeViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check list exists
            var List = await _applicationDbContext.KeysLists
                .FirstOrDefaultAsync(l => l.Id == VM.ListId);

            if (List is null)
                return NotFound("List not found");

            //Check if code is null
            if (string.IsNullOrEmpty(VM.Code))
                return BadRequest("Code cannot be empty");

            var codeExists = await _applicationDbContext.KeysLists
                .AnyAsync(i => i.Code == VM.Code && i.DataPoolId == DP.Id);

            if (codeExists)
                return BadRequest("Code already exists");

            List.Code = VM.Code;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


    }
}
