using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.Dtos;

// Used when DISPLAYING a role (e.g. a dropdown list, or a details page)
public class RoleDto
{
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
}


