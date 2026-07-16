using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class RoleCreateDto
    {
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
    }

}
