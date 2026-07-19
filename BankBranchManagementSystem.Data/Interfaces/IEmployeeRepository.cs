using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.Interfaces;

public interface IEmployeeRepository : IExtendedRepository<Employee>
{
    Task<Employee?> GetEmployeeWithBranchAsync(int employeeId);
    Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId);
    Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm);
    Task<int> GetTotalEmployeesAsync();

    Task<Dictionary<int, int>> GetEmployeeCountsByBranchAsync();

    Task<(IEnumerable<Employee> Items, int TotalCount)> SearchEmployeesPagedAsync(string? searchTerm, int pageNumber, int pageSize);
}