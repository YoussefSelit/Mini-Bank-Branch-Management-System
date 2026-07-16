using BankBranchManagementSystem.Models;


namespace BankBranchManagementSystem.Interfaces;

// Everything generic (GetAll, GetById, Add, Update, Delete) comes free from IGenericRepository.
// Here we only declare the queries that are specific to Branch.
public interface IBranchRepository : IExtendedRepository<Branch>
{
    Task<Branch?> GetBranchWithEmployeesAsync(int branchId);
    Task<IEnumerable<Branch>> SearchBranchesAsync(string? searchTerm);
    Task<bool> IsBranchCodeUniqueAsync(string branchCode, int? excludeBranchId = null);
    Task<bool> HasActiveEmployeesAsync(int branchId);
    Task<int> GetActiveEmployeeCountAsync(int branchId);

    Task<int> GetTotalBranchesAsync();
    Task<int> GetActiveBranchesAsync();
    Task<IEnumerable<Branch>> GetRecentlyAddedBranchesAsync();

    Task<bool> IsBranchManagerAsync(int employeeId);

    Task<Branch?> GetBranchByManagerAsync(int managerId);
}
