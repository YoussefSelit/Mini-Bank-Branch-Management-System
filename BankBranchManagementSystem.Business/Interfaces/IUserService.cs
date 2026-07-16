using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Dtos;

namespace BankBranchManagementSystem.Interfaces
{
    public interface IUserService
    {
        Task<User?> LoginAsync(string username, string password);

        Task<UserDto?> GetUserAsync(int id);

        //Task<IEnumerable<User>> GetAllUsersAsync();

        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        Task ChangeEmailAsync(int userId, string newEmail);

        Task ChangePhoneNumberAsync(int userId, string newPhone);

        //Task DeleteUserAsync(int adminId, int userId);

        //Task<User> CreateUserAccountAsync(int adminId, int employeeId, string username, string initialPassword, int roleId);

        Task CreateBranchManagerAccountAsync(Employee employee);
    
    
        //Removed the CreateUserAccountAsync, GetAllUsersAsync and DeleteUserAsync
    }
}
