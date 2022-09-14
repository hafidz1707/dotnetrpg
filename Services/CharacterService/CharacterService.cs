using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        // private static List<Character> characters = new List<Character>
        // {
        //     new Character(),
        //     new Character{Id = 1, Name = "Naufal"}
        // };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(IMapper  mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            // character.Id = characters.Max(c => c.Id) + 1;
            // characters.Add(character);
            serviceResponse.Data = await _context.Characters
                .Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDTO>(c))
                .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDTO>> response = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                if(character != null)
                {
                    _context.Characters.Remove(character);
                    await _context.SaveChangesAsync();
                    response.Data = await _context.Characters
                        .Where(c => c.User.Id == GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found!";
                }
                
            }
            catch (Exception ex)
            {
                response.Success = false;  
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == GetUserId())
                .ToListAsync();
            response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            return response;
            // return new ServiceResponse<List<GetCharacterDTO>>
            // {
            //     Data = characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList()
            // };
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            var dbCharacter = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            ServiceResponse<GetCharacterDTO> response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character.User.Id == GetUserId())
                {
                    character.Name = updatedCharacter.Name;
                    character.HitPoints = updatedCharacter.HitPoints;
                    character.Strength = updatedCharacter.Strength;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.Agility = updatedCharacter.Agility;
                    character.Defense = updatedCharacter.Defense;
                    character.Class = updatedCharacter.Class;
                    //_mapper.Map(updatedCharacter, character);
                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<GetCharacterDTO>(character);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found!";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                    c.User.Id == GetUserId());
                
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not Found!";
                    return response;
                }
                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not Found!";

                }
                character.Skills.Add(skill);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<string>> UploadPhoto(int id)
        {
            var response = new ServiceResponse<string>();
            Account account = new Account(
                "du5w56akk",
                "642318848454273",
                "62gH_RWfkeWLpbonbLyVCX24Qfs");
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(@"https://upload.wikimedia.org/wikipedia/commons/a/ae/Olympic_flag.jpg"),
                PublicId = "olympic_flag"
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            response.Data = uploadResult.Url.ToString();
            return response;
        }

        public async Task<ServiceResponse<string>> UploadImage()
        {
            var response = new ServiceResponse<string>();
            Account account = new Account(
                "du5w56akk",
                "642318848454273",
                "62gH_RWfkeWLpbonbLyVCX24Qfs");
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            var file = new FileInfo(_httpContextAccessor.HttpContext.Request.Form.Files["Image"].FileName);
            string filename = file.ToString().Substring(0, file.ToString().Length - file.Extension.ToString().Length);
            //var filepath = new FileInfo(_httpContextAccessor.HttpContext.Request.Form.Files["Image"].FileName);
            //var filepath = _httpContextAccessor.HttpContext.Request.Form.Files["Image"].;
            //var filePath = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_signature_{extension.Extension}");
            
            var filepath = Path.Combine("Upload", $"{file}");
            using (var stream = System.IO.File.Create(filepath))
            {
                await _httpContextAccessor.HttpContext.Request.Form.Files["Image"].CopyToAsync(stream);
            }
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(@$"{filepath}"),
                // File = new FileDescription(_httpContextAccessor.HttpContext.Request.Form.Files["Image"])
                // File = new FileDescription(httpRequest.Form.Files["Image"].FileName), 
                //File = new FileDescription(@"https://upload.wikimedia.org/wikipedia/commons/a/ae/Olympic_flag.jpg"),
                PublicId = $"{DateTime.Now.ToString("yyyy-MM-dd:hh-mm-ss")}_{filename}"
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            response.Data = uploadResult.Url.ToString();
            //response.Data = result.ToString();
            return response;
        }
        //private int Test() => int.Parse(_httpContextAccessor.HttpContext.User
        //    .FindFirstValue(ClaimTypes.NameIdentifier));
        //private void GetImageInfo() => _httpContextAccessor.HttpContext.Request.Form.Files["Image"];
        //int.Parse(_httpContextAccessor.HttpContext.User
        //    .FindFirstValue(ClaimTypes.NameIdentifier));
    }
}