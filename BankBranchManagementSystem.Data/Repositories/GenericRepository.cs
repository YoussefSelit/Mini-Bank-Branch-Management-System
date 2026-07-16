using Microsoft.EntityFrameworkCore;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Data;

namespace BankBranchManagementSystem.Repositories;

// The concrete implementation - this is the ONLY layer that is allowed to
// talk to the DbContext directly. Controllers and Services never see it.
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly BankDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(BankDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}