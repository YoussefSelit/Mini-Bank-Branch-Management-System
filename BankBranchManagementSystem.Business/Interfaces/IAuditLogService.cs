using BankBranchManagementSystem.Models;

public interface IAuditLogService
{
    Task LogAsync(
    int? userId,
    string action,
    string entityName,
    int? employeeId = null,
    int? branchId = null);

    Task<List<AuditLog>> GetAllAsync();
}