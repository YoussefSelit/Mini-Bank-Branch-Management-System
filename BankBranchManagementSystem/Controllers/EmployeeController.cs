using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Services;
using BankBranchManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBranchService _branchService;

        public EmployeeController(IEmployeeService employeeService, IBranchService branchService)
        {
            _employeeService = employeeService;
            _branchService = branchService;
        }

        // GET: /Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.SearchEmployeesAsync(null);
            return View("EmployeeManagement", employees);
        }

        // GET: /Employee/Details/5  (full-page fallback / direct link)
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // GET: /Employee/ViewModal/5  (fragment used by the Bootstrap modal on EmployeeManagement)
        public async Task<IActionResult> ViewModal(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);
            if (employee == null)
                return NotFound();

            return PartialView("ViewEmployee", employee);
        }

        // GET: /Employee/Create
        public async Task<IActionResult> Create()
        {
            await PopulateBranchDropdownAsync(activeOnly: true);
            return View();
        }

        // POST: /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateBranchDropdownAsync(activeOnly: true);
                return View(model);
            }

            var employee = new Employee
            {
                EmployeeFirstName = model.EmployeeFirstName,
                EmployeeLastName = model.EmployeeLastName,
                EmployeeJobTitle = model.EmployeeJobTitle,
                EmployeePhone = model.EmployeePhone,
                EmployeeEmail = model.EmployeeEmail,
                EmployeeHireDate = model.EmployeeHireDate,
                EmployeeBranchId = model.EmployeeBranchId,
                EmploymentStatus = model.EmploymentStatus
            };

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _employeeService.AddEmployeeAsync(employee, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateBranchDropdownAsync(activeOnly: true);
                return View(model);
            }
        }

        // GET: /Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeAsync(id);
            if (employee == null)
                return NotFound();

            var model = new EditEmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeFirstName = employee.EmployeeFirstName ?? string.Empty,
                EmployeeLastName = employee.EmployeeLastName ?? string.Empty,
                EmployeeJobTitle = employee.EmployeeJobTitle,
                EmployeePhone = employee.EmployeePhone,
                EmployeeEmail = employee.EmployeeEmail,
                EmployeeHireDate = employee.EmployeeHireDate,
                EmployeeBranchId = employee.EmployeeBranchId,
                EmploymentStatus = employee.EmploymentStatus
            };

            await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: employee.EmployeeBranchId);
            return View(model);
        }

        // POST: /Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel model)
        {
            if (id != model.EmployeeId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: model.EmployeeBranchId);
                return View(model);
            }

            var employee = new Employee
            {
                EmployeeId = model.EmployeeId,
                EmployeeFirstName = model.EmployeeFirstName,
                EmployeeLastName = model.EmployeeLastName,
                EmployeeJobTitle = model.EmployeeJobTitle,
                EmployeePhone = model.EmployeePhone,
                EmployeeEmail = model.EmployeeEmail,
                EmployeeHireDate = model.EmployeeHireDate,
                EmployeeBranchId = model.EmployeeBranchId,
                EmploymentStatus = model.EmploymentStatus
            };

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _employeeService.UpdateEmployeeAsync(employee, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: model.EmployeeBranchId);
                return View(model);
            }
        }

        /// GET: /Employee/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);

            if (employee == null)
                return NotFound();

            var branch = await _branchService.GetBranchAsync(employee.EmployeeBranchId);

            ViewBag.IsBranchManager =
                branch != null &&
                branch.BranchManager == employee.EmployeeId;

            return View(employee);
        }

        /// POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _employeeService.DeleteEmployeeAsync(currentUserId, id, null);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var employee = await _employeeService.GetEmployeeDetailsAsync(id);

                if (employee != null)
                {
                    var branch = await _branchService.GetBranchAsync(employee.EmployeeBranchId);

                    ViewBag.IsBranchManager =
                        branch != null &&
                        branch.BranchManager == employee.EmployeeId;
                }

                return View("Delete", employee);
            }
        }

        private async Task PopulateReplacementManagerDropdownAsync(int branchId, int excludeEmployeeId)
        {
            var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
            ViewBag.ReplacementManagers = employees
                .Where(e => e.EmployeeId != excludeEmployeeId && e.EmploymentStatus == "Active")
                .Select(e => new { e.EmployeeId, FullName = $"{e.EmployeeFirstName} {e.EmployeeLastName}" })
                .ToList();
        }

        // GET: /Employee/Transfer/5
        public async Task<IActionResult> Transfer(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);
            if (employee == null)
                return NotFound();

            var model = new TransferEmployeeViewModel { EmployeeId = employee.EmployeeId };

            // Only active branches other than the employee's current one are valid transfer targets.
            await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: employee.EmployeeBranchId, excludeCurrentBranch: true);
            ViewBag.Employee = employee;
            return View(model);
        }

        // POST: /Employee/Transfer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(int id, TransferEmployeeViewModel model)
        {
            if (id != model.EmployeeId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var employee = await _employeeService.GetEmployeeDetailsAsync(id);
                await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: employee?.EmployeeBranchId, excludeCurrentBranch: true);
                ViewBag.Employee = employee;
                return View(model);
            }

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _employeeService.TransferEmployeeAsync(model.EmployeeId, model.NewBranchId, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var employee = await _employeeService.GetEmployeeDetailsAsync(id);
                await PopulateBranchDropdownAsync(activeOnly: false, currentBranchId: employee?.EmployeeBranchId, excludeCurrentBranch: true);
                ViewBag.Employee = employee;
                return View(model);
            }
        }

        // GET: /Employee/Search?searchTerm=
        public async Task<IActionResult> Search(string? searchTerm)
        {
            var results = await _employeeService.SearchEmployeesAsync(searchTerm);
            return View("EmployeeManagement", results);
        }

        /// <summary>
        /// Builds the branch dropdown used by Create/Edit/Transfer.
        /// - activeOnly: only include Active branches (used for Create, where an inactive
        ///   branch should never even be selectable).
        /// - currentBranchId: the employee's current branch, if any — always included even
        ///   if inactive, and flagged so the view can leave it selectable while still
        ///   disabling *other* inactive branches.
        /// - excludeCurrentBranch: used for Transfer, where picking the same branch again
        ///   isn't a valid transfer target.
        /// </summary>
        private async Task PopulateBranchDropdownAsync(bool activeOnly, int? currentBranchId = null, bool excludeCurrentBranch = false)
        {
            var branches = await _branchService.GetAllBranchesAsync();

            var query = branches.AsEnumerable();

            if (activeOnly)
            {
                query = query.Where(b => b.BranchStatus == "Active" || b.BranchId == currentBranchId);
            }

            if (excludeCurrentBranch && currentBranchId.HasValue)
            {
                query = query.Where(b => b.BranchId != currentBranchId.Value);
            }

            ViewBag.Branches = query
                .Select(b => new
                {
                    b.BranchId,
                    b.BranchName,
                    IsActive = b.BranchStatus == "Active",
                    IsCurrent = b.BranchId == currentBranchId
                })
                .ToList();
        }
    }
}
