//using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Repositories;

public class BranchRepository : ExtendedRepository<Branch>, IBranchRepository
{
    public BranchRepository(BankDbContext context) : base(context)
    {
    }

    public async Task<Branch?> GetBranchWithEmployeesAsync(int branchId)
    {
        return await _context.Branches
            .Include(b => b.Employees)
            .Include(b => b.BranchManagerNavigation)
            .FirstOrDefaultAsync(b => b.BranchId == branchId);
    }

    public async Task<IEnumerable<Branch>> SearchBranchesAsync(string? searchTerm)
    {
        var query = _context.Branches.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b =>
                (b.BranchCode != null && b.BranchCode.Contains(searchTerm)) ||
                (b.BranchName != null && b.BranchName.Contains(searchTerm)) ||
                (b.BranchCity != null && b.BranchCity.Contains(searchTerm)));
        }

        return await query.ToListAsync();
    }

    public async Task<bool> IsBranchCodeUniqueAsync(string branchCode, int? excludeBranchId = null)
    {
        var query = _context.Branches.Where(b => b.BranchCode == branchCode);

        if (excludeBranchId.HasValue)
            query = query.Where(b => b.BranchId != excludeBranchId.Value);

        return !await query.AnyAsync();
    }

    public async Task<bool> HasActiveEmployeesAsync(int branchId)
    {
        return await _context.Employees
            .AnyAsync(e => e.EmployeeBranchId == branchId && e.EmploymentStatus == "Active");
    }

    public async Task<int> GetActiveEmployeeCountAsync(int branchId)
    {
        return await _context.Employees
            .CountAsync(e => e.EmployeeBranchId == branchId && e.EmploymentStatus == "Active");
    }

    public async Task<int> GetTotalBranchesAsync() => await _context.Branches.CountAsync();

    public async Task<int> GetActiveBranchesAsync() => await _context.Branches.CountAsync(b => b.BranchStatus == "Active");

    public async Task<IEnumerable<Branch>> GetRecentlyAddedBranchesAsync()
    {
        return await _context.Branches
            .OrderByDescending(b => b.BranchOpeningDate)
            .Take(5)
            .ToListAsync();
    }

    public async Task<bool> IsBranchManagerAsync(int employeeId)
    {
        return await _context.Branches
            .AnyAsync(b => b.BranchManager == employeeId);
    }

    public async Task<Branch?> GetBranchByManagerAsync(int managerId)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchManager == managerId);
    }


}
