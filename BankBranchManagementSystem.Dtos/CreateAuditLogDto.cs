using System;
using System.Collections.Generic;
using System.Text;

namespace BankBranchManagementSystem.Dtos
{
    public class CreateAuditLogDto
    {
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public int? EmployeeId { get; set; }
        public int? BranchId { get; set; }
        public string? EntityName { get; set; }
        // ActionDate is NOT here — your code sets it to DateTime.Now when saving, not the caller
    }
}
