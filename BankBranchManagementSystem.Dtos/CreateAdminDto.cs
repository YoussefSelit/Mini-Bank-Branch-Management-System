using System.ComponentModel.DataAnnotations;

namespace BankBranchManagementSystem.Dtos
{
    public class CreateAdminDto
    {
        [Required, StringLength(50)]
        public string UserFirstName { get; set; } = null!;

        [Required, StringLength(50)]
        public string UserLastName { get; set; } = null!;

        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = null!;

        [EmailAddress, StringLength(50)]
        public string? UserEmail { get; set; }

        [Phone, StringLength(50)]
        public string? UserPhoneNumber { get; set; }
    }
}