using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string UserFirstName { get; set; } = null!;

        [Required, StringLength(50)]
        public string UserLastName { get; set; } = null!;

        [Required, StringLength(50)]
        public string UserUsername { get; set; } = null!;

        public int? UserRoleId { get; set; }

        [EmailAddress, StringLength(50)]
        public string? UserEmail { get; set; }

        [Phone, StringLength(50)]
        public string? UserPhoneNumber { get; set; }

        public int? EmployeeId { get; set; }
        // Password left out on purpose — editing a user shouldn't silently touch their password.
        // Later, if you want a "change password" feature, make a separate ChangePasswordDto for it.
    }
}
