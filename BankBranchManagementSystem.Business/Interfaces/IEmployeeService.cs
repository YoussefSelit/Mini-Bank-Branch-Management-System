//using BankBranchManagementSystem.Models;

//namespace BankBranchManagementSystem.Interfaces
//{
//    public interface IEmployeeService
//    {

//        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

//        Task<Employee?> GetEmployeeAsync(int id);

//        Task<Employee?> GetEmployeeDetailsAsync(int id);

//        Task AddEmployeeAsync(Employee employee);

//        Task UpdateEmployeeAsync(Employee employee);

//        Task DeleteEmployeeAsync(int currentUserId, int employeeId, int? managerId);

//        Task TransferEmployeeAsync(int employeeId, int newBranchId);

//        Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm);

//        Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId);

//        Task<int> GetTotalEmployeesAsync();

//        Task<Dictionary<int, int>> GetEmployeeCountsByBranchAsync();

//    }
//}

using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Dtos;


namespace BankBranchManagementSystem.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeListDto>> GetAllEmployeesAsync();

        Task<Employee?> GetEmployeeAsync(int id);

        Task<EmployeeDetailsDto?> GetEmployeeDetailsAsync(int id);

        Task AddEmployeeAsync(Employee employee, int userID);

        Task UpdateEmployeeAsync(Employee employee, int userID);

        Task DeleteEmployeeAsync(int currentUserId, int employeeId, int? managerId);

        Task TransferEmployeeAsync(int employeeId, int newBranchId, int userID);

        Task<IEnumerable<EmployeeListDto>> SearchEmployeesAsync(string? searchTerm);

        Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId);

        Task<int> GetTotalEmployeesAsync();

        Task<Dictionary<int, int>> GetEmployeeCountsByBranchAsync();

        Task<PagedResult<EmployeeListDto>> GetEmployeesPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
}
