using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; 

namespace crypto.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers() 
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById(int userId) 
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser(UserRegisterDto userDto) 
        {
            try
            {
                var registeredUser = await _userService.RegisterUserAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { userId = registeredUser.Id }, registeredUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int userId, UserUpdateDto userDto)
        {
             if (userId <= 0) 
            {
                return BadRequest("Invalid user ID.");
            }


            var updatedUser = await _userService.UpdateUserAsync(userId, userDto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser); 
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}