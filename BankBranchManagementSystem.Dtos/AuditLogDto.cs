namespace BankBranchManagementSystem.Dtos;

// Used only for DISPLAYING audit history to an admin — never submitted by a user
public class AuditLogDto
{
    public int LogId { get; set; }
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public DateTime? ActionDate { get; set; }

    public int? UserId { get; set; }
    public string? UserUsername { get; set; } // flattened from User.UserUsername

    public int? EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; } // flattened

    public int? BranchId { get; set; }
    public string? BranchName { get; set; } // flattened
}

