namespace BankBranchManagementSystem.Interfaces;

// A generic interface holds the CRUD operations that are identical for every entity
// (Branch, Employee, ...) so we don't retype GetAll/GetById/Add/Update/Delete for each one.
// Entity-specific interfaces (IBranchRepository, IEmployeeRepository) inherit from this
// and add only the queries that are unique to that entity.
public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task SaveChangesAsync();
}
