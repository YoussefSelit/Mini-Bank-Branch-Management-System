using System;
using System.Collections.Generic;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class BranchListDto
    {
        public int BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCity { get; set; }
        public string? BranchStatus { get; set; }
        public string? BranchManagerName { get; set; }
        public DateOnly? BranchOpeningDate { get; set; }
        public int? BranchManager { get; set; } 


    }
}
