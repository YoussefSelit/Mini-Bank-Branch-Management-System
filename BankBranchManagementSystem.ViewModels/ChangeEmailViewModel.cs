using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required, EmailAddress]
        public string NewEmail { get; set; } = null!;
    }

}