using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BankBranchManagementSystem.Repositories
{
    public class UserRepository : ExtendedRepository<User>, IUserRepository
    {

        public UserRepository(BankDbContext context) : base(context)
        {

        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.UserUsername == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.UserUsername == username);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.UserEmail == email);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.UserId != excludeUserId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.UserPhoneNumber == phone);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.UserId != excludeUserId.Value);

            return await query.AnyAsync();
        }

        //public async Task<User?> LoginAsync(string username, string password)
        //{
        //    return await _context.Users
        //        .Include(u => u.UserRole)
        //        .FirstOrDefaultAsync(u => u.UserUsername == username && u.UserPassword == password);


        //}



        public async Task<User?> GetUserWithRoleAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }


        public async Task<User?> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

        }


        public async Task<List<string>> GetUsernamesByPrefixAsync(string prefix)
        {
            return await _context.Users
                .Where(u => u.UserUsername != null && u.UserUsername.StartsWith(prefix))
                .Select(u => u.UserUsername!)
                .ToListAsync();
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
