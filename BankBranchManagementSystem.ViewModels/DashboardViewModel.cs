using BankBranchManagementSystem.Models;

namespace BankBranchManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBranches { get; set; }
        public int ActiveBranches { get; set; }
        public int TotalEmployees { get; set; }
        public List<Branch> RecentBranches { get; set; } = new();
        public List<BranchEmployeeCountViewModel> EmployeesPerBranch { get; set; } = new();
    }
}