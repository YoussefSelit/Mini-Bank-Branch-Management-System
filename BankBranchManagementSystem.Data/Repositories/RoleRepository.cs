using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Repositories
{
    public class RoleRepository : ExtendedRepository<Role>, IRoleRepository
    {

        public RoleRepository(BankDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }


    }
}
