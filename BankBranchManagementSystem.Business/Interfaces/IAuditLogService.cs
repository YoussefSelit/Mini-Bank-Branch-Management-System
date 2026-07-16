using BankBranchManagementSystem.Dtos;

public interface IAuditLogService
{
    Task LogAsync(CreateAuditLogDto log);

    Task<List<AuditLogDto>> GetAllAsync();
}