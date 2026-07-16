using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Models;

[Index("BranchCode", Name = "UQ__Branches__A9F83E3BE96A25C2", IsUnique = true)]
public partial class Branch
{
    [Key]
    public int BranchId { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? BranchCode { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BranchName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BranchAddress { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BranchCity { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BranchPhone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BranchEmail { get; set; }

    public int? BranchManager { get; set; }

    public DateOnly? BranchOpeningDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? BranchStatus { get; set; }

    [InverseProperty("Branch")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("EmployeeBranch")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [ForeignKey("BranchManager")]
    [InverseProperty("Branches")]
    public virtual Employee? BranchManagerNavigation { get; set; }
}
