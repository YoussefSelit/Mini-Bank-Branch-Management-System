using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class EditBranchViewModel
    {
        [Required]
        public int BranchId { get; set; }

        [Required]
        [StringLength(15)]
        public string BranchCode { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string BranchName { get; set; } = null!;

        [StringLength(50)]
        public string? BranchAddress { get; set; }

        [StringLength(50)]
        public string? BranchCity { get; set; }

        [StringLength(50)]
        public string? BranchPhone { get; set; }

        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? BranchEmail { get; set; }

        public int? BranchManager { get; set; }

        public DateOnly? BranchOpeningDate { get; set; }

        [StringLength(10)]
        public string? BranchStatus { get; set; }

        public string? OldManagerNewJobTitle { get; set; }
    }
}