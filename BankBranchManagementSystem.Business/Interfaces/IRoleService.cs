using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.Interfaces
{
    public interface IRoleService
    {

        Task<IEnumerable<Role>> GetAllRolesAsync();

        Task<Role?> GetRoleAsync(int id);

        Task AddRoleAsync(Role role);

        Task UpdateRoleAsync(Role role);

        Task DeleteRoleAsync(int id);

    }
}
