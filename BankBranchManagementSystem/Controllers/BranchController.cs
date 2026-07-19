using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IEmployeeService _employeeService;

        public BranchController(IBranchService branchService, IEmployeeService employeeService)
        {
            _branchService = branchService;
            _employeeService = employeeService;
        }

        // GET: /Branch
        //public async Task<IActionResult> Index()
        //{
        //    var branches = await _branchService.GetAllBranchesAsync();
        //    await PopulateDeleteFlagsAsync(branches);   
        //    return View("BranchManagement", branches);
        //}

        // GET: /Branch/Details/5  (full-page fallback / direct link)
        public async Task<IActionResult> Details(int id)
        {
            var branch = await _branchService.GetBranchDetailsAsync(id);
            if (branch == null)
                return NotFound();

            return View(branch);
        }

        // GET: /Branch/ViewModal/5  (fragment used by the Bootstrap modal on BranchManagement)
        public async Task<IActionResult> ViewModal(int id)
        {
            var branch = await _branchService.GetBranchDetailsAsync(id);
            if (branch == null)
                return NotFound();

            return PartialView("ViewBranch", branch);
        }

        // GET: /Branch/Create
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Create()
        {
            await PopulateManagerDropdownAsync();
            return View();
        }

        // POST: /Branch/Create
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBranchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateManagerDropdownAsync();
                return View(model);
            }

            var branch = new Branch
            {
                BranchCode = model.BranchCode,
                BranchName = model.BranchName,
                BranchAddress = model.BranchAddress,
                BranchCity = model.BranchCity,
                BranchPhone = model.BranchPhone,
                BranchEmail = model.BranchEmail,
                BranchManager = model.BranchManager,
                BranchOpeningDate = model.BranchOpeningDate,
            };

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _branchService.AddBranchAsync(branch, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateManagerDropdownAsync();
                return View(model);
            }
        }

        // GET: /Branch/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _branchService.GetBranchAsync(id);
            if (branch == null)
                return NotFound();

            var model = new EditBranchViewModel
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode ?? string.Empty,
                BranchName = branch.BranchName ?? string.Empty,
                BranchAddress = branch.BranchAddress,
                BranchCity = branch.BranchCity,
                BranchPhone = branch.BranchPhone,
                BranchEmail = branch.BranchEmail,
                BranchManager = branch.BranchManager,
                BranchOpeningDate = branch.BranchOpeningDate,
                BranchStatus = branch.BranchStatus
            };

            await PopulateManagerDropdownAsync(branch.BranchManager);
            return View(model);
        }

        // POST: /Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditBranchViewModel model)
        {
            if (id != model.BranchId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateManagerDropdownAsync();
                return View(model);
            }

            var branch = new Branch
            {
                BranchId = model.BranchId,
                BranchCode = model.BranchCode,
                BranchName = model.BranchName,
                BranchAddress = model.BranchAddress,
                BranchCity = model.BranchCity,
                BranchPhone = model.BranchPhone,
                BranchEmail = model.BranchEmail,
                BranchManager = model.BranchManager,
                BranchOpeningDate = model.BranchOpeningDate,
                BranchStatus = model.BranchStatus
            };

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _branchService.UpdateBranchAsync(branch, model.OldManagerNewJobTitle, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateManagerDropdownAsync(model.BranchManager);
                return View(model);
            }
        }

        // GET: /Branch/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _branchService.GetBranchAsync(id);
            if (branch == null)
                return NotFound();

            ViewBag.HasActiveEmployees = await _branchService.HasActiveEmployeesAsync(id);
            return View(branch);
        }

        // POST: /Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _branchService.DeleteBranchAsync(id, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                // e.g. "Cannot delete a branch with active employees."
                ModelState.AddModelError(string.Empty, ex.Message);
                var branch = await _branchService.GetBranchAsync(id);
                ViewBag.HasActiveEmployees = true;
                return View("Delete", branch);
            }
        }

        // POST: /Branch/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _branchService.UpdateBranchStatusAsync(id, true, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: /Branch/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _branchService.UpdateBranchStatusAsync(id, false, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: /Branch/Search?searchTerm=
        //public async Task<IActionResult> Search(string? searchTerm)
        //{
        //    var results = string.IsNullOrWhiteSpace(searchTerm)
        //        ? await _branchService.GetAllBranchesAsync()
        //        : await _branchService.SearchBranchesAsync(searchTerm);

        //    await PopulateDeleteFlagsAsync(results);   
        //    return View("BranchManagement", results);
        //}

        private async Task PopulateManagerDropdownAsync(int? currentManagerId = null)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var branches = await _branchService.GetAllBranchesAsync();

            ViewBag.Employees = employees.Select(e => new
            {
                e.EmployeeId,
                FullName = $"{e.EmployeeFirstName} {e.EmployeeLastName}",

                IsManager = branches.Any(b =>
                    b.BranchManager == e.EmployeeId &&
                    e.EmployeeId != currentManagerId)
            }).ToList();
        }

        private async Task PopulateDeleteFlagsAsync(IEnumerable<BranchListDto> branches)
        {
            var canDelete = new Dictionary<int, bool>();

            foreach (var branch in branches)
            {
                canDelete[branch.BranchId] = !await _branchService.HasActiveEmployeesAsync(branch.BranchId);
            }

            ViewBag.CanDelete = canDelete;
        }
        
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var result = await _branchService.GetBranchesPagedAsync(pageNumber, 10);
            await PopulateDeleteFlagsAsync(result.Items);
            return View("BranchManagement", result);
        }

        public async Task<IActionResult> Search(string? searchTerm, int pageNumber = 1)
        {
            var result = await _branchService.GetBranchesPagedAsync(pageNumber, 10, searchTerm);
            await PopulateDeleteFlagsAsync(result.Items);
            return View("BranchManagement", result);
        }
    }
}