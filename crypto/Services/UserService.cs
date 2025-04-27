using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq;

namespace crypto.Services
{
    public class UserService : IUserService
    {
        private readonly CryptoDbContext _context;

        public UserService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto { Id = u.Id, Username = u.Username, Email = u.Email })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id) 
        {
            var user = await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email };
        }

        public async Task<UserDto> RegisterUserAsync(UserRegisterDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password, 
                Wallet = new Wallet { Balance = 10000 } // kezdő egyenleg
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email }; 
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto updatedUserDto) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

      
            if (!string.IsNullOrWhiteSpace(updatedUserDto.Username))
            {
                 user.Username = updatedUserDto.Username;
            }
             if (!string.IsNullOrWhiteSpace(updatedUserDto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == updatedUserDto.Email && u.Id != id))
                {
                    throw new InvalidOperationException("A user with this email already exists");
                }
                 user.Email = updatedUserDto.Email;
            }
            if (!string.IsNullOrWhiteSpace(updatedUserDto.Password))
            {
                user.Password = updatedUserDto.Password; 
            }

            await _context.SaveChangesAsync();
            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email }; 
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}