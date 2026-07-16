using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.Interfaces
{
    public interface IUserRepository : IExtendedRepository<User>
    {

        Task<User?> GetByUsernameAsync(string username);

        //Task<User?> LoginAsync(string username, string password);

        Task<bool> UsernameExistsAsync(string username);

        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        Task<bool> PhoneExistsAsync(string phone, int? excludeUserId = null);

        Task<User?> GetUserWithRoleAsync(int id);

        Task<User?> GetByEmployeeIdAsync(int employeeId);

        void Delete(User user);
        Task SaveChangesAsync();
    }
}
