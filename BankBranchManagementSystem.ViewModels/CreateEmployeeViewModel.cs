using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(50)]
        public string EmployeeFirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string EmployeeLastName { get; set; } = null!;

        [StringLength(50)]
        public string? EmployeeJobTitle { get; set; }

        [StringLength(50)]
        public string? EmployeePhone { get; set; }

        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? EmployeeEmail { get; set; }

        public DateOnly? EmployeeHireDate { get; set; }

        [Required]
        public int EmployeeBranchId { get; set; }

        [StringLength(10)]
        public string? EmploymentStatus { get; set; }
    }
}