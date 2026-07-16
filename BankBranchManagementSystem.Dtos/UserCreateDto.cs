using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class UserCreateDto
    {
        [Required, StringLength(50)]
        public string UserFirstName { get; set; } = null!;

        [Required, StringLength(50)]
        public string UserLastName { get; set; } = null!;

        [Required, StringLength(50)]
        public string UserUsername { get; set; } = null!;

        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = null!;

        public int? UserRoleId { get; set; }

        [EmailAddress, StringLength(50)]
        public string? UserEmail { get; set; }

        [Phone, StringLength(50)]
        public string? UserPhoneNumber { get; set; }

        public int? EmployeeId { get; set; }
    }

}
