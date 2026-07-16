namespace BankBranchManagementSystem.Dtos;

public class BranchDetailsDto
{
    public int BranchId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchName { get; set; }
    public string? BranchAddress { get; set; }
    public string? BranchCity { get; set; }
    public string? BranchPhone { get; set; }
    public string? BranchEmail { get; set; }
    public DateOnly? BranchOpeningDate { get; set; }
    public string? BranchStatus { get; set; }

    public int? BranchManager { get; set; }
    public string? BranchManagerName { get; set; }

    public List<EmployeeSummaryDto>? Employees { get; set; }
}