using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.ViewModels
{
    public class CreateBranchViewModel
    {
        [Display(Name = "Branch Code")]
        [Required]
        [StringLength(15)]
        public string BranchCode { get; set; } = null!;

        [Display(Name = "Branch Name")]
        [Required]
        [StringLength(50)]
        public string BranchName { get; set; } = null!;

        [Display(Name = "Branch Address")]
        [StringLength(50)]
        public string? BranchAddress { get; set; }

        [Display(Name = "Branch City")]
        [StringLength(50)]
        public string? BranchCity { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(50)]
        public string? BranchPhone { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? BranchEmail { get; set; }

        [Display(Name = "Branch Manager")]
        public int? BranchManager { get; set; }

        [Display(Name = "Branch Opening Date")]
        public DateOnly? BranchOpeningDate { get; set; }
    }
}