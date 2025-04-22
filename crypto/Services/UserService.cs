using crypto.Interfaces;
using crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace crypto.Services
{
    public class UserService : IUserService
    {
        private readonly CryptoDbContext _context;

        public UserService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Wallet)
                .ThenInclude(w => w.WalletCryptos)
                .ThenInclude(wc => wc.Cryptocurrency)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Create wallet with initial balance for new user
            user.Wallet = new Wallet { Balance = 10000 }; // Start with 10,000 USD

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            // Update user properties
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            
            // Only update password if provided
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                user.Password = updatedUser.Password; // In a real app, you'd hash the password
            }

            await _context.SaveChangesAsync();
            return user;
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