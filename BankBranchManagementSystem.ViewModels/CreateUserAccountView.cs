using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class CreateUserAccountViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string InitialPassword { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }
    }
}
