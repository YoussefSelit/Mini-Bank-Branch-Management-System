using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class TransferEmployeeViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int NewBranchId { get; set; }
    }
}