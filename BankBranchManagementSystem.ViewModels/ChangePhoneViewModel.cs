using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class ChangePhoneViewModel
    {
        [Required]
        public string NewPhone { get; set; } = null!;
    }

}
