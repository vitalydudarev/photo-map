using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserImages()
        {
            return Ok(_userService.GetUserFiles());
        }
    }
}
