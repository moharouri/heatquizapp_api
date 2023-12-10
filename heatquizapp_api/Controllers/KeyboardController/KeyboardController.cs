using AutoMapper;
using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Keyboard;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeatQuizAPI.Utilities;
using static heatquizapp_api.Utilities.Utilities;

namespace heatquizapp_api.Controllers.KeyboardController
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KeyboardController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public KeyboardController(
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
        //Change type in vs code
        public async Task<IActionResult> GetQuestionsSpecificKeyboard([FromBody] SearchKeyboardSpecificQuestionsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Keyboard = await _applicationDbContext.Keyboards

               .Include(k => k.KeyboardQuestions)
               .ThenInclude(q => q.Subtopic)
               .ThenInclude(st => st.Topic)

               .Include(k => k.KeyboardQuestions)
               .ThenInclude(q => q.LevelOfDifficulty)

               .Include(k => k.KeyboardQuestions)
               .ThenInclude(q => q.AddedBy)

               .FirstOrDefaultAsync(k => k.Id == VM.Id);

            if (Keyboard is null)
                return NotFound("Not found");

            var KeyboardQuestions = Keyboard.KeyboardQuestions;


            return Ok(new
            {
                KeyboardQuestions = _mapper.Map<List<KeyboardQuestion>, List<KeyboardQuestionViewModel>>(KeyboardQuestions),

            });

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddKeyboardNumericKey([FromBody] KeyboardNumericKeyViewModel KeyVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == KeyVM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check code not null
            if (string.IsNullOrEmpty(KeyVM.Code))
                return BadRequest("Code can't be empty");

            //Check LaTeX code not null
            if (string.IsNullOrEmpty(KeyVM.TextPresentation))
                return BadRequest("Latex Code Can't Be Empty");

            //Check code not taken
            var codeTaken = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id)
                ||
                            await _applicationDbContext.VariableKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code Taken, Choose Different Code");

            //Check Latex Code Unique 
            var latexCodeUsed = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.TextPresentation == KeyVM.TextPresentation && k.DataPoolId == DP.Id);

            if (latexCodeUsed)
                return BadRequest($"Latex already used");

            var List = await _applicationDbContext.KeysLists
                .FirstOrDefaultAsync(l => l.Id == KeyVM.KeysListId && l.DataPoolId == DP.Id);

            if (List is null)
                return BadRequest("List not found");

            //Check Numeric Key IsInteger
            var IsInteger = int.TryParse(KeyVM.TextPresentation, out _);

            //Create Key
            var Key = new KeyboardNumericKey()
            {
                Code = KeyVM.Code,
                TextPresentation = KeyVM.TextPresentation,
                IsInteger = IsInteger,
                
                KeysListId = List.Id,
                DataPoolId = DP.Id
            };


            _applicationDbContext.NumericKeys.Add(Key);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<KeyboardNumericKey, KeyboardNumericKeyViewModel>(Key));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddKeyboardVariableKey([FromBody] KeyboardVariableKeyViewModel KeyVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == KeyVM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check Code not Null
            if (string.IsNullOrEmpty(KeyVM.Code))
                return BadRequest("Code Can't Be Empty");

            //Check LATEX Code not Null
            if (string.IsNullOrEmpty(KeyVM.TextPresentation))
                return BadRequest("Latex Code Can't Be Empty");

            //Check LaTeX code not null
            if (string.IsNullOrEmpty(KeyVM.TextPresentation))
                return BadRequest("Latex Code Can't Be Empty");

            //Check code not taken
            var codeTaken = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id)
                ||
                            await _applicationDbContext.VariableKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code Taken, Choose Different Code");

            //Check Latex Code Unique 
            var latexCodeUsed = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.TextPresentation == KeyVM.TextPresentation && k.DataPoolId == DP.Id);

            if (latexCodeUsed)
                return BadRequest($"Latex already used");

            var List = await _applicationDbContext.KeysLists
                .FirstOrDefaultAsync(l => l.Id == KeyVM.KeysListId && l.DataPoolId == DP.Id);

            if (List is null)
                return BadRequest("List not found");
           
            //Check All Variables Have Valid Values
            if (!KeyVM.Variations.Any())
                return BadRequest("Please provide values for variations");

            //Create Key
            var Key = new KeyboardVariableKey()
            {
                Code = KeyVM.Code,
                TextPresentation = KeyVM.TextPresentation,

                Variations = KeyVM.Variations.Select(image => new KeyboardVariableKeyVariation()
                {
                    TextPresentation = image.TextPresentation,
                    DataPoolId = DP.Id
                }).ToList(),

                KeysListId = List.Id,
                DataPoolId = DP.Id
            };

            _applicationDbContext.VariableKeys.Add(Key);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<KeyboardVariableKey, KeyboardVariableKeyViewModel>(Key));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddKeyboard([FromBody] KeyboardViewModel KeyboardVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool exists
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == KeyboardVM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            //Check name not null
            if (string.IsNullOrEmpty(KeyboardVM.Name))
                return BadRequest("Name can't be empty");

            //Check Name Not Taken 
            var nameTaken = await _applicationDbContext.Keyboards
                .AnyAsync(k => k.Name == KeyboardVM.Name);

            if (nameTaken)
                return BadRequest("Name Taken, Choose Different Name");

            //Get adder
            var adder = await getCurrentUser(_contextAccessor, _userManager);

            //Create Keyboard
            var Keyboard = new Keyboard()
            {
                Name = KeyboardVM.Name,
                AddedById = adder.Id,
                DataPoolId = DP.Id
            };

            //Check keys exist
            var NKeys = await _applicationDbContext.NumericKeys
                .Where(k => KeyboardVM.NumericKeys.Any(kvm => k.Id == kvm.Id))
                .ToListAsync();

            if (NKeys.Count != KeyboardVM.NumericKeys.Distinct().Count())
                return BadRequest("Alteast One numeric key doest not exist");

            //Check Keys Exist
            var VKeys = await _applicationDbContext.VariableKeys
                .Include(k => k.Variations)
                .Where(k => KeyboardVM.VariableKeys.Any(kvm => k.Id == kvm.Id))
                .ToListAsync();

            if (VKeys.Count != KeyboardVM.VariableKeys.Distinct().Count())
                return BadRequest("Alteast one variable key doest not exist");

            //Try to get replacement char
            var uniqueReplacementChar = GetKeyboardReplacementChars();

            var index = 0;
            foreach (var key in KeyboardVM.NumericKeys)
            {
                string uniqueChar = null;

                try
                { 
                    uniqueChar = uniqueReplacementChar[index];
                }
                catch {}

                if (uniqueChar is null)
                    return BadRequest($"Failed to find a replacement character for a key");

                Keyboard.NumericKeys.Add(new KeyboardNumericKeyRelation() 
                {
                    NumericKeyId = key.Id,
                    Order = key.Order,
                    KeySimpleForm = uniqueChar,

                    DataPoolId = DP.Id,
                });

                //Update index;
                index += 1;
            }

            index = KeyboardVM.NumericKeys.Count;

            foreach (var key in KeyboardVM.VariableKeys)
            {
                Keyboard.VariableKeys.Add(new KeyboardVariableKeyRelation()
                {
                    VariableKeyId = key.Id,
                    Order = key.Order,
                    DataPoolId = DP.Id
                });

                var VKey = VKeys.FirstOrDefault(k => k.Id == key.Id);

                foreach(var variation in VKey.Variations)
                {
                    string uniqueChar = null;

                    try
                    {
                        uniqueChar = uniqueReplacementChar[index];
                    }
                    catch { }

                    if (uniqueChar is null)
                        return BadRequest($"Failed to find a replacement character for a key");

                    Keyboard.VariableKeyImages.Add(new KeyboardVariableKeyImageRelation()
                    {
                        VariationId = variation.Id,
                        ReplacementCharacter = uniqueChar,
                        DataPoolId = DP.Id
                    });

                    //Update index;
                    index += 1;
                }

            }

            _applicationDbContext.Keyboards.Add(Keyboard);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Keyboard, KeyboardViewModel>(Keyboard));
        }

        [HttpPut("[action]")]
        //Change type in vs code
        public async Task<IActionResult> EditKeyboardName([FromBody] KeyboardViewModel KeyboardVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check new name not null
            if (string.IsNullOrEmpty(KeyboardVM.Name))
                return BadRequest("Name cannot be null");

            //Check name not taken 
            var nameTaken = await _applicationDbContext.Keyboards
                .AnyAsync(k => k.Name == KeyboardVM.Name && k.Id != KeyboardVM.Id);

            if (nameTaken)
                return BadRequest("Name taken, choose different name");

            //Get Keyboard
            var Keyboard = await _applicationDbContext.Keyboards
                .FirstOrDefaultAsync(k => k.Id == KeyboardVM.Id);

            if (Keyboard is null)
                return NotFound("Keyboard not found");

            //Update name
            Keyboard.Name = KeyboardVM.Name;

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddKeyboardVariableKeyVariation([FromBody] KeyboardVariableKeyVariationViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            var Key = await _applicationDbContext.VariableKeys
                .Include(k => k.Variations)
                .FirstOrDefaultAsync(k => k.Id == VM.KeyId && k.DataPoolId == DP.Id);

            if (Key is null)
                return NotFound("Key not found");

            if (Key.Variations.Any(i => i.TextPresentation == VM.TextPresentation && i.DataPoolId == DP.Id))
                return BadRequest("Already Exists");

            var Image = new KeyboardVariableKeyVariation()
            {
                TextPresentation = VM.TextPresentation,
                DataPoolId = DP.Id
            };

            Key.Variations.Add(Image);

            var Keyboards = await _applicationDbContext.Keyboards
                .Include(k => k.VariableKeyImages)
                .Include(k => k.NumericKeys)
                .Where(k => k.VariableKeys.Any(vk => vk.VariableKeyId == Key.Id))
                .ToListAsync();

            foreach (var keyboard in Keyboards)
            {
                //Try to get replacement char
                var uniqueReplacementChar = GetUniqueKeyboardReplacementChar(keyboard);

                if (uniqueReplacementChar is null)
                    return BadRequest($"Cannot add key variation; keyboard {keyboard.Name} has too many keys");

                keyboard.VariableKeyImages.Add(new KeyboardVariableKeyImageRelation()
                {
                    ReplacementCharacter = uniqueReplacementChar,
                    DataPoolId = DP.Id
                });
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        //Change type in vs code
        //Try optimize
        public async Task<IActionResult> SwabKeyboardKeys([FromBody] SwabKeyboardKeysViewModel KeyboardVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check keyboard exists
            var Keyboard = await _applicationDbContext.Keyboards
                .Include(k => k.NumericKeys)
                .Include(k => k.VariableKeys)
                .FirstOrDefaultAsync(k => k.Id == KeyboardVM.KeyboardId);

            if (Keyboard is null)
                return NotFound("Keyboard not found");

            if (KeyboardVM.IsFirstNumeric)
            {
                var First = Keyboard.NumericKeys.FirstOrDefault((fk) => fk.NumericKeyId == KeyboardVM.FirstKeyId);

                if (First is null)
                    return BadRequest("Key not found");

                if (KeyboardVM.IsSecondNumeric)
                {
                    var Second = Keyboard.NumericKeys.FirstOrDefault((fk) => fk.NumericKeyId == KeyboardVM.SecondKeyId);

                    if (Second is null)
                        return BadRequest("Key not found");

                    var FirstOrder = First.Order;

                    First.Order = Second.Order;
                    Second.Order = FirstOrder;
                }
                else
                {
                    var Second = Keyboard.VariableKeys.FirstOrDefault((fk) => fk.VariableKeyId == KeyboardVM.SecondKeyId);

                    if (Second is null)
                        return BadRequest("Key not found");

                    var FirstOrder = First.Order;

                    First.Order = Second.Order;
                    Second.Order = FirstOrder;
                }
            }
            else
            {
                var First = Keyboard.VariableKeys.FirstOrDefault((fk) => fk.VariableKeyId == KeyboardVM.FirstKeyId);

                if (First is null)
                    return BadRequest("Key not found");

                if (KeyboardVM.IsSecondNumeric)
                {
                    var Second = Keyboard.NumericKeys.FirstOrDefault((fk) => fk.NumericKeyId == KeyboardVM.SecondKeyId);

                    if (Second is null)
                        return BadRequest("Key not found");

                    var FirstOrder = First.Order;

                    First.Order = Second.Order;
                    Second.Order = FirstOrder;
                }
                else
                {
                    var Second = Keyboard.VariableKeys.FirstOrDefault((fk) => fk.VariableKeyId == KeyboardVM.SecondKeyId);

                    if (Second is null)
                        return BadRequest("Key not found");

                    var FirstOrder = First.Order;

                    First.Order = Second.Order;
                    Second.Order = FirstOrder;
                }
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Keyboard, KeyboardViewModel>(Keyboard));
        }

        [HttpPost("[action]")]
        //CHECK LATER
        public async Task<IActionResult> GetQuestionsForKeyboardKey()
        {
            return Ok();
        }


    }
}
