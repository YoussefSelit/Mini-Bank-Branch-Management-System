namespace BankBranchManagementSystem.Interfaces
{
    public interface IExtendedRepository<T> : IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        void Update(T entity);
        void Delete(T entity);
    }
}
