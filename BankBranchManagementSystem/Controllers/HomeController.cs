using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IEmployeeService _employeeService;

        public HomeController(IBranchService branchService, IEmployeeService employeeService)
        {
            _branchService = branchService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var allBranches = await _branchService.GetAllBranchesAsync();
            var employeeCounts = await _employeeService.GetEmployeeCountsByBranchAsync();

            var model = new DashboardViewModel
            {
                TotalBranches = await _branchService.GetTotalBranchesAsync(),
                ActiveBranches = await _branchService.GetActiveBranchesAsync(),
                TotalEmployees = await _employeeService.GetTotalEmployeesAsync(),
                RecentBranches = (await _branchService.GetRecentlyAddedBranchesAsync()).ToList(),
                EmployeesPerBranch = allBranches.Select(b => new BranchEmployeeCountViewModel
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName ?? string.Empty,
                    EmployeeCount = employeeCounts.TryGetValue(b.BranchId, out var count) ? count : 0
                }).ToList()
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}