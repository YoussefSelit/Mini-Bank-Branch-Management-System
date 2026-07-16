using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Interfaces;

namespace BankBranchManagementSystem.Repositories
{
    public class ExtendedRepository<T> : GenericRepository<T>, IExtendedRepository<T> where T : class
    {
        public ExtendedRepository(BankDbContext context) : base(context)
        {
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}