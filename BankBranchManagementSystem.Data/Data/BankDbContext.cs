using BankBranchManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BankBranchManagementSystem.Data;

public partial class BankDbContext : DbContext
{
    public BankDbContext()
    {
    }

    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=branchManagement;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Audit_Lo__2D26E7AE92D18E3B");

            entity.ToTable("Audit_Logs");

            entity.Property(e => e.LogId).HasColumnName("Log_ID");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("action");
            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime")
                .HasColumnName("action_date");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EntityName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entity_name");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Branch).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("fk_audit_branch");

            entity.HasOne(d => d.Employee).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_audit_employee");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_audit_user");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branches__E55E37DE712B3E54");

            entity.HasIndex(e => e.BranchCode, "UQ__Branches__A9F83E3B7ABA43D7").IsUnique();

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.BranchAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("branch_address");
            entity.Property(e => e.BranchCity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("branch_city");
            entity.Property(e => e.BranchCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("branch_code");
            entity.Property(e => e.BranchEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("branch_email");
            entity.Property(e => e.BranchManager).HasColumnName("branch_manager");
            entity.Property(e => e.BranchName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("branch_name");
            entity.Property(e => e.BranchOpeningDate).HasColumnName("branch_opening_date");
            entity.Property(e => e.BranchPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("branch_phone");
            entity.Property(e => e.BranchStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("branch_status");

            entity.HasOne(d => d.BranchManagerNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.BranchManager)
                .HasConstraintName("fk_branch_manager");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA86731ADF4");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeBranchId).HasColumnName("employee_branch_id");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_email");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_first_name");
            entity.Property(e => e.EmployeeHireDate).HasColumnName("employee_hire_date");
            entity.Property(e => e.EmployeeJobTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_job_title");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_last_name");
            entity.Property(e => e.EmployeePhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_phone");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("employment_status");

            entity.HasOne(d => d.EmployeeBranch).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_branch");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC6D61DB63");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FCDA0CAD9");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserFirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_first_name");
            entity.Property(e => e.UserLastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_last_name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_password");
            entity.Property(e => e.UserPhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_phone_number");
            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.UserUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_username");

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_user_employee");

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .HasConstraintName("fk_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}