using BankBranchManagementSystem.Models;
public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task SaveChangesAsync();

    Task<List<AuditLog>> GetAllAsync();
}