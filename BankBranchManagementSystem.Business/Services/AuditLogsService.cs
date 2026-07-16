using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Repositories;


public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository auditLogRepository;

    public AuditLogService(IAuditLogRepository repository)
    {
        auditLogRepository = repository;
    }

    public async Task LogAsync(CreateAuditLogDto log)
    {
        var entity = new AuditLog
        {
            UserId = log.UserId,
            Action = log.Action,
            EntityName = log.EntityName,
            EmployeeId = log.EmployeeId,
            BranchId = log.BranchId,
            ActionDate = DateTime.Now
        };

        await auditLogRepository.AddAsync(entity);
        await auditLogRepository.SaveChangesAsync();
    }

    public async Task<List<AuditLogDto>> GetAllAsync()
    {
        var logs = await auditLogRepository.GetAllAsync();
        return logs.Select(MapToDto).ToList();
    }

    private static AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            LogId = log.LogId,
            Action = log.Action,
            EntityName = log.EntityName,
            ActionDate = log.ActionDate,

            UserId = log.UserId,
            UserUsername = log.User?.UserUsername,

            EmployeeId = log.EmployeeId,
            EmployeeFullName = log.Employee != null
                ? $"{log.Employee.EmployeeFirstName} {log.Employee.EmployeeLastName}"
                : null,

            BranchId = log.BranchId,
            BranchName = log.Branch?.BranchName
        };
    }
}