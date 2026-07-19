using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Display(Name = "First Name")]
        [Required]
        [StringLength(50)]
        public string EmployeeFirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        [Required]
        [StringLength(50)]
        public string EmployeeLastName { get; set; } = null!;

        [Display(Name = "Job Title")]

        [StringLength(50)]
        public string? EmployeeJobTitle { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(50)]
        public string? EmployeePhone { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? EmployeeEmail { get; set; }

        [Display(Name = "Hire Date")]
        public DateOnly? EmployeeHireDate { get; set; }

        [Display(Name = "Branch Name")]
        [Required]
        public int EmployeeBranchId { get; set; }

        [Display(Name = "Status")]
        [StringLength(10)]
        public string? EmploymentStatus { get; set; }
    }
}