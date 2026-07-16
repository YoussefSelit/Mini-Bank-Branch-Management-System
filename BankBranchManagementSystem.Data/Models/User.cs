using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserFirstName{ get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserLastName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserUsername { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserPassword { get; set; }

    public int? UserRoleId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserEmail { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserPhoneNumber { get; set; }

    public int? EmployeeId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("Users")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("UserRoleId")]
    [InverseProperty("Users")]
    public virtual Role? UserRole { get; set; }
}
