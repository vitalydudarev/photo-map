using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IUserService _dbUserService;

        public UsersController(IPhotoService photoService, IUserService dbUserService)
        {
            _photoService = photoService;
            _dbUserService = dbUserService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserAsync([FromBody] AddUserDto addUserDto)
        {
            await _dbUserService.AddAsync(addUserDto);

            return Ok();
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            await _dbUserService.UpdateAsync(id, updateUserDto);

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

        [HttpGet("{id}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<PhotoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserPhotos([FromRoute] int id, [FromQuery] int top, [FromQuery] int skip)
        {
            var userPhotos = await _photoService.GetByUserIdAsync(id, top, skip);
            return Ok(userPhotos);
        }
    }
}
