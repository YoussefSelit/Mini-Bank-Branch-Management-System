using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetRoleAsync(int id)
        {
            return await roleRepository.GetByIdAsync(id);
        }

        public async Task AddRoleAsync(Role role)
        {
            await roleRepository.AddAsync(role);
            await roleRepository.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            var existing = await roleRepository.GetByIdAsync(role.RoleId);
            if (existing == null)
                throw new KeyNotFoundException("Role not found.");

            roleRepository.Update(role);
            await roleRepository.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Role not found.");
            roleRepository.Delete(role);
            await roleRepository.SaveChangesAsync();
        }

    }
}
