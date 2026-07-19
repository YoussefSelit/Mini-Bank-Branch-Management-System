using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankBranchManagementSystem.Repositories;

public class EmployeeRepository : ExtendedRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetEmployeeWithBranchAsync(int employeeId)
    {
        return await _context.Employees
            .Include(e => e.EmployeeBranch)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId)
    {
        return await _context.Employees
            .Where(e => e.EmployeeBranchId == branchId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm)
    {
        var query = _context.Employees.Include(e => e.EmployeeBranch).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.EmployeeId.ToString().Equals(searchTerm) ||
                e.EmployeeId.ToString().Contains(searchTerm) ||
                (e.EmployeeFirstName != null && e.EmployeeFirstName.Contains(searchTerm)) ||
                (e.EmployeeLastName != null && e.EmployeeLastName.Contains(searchTerm)) ||
                (e.EmployeeFirstName + " " +  e.EmployeeLastName).Contains(searchTerm) ||
                (e.EmployeeBranch != null && e.EmployeeBranch.BranchName != null && e.EmployeeBranch.BranchName.Contains(searchTerm)));
        }

        return await query.ToListAsync();
    }


    public async Task<int> GetTotalEmployeesAsync() => await _context.Employees.CountAsync();


    public async Task<Dictionary<int, int>> GetEmployeeCountsByBranchAsync()
    {
        return await _context.Employees
            .GroupBy(e => e.EmployeeBranchId)
            .Select(g => new { BranchId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BranchId, x => x.Count);
    }

    public async Task<(IEnumerable<Employee> Items, int TotalCount)> SearchEmployeesPagedAsync(string? searchTerm, int pageNumber, int pageSize)
    {
        var query = _context.Employees.Include(e => e.EmployeeBranch).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.EmployeeId.ToString().Equals(searchTerm) ||
                e.EmployeeId.ToString().Contains(searchTerm) ||
                (e.EmployeeFirstName != null && e.EmployeeFirstName.Contains(searchTerm)) ||
                (e.EmployeeLastName != null && e.EmployeeLastName.Contains(searchTerm)) ||
                (e.EmployeeFirstName + " " + e.EmployeeLastName).Contains(searchTerm) ||
                (e.EmployeeBranch != null && e.EmployeeBranch.BranchName != null && e.EmployeeBranch.BranchName.Contains(searchTerm)));
        }

        query = query.OrderBy(e => e.EmployeeLastName).ThenBy(e => e.EmployeeFirstName);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
