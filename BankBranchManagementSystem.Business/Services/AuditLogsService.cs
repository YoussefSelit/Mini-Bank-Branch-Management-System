using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Repositories;


public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository auditLogRepository;

    public AuditLogService(IAuditLogRepository repository)
    {
        auditLogRepository = repository;
    }

    public async Task LogAsync(
    int? userId,
    string action,
    string entityName,
    int? employeeId = null,
    int? branchId = null)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EmployeeId = employeeId,
            BranchId = branchId,
            ActionDate = DateTime.Now
        };

        await auditLogRepository.AddAsync(log);
        await auditLogRepository.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await auditLogRepository.GetAllAsync();
    }
}