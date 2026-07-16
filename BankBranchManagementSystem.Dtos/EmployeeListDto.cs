using System;
using System.Collections.Generic;
using System.Text;

namespace BankBranchManagementSystem.Dtos;
public class EmployeeListDto
{
    public int EmployeeId { get; set; }
    public string? EmployeeFirstName { get; set; }
    public string? EmployeeLastName { get; set; }
    public string? EmployeeJobTitle { get; set; }
    public string? EmploymentStatus { get; set; }
    public int EmployeeBranchId { get; set; }              
    public string? EmployeeBranchName { get; set; }
    public string? EmployeeBranchStatus { get; set; }
    public int? EmployeeBranchManagerId { get; set; }
}


