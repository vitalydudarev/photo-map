using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Services;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Database.Services.IUserService _dbUserService;

        public UsersController(IUserService userService, Database.Services.IUserService dbUserService)
        {
            _userService = userService;
            _dbUserService = dbUserService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddTokenAsync([FromBody] AddUserDto addUserDto)
        {
            await _dbUserService.AddAsync(addUserDto);

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await _dbUserService.GetAsync(id);
            if (user != null)
                return Ok(user);

            return NotFound();
        }

        [HttpGet("images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserImages()
        {
            return Ok(_userService.GetUserFiles());
        }
    }
}
