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
using heatquizapp_api.Models;

namespace heatquizapp_api.Controllers.KeyboardController
{
    [EnableCors("CorsPolicy")]
    [Route("apidpaware/[controller]")]
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
        public async Task<IActionResult> GetKeyboard([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var Keyboard = await _applicationDbContext.Keyboards

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.NumericKey)

                .Include(k => k.NumericKeys)
                .ThenInclude(nk => nk.AnswerElements)

                .Include(k => k.VariableKeys)
                .ThenInclude(vk => vk.VariableKey)

                .Include(k => k.VariableKeyImages)
                .ThenInclude(vk => vk.AnswerElements)

                .Include(k => k.VariableKeyImages)
                .ThenInclude(vk => vk.Variation)

                .Include(k => k.KeyboardQuestions)
                .FirstOrDefaultAsync(k => k.Id == VM.Id);

            if (Keyboard is null)
                return BadRequest("Keyboard not found");

            return Ok(_mapper.Map<Keyboard, KeyboardViewModel>(Keyboard));

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
        public async Task<IActionResult> AddKeyboardNumericKey([FromBody] AddNumericKeyViewModel KeyVM)
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
                return BadRequest("Latex code can't be empty");

            //Check code not taken
            var codeTaken = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id)
                ||
                            await _applicationDbContext.VariableKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code Taken, choose different code");

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
        public async Task<IActionResult> AddKeyboardVariableKey([FromBody] AddVariableKeyViewModel KeyVM)
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
                return BadRequest("Code can't be empty");

            //Check LATEX Code not Null
            if (string.IsNullOrEmpty(KeyVM.TextPresentation))
                return BadRequest("Latex code can't be empty");

            //Check LaTeX code not null
            if (string.IsNullOrEmpty(KeyVM.TextPresentation))
                return BadRequest("Latex code can't be empty");

            //Check code not taken
            var codeTaken = await _applicationDbContext.NumericKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id)
                ||
                            await _applicationDbContext.VariableKeys
                .AnyAsync(k => k.Code == KeyVM.Code && k.DataPoolId == DP.Id);

            if (codeTaken)
                return BadRequest("Code taken, choose different code");

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

                Variations = KeyVM.Variations.Select(v => new KeyboardVariableKeyVariation()
                {
                    TextPresentation = v,
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
        public async Task<IActionResult> AddKeyboard([FromBody] AddKeyboardViewModel KeyboardVM)
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
            var UsedNKeyIds = KeyboardVM.NumericKeys.Select(x => x.Id).ToList();

            var NKeys = await _applicationDbContext.NumericKeys
                .Where(k => UsedNKeyIds.Any(Id => Id == k.Id))
                .ToListAsync();

            if (NKeys.Count != KeyboardVM.NumericKeys.Distinct().Count())
                return BadRequest("Alteast One numeric key doest not exist");

            //Check Keys Exist
            var UsedVKeyIds = KeyboardVM.VariableKeys.Select(x => x.Id).ToList();

            var VKeys = await _applicationDbContext.VariableKeys
                .Include(k => k.Variations)
                .Where(k => UsedVKeyIds.Any(Id => Id == k.Id))
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
        public async Task<IActionResult> RemoveKeyboard([FromBody] UniversalAccessByIdViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            var keyboard = await _applicationDbContext.Keyboards
                .Include(a => a.KeyboardQuestions)
                .Include(a => a.NumericKeys)
                .Include(a => a.VariableKeys)
                .Include(a => a.VariableKeyImages)

                .FirstOrDefaultAsync(a => a.Id == VM.Id);

            if (keyboard is null)
                return BadRequest("Keyboard not found");

            if (keyboard.KeyboardQuestions.Any())
                return BadRequest("Keyboard used in some questions cannot be removed");

            _applicationDbContext.Keyboards.Remove(keyboard);

            await _applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EditKeyboardName([FromBody] UpdateKeyboardNameViewModel KeyboardVM)
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

        [HttpPost("[action]")]
        public async Task<IActionResult> AssignKeysToKeyboard([FromBody] AssignKeysToKeyboardViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Datapool
            var DP = await _applicationDbContext.DataPools
               .FirstOrDefaultAsync(dp => dp.Id == VM.DataPoolId);

            if (DP is null)
                return NotFound("Datapool not found");

            var Keyboard = await _applicationDbContext.Keyboards
                .Include(k => k.NumericKeys)
                .Include(k => k.VariableKeys)
                .Include(k => k.VariableKeyImages)
                .FirstOrDefaultAsync(k => k.Id == VM.Id && k.DataPoolId == DP.Id);

            if (Keyboard is null)
                return NotFound("Not Found");

            //Check keys exist
            var UsedNKeyIds = VM.NumericKeys.Select(x => x.Id).ToList();

            var NKeys = await _applicationDbContext.NumericKeys
                .Where(k => UsedNKeyIds.Any(Id => Id == k.Id))
                .ToListAsync();

            if (NKeys.Count != VM.NumericKeys.Distinct().Count())
                return BadRequest("Alteast One numeric key doest not exist");

            //Check Keys Exist
            var UsedVKeyIds = VM.VariableKeys.Select(x => x.Id).ToList();

            var VKeys = await _applicationDbContext.VariableKeys
                .Include(k => k.Variations)
                .Where(k => UsedVKeyIds.Any(Id => Id == k.Id))
                .ToListAsync();

            if (VKeys.Count != VM.VariableKeys.Distinct().Count())
                return BadRequest("Alteast one variable key doest not exist");

            //Check Not Used Already
            if (Keyboard.NumericKeys.Any(nk => NKeys.Any(vnk => vnk.Id == nk.NumericKeyId)))
                return BadRequest("Some Keys Already Included");

            if (Keyboard.VariableKeys.Any(nk => VKeys.Any(vnk => vnk.Id == nk.VariableKeyId)))
                return BadRequest("Some Keys Already Included");

            //Try to get replacement char
            var BaseOrder = Math.Max(
                Keyboard.NumericKeys.Count != 0 ? Keyboard.NumericKeys.Max(nk => nk.Order) : 0,
                Keyboard.VariableKeys.Count != 0 ? Keyboard.VariableKeys.Max(nk => nk.Order) : 0
                );

            var index = 0;
            foreach(var nk in VM.NumericKeys.OrderBy(nk => nk.Order))
            {
                var uniqueReplacementChar = GetUniqueKeyboardReplacementChar(Keyboard);

                if (uniqueReplacementChar is null)
                    return BadRequest($"Cannot add key variation; keyboard {Keyboard.Name} has too many keys");

                var newKey = new KeyboardNumericKeyRelation()
                {
                    NumericKeyId = nk.Id,
                    KeySimpleForm = uniqueReplacementChar,
                    Order = BaseOrder + index + 1,
                    DataPoolId = DP.Id
                };

                Keyboard.NumericKeys.Add(newKey);

                index++;
            }

            var VKIndex = NKeys.Count;

            foreach (var vk in VM.VariableKeys.OrderBy(nk => nk.Order))
            {
                var VK = VKeys.FirstOrDefault(vkid => vkid.Id == vk.Id);

                Keyboard.VariableKeys.Add(new KeyboardVariableKeyRelation()
                {
                    VariableKey = VK,
                    Order = BaseOrder + index + 1,
                    DataPoolId = DP.Id
                });

                index++;

                var uniqueReplacementChar = GetUniqueKeyboardReplacementChar(Keyboard);

                if (uniqueReplacementChar is null)
                    return BadRequest($"Cannot add key variation; keyboard {Keyboard.Name} has too many keys");

                Keyboard.VariableKeyImages.AddRange(VK.Variations.Select((k, i) => new KeyboardVariableKeyImageRelation()
                {
                    VariationId = k.Id,
                    ReplacementCharacter = uniqueReplacementChar,
                    DataPoolId = DP.Id
                }));
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok(_mapper.Map<Keyboard, KeyboardViewModel>(Keyboard));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> RemoveKeyFromKeyboard([FromBody] RemoveKeyFromKeyboardViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);

            //Check Datapool

            var Keyboard = await _applicationDbContext.Keyboards
                .Include(k => k.NumericKeys)
                .ThenInclude(a => a.AnswerElements)

                .Include(k => k.VariableKeys)
                
                .Include(k => k.VariableKeyImages)
                .ThenInclude(a => a.AnswerElements)

                 .Include(k => k.VariableKeyImages)
                .ThenInclude(a => a.Variation)

                .FirstOrDefaultAsync(k => k.Id == VM.KeyboardId);

            if (Keyboard is null)
                return NotFound("Keyboard not found");
            
            if(VM.KeyType == Constants.NUMERIC_KEY_TYPE)
            {
                var relation = Keyboard.NumericKeys.FirstOrDefault(a => a.Id == VM.RelationId);

                if (relation is null)
                    return BadRequest("Relationship not found");

                if (relation.AnswerElements.Any())
                    return BadRequest("Key is used cannot be removed");

                if(!Keyboard.NumericKeys.Any(a => a.Id != VM.RelationId) && !Keyboard.VariableKeys.Any())
                    return BadRequest("Keyboard is left with no keys");

                _applicationDbContext.KeyboardNumericKeyRelation.Remove(relation);

                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                var relation = Keyboard.VariableKeys.FirstOrDefault(a => a.Id == VM.RelationId);

                if (relation is null)
                    return BadRequest("Relationship not found");

                var relatedVariantRelations = Keyboard.VariableKeyImages.Where(a => a.Variation.KeyId == relation.VariableKeyId).ToList();
                var isUsed = relatedVariantRelations.Any(x => x.AnswerElements.Any());

                if (isUsed)
                    return BadRequest("Key is used cannot be removed");

                if (!Keyboard.NumericKeys.Any() && !Keyboard.VariableKeys.Any(a => a.Id != VM.RelationId))
                    return BadRequest("Keyboard is left with no keys");

                _applicationDbContext.KeyboardVariableKeyRelation.Remove(relation);
                _applicationDbContext.KeyboardVariableKeyImageRelation.RemoveRange(relatedVariantRelations);

                await _applicationDbContext.SaveChangesAsync();
            }

            return Ok(_mapper.Map<Keyboard, KeyboardViewModel>(Keyboard));
        }


        [HttpPut("[action]")]
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
        public async Task<IActionResult> GetQuestionsForKeyboardKey([FromBody] ViewKeyUsedQuestionsViewModel VM)
        {
            if (!ModelState.IsValid)
                return BadRequest(Constants.HTTP_REQUEST_INVALID_DATA);
            
            if(VM.KeyType == Constants.NUMERIC_KEY_TYPE)
            {
                var relation = _applicationDbContext.KeyboardNumericKeyRelation
                    
                    .FirstOrDefault(a => a.Id == VM.RelationId);

                if (relation is null)
                    return BadRequest("Relationship not found");

                var usedKeyboardQuestions = await _applicationDbContext.KeyboardQuestionAnswerElement
                    .Where(a => a.NumericKeyId == relation.Id)
                    .Select(a => a.Answer.Question)
                    .ToListAsync();

                return Ok(new
                {
                    KeyboardQuestions = _mapper.Map<List<KeyboardQuestionViewModel>>(usedKeyboardQuestions)
                });
            }
            else
            {
                var relation = _applicationDbContext.KeyboardVariableKeyRelation
                    .Include(a => a.Keyboard)
                    .ThenInclude(k => k.VariableKeyImages)
                    .ThenInclude(i => i.Variation)
                    .FirstOrDefault(a => a.Id == VM.RelationId);

                if (relation is null)
                    return BadRequest("Relationship not found");

                var Keyboard = relation.Keyboard;

                var relatedVariantRelations = Keyboard.VariableKeyImages
                    .Where(a => a.Variation.KeyId == relation.VariableKeyId)
                    .Select(a => a.Id)
                    .ToList();

                var usedKeyboardQuestions = await _applicationDbContext.KeyboardQuestionAnswerElement
                    .Where(a => relatedVariantRelations.Any(Id => a.ImageId == Id))
                    .Select(a => a.Answer.Question)
                    .ToListAsync();

                return Ok(new
                {
                    KeyboardQuestions = _mapper.Map<List<KeyboardQuestionViewModel>>(usedKeyboardQuestions)
                });
            }

        }


    }
}
