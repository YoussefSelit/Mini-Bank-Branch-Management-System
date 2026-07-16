using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Models;

public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmployeeFirstName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmployeeLastName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmployeeJobTitle { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmployeePhone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmployeeEmail { get; set; }

    public DateOnly? EmployeeHireDate { get; set; }

    public int EmployeeBranchId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? EmploymentStatus { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("BranchManagerNavigation")]
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    [InverseProperty("Employee")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [ForeignKey("EmployeeBranchId")]
    [InverseProperty("Employees")]
    public virtual Branch EmployeeBranch { get; set; } = null!;
}
