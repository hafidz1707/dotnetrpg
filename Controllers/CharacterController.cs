using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Dtos.Character;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService characterService;
        public CharacterController(ICharacterService characterService)
        {
            this.characterService = characterService;
            
        }
        // private static List<Character> characters = new List<Character>
        // {
        //     new Character(),
        //     new Character{Id = 1, Name = "Naufal"}
        // };
        // Get All

        //[AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> Get()
        {
            //int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await characterService.GetAllCharacters());
        }
        // Get by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> GetSingle(int id)
        {
            return Ok(await characterService.GetCharacterById(id));
        }
        // Delete by Id
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> Delete(int id)
        {
            var response = await characterService.DeleteCharacter(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        // Post new Character
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            return Ok(await characterService.AddCharacter(newCharacter));
        }
        // Update Character
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> UpdateCharacter(UpdateCharacterDTO updateCharacter)
        {
            var response = await characterService.UpdateCharacter(updateCharacter);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        // Add Skill to Character
        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            return Ok(await characterService.AddCharacterSkill(newCharacterSkill));
        }
    }
}