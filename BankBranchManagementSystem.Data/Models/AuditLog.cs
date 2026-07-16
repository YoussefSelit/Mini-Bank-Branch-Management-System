using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Models;

public partial class AuditLog
{
    [Key]
    public int LogId { get; set; }

    public int? UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Action { get; set; }

    public int? EmployeeId { get; set; }

    public int? BranchId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EntityName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ActionDate { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? User { get; set; }

    [ForeignKey("BranchId")]
    [InverseProperty("AuditLogs")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("AuditLogs")]
    public virtual Employee? Employee { get; set; }
}
