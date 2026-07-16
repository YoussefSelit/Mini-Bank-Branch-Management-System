using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class RoleUpdateDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
    }
}
