using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(BankDbContext context)
        : base(context)
    {
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .Include(a => a.Employee)
            .Include(a => a.Branch)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();
    }
}