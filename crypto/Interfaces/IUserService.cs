using crypto.Models;
using crypto.Dtos;
using System.Collections.Generic;

namespace crypto.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(); 
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> RegisterUserAsync(UserRegisterDto userDto); 
        Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto); 
        Task<bool> DeleteUserAsync(int id);
    }
}