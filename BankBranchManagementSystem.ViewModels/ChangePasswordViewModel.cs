using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required, Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
