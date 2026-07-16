using System;
using System.Collections.Generic;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class EmployeeDetailsDto
    {
        public int EmployeeId { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string? EmployeeJobTitle { get; set; }
        public string? EmployeePhone { get; set; }
        public string? EmployeeEmail { get; set; }
        public DateOnly? EmployeeHireDate { get; set; }
        public string? EmploymentStatus { get; set; }

        public int EmployeeBranchId { get; set; }
        public string? EmployeeBranchName { get; set; }

        // Optional extras — safe here since we only pull names/ids, no nested cycles
        public List<string>? LinkedUsernames { get; set; }       // User accounts tied to this employee
        public List<string>? ManagedBranchNames { get; set; }    // Branches where this employee is the manager
    }
}
