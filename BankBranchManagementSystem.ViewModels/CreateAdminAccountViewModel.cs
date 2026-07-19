using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class CreateAdminAccountViewModel
    {
        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string UserFirstName { get; set; } = null!;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string UserLastName { get; set; } = null!;

        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(UserPassword), ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required, EmailAddress, StringLength(50)]
        public string UserEmail { get; set; } = null!;

        [Required, Phone, StringLength(50)]
        public string UserPhoneNumber { get; set; } = null!;
    }

}