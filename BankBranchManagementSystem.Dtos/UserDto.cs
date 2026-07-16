using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.Dtos;

public class UserDto
{
    public int UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserUsername { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhoneNumber { get; set; }

    public int? UserRoleId { get; set; }
    public string? UserRoleName { get; set; } // flattened from Role.RoleName

    public int? EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; } // flattened from Employee names
    // NOTE: no password field here — never show it back to anyone
}


