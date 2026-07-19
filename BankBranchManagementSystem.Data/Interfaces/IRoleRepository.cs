using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.Interfaces

{
    public interface IRoleRepository : IExtendedRepository<Role>
    {

        Task<Role?> GetRoleByNameAsync(string roleName);

    }
}
