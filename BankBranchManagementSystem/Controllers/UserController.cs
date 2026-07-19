using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        //private readonly IRoleService _roleService;

        public UserController(IUserService userService)
        {
            _userService = userService;
            //_roleService = roleService;
        }

        // ---------- "My Account" — any logged-in user manages their own info ----------

        // GET: /User/Profile
        public async Task<IActionResult> Profile()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _userService.GetUserAsync(currentUserId);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: /User/ChangeEmail
        public IActionResult ChangeEmail() => View();

        // POST: /User/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.ChangeEmailAsync(GetCurrentUserId(), model.NewEmail);
                return RedirectToAction(nameof(Profile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: /User/ChangePhoneNumber
        public IActionResult ChangePhoneNumber() => View();

        // POST: /User/ChangePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.ChangePhoneNumberAsync(GetCurrentUserId(), model.NewPhone);
                return RedirectToAction(nameof(Profile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: /User/ChangePassword
        public IActionResult ChangePassword() => View();

        // POST: /User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.ChangePasswordAsync(GetCurrentUserId(), model.CurrentPassword, model.NewPassword);
                return RedirectToAction(nameof(Profile));
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // ---------- Admin-only user management ----------

        // GET: /User/Index
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Index()
        //{
        //    var users = await _userService.GetAllUsersAsync();
        //    return View(users);
        //}

        // GET: /User/CreateAccount?employeeId=5
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateAccount(int employeeId)
        //{
        //    var model = new CreateUserAccountViewModel { EmployeeId = employeeId };
        //    await PopulateRoleDropdownAsync();
        //    return View(model);
        //}

        // POST: /User/CreateAccount
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateAccount(CreateUserAccountViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await PopulateRoleDropdownAsync();
        //        return View(model);
        //    }

        //    try
        //    {
        //        await _userService.CreateUserAccountAsync(
        //            GetCurrentUserId(),
        //            model.EmployeeId,
        //            model.Username,
        //            model.InitialPassword,
        //            model.RoleId);

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        await PopulateRoleDropdownAsync();
        //        return View(model);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Forbid();
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        await PopulateRoleDropdownAsync();
        //        return View(model);
        //    }
        //}

        // GET: /User/Delete/5
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var user = await _userService.GetUserAsync(id);
        //    if (user == null)
        //        return NotFound();

        //    return View(user);
        //}

        // POST: /User/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Administrator")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        await _userService.DeleteUserAsync(GetCurrentUserId(), id);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Forbid();
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        var user = await _userService.GetUserAsync(id);
        //        return View("Delete", user);
        //    }
        //}

        // ---------- helpers ----------

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        //private async Task PopulateRoleDropdownAsync()
        //{
        //    var roles = await _roleService.GetAllRolesAsync();
        //    ViewBag.Roles = roles
        //        .Select(r => new { r.RoleId, r.RoleName })
        //        .ToList();
        //}

        // GET: /User/CreateAdmin
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult CreateAdmin() => View();

        // POST: /User/CreateAdmin
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dto = new CreateAdminDto
                {
                    UserFirstName = model.UserFirstName,
                    UserLastName = model.UserLastName,
                    UserPassword = model.UserPassword,
                    UserEmail = model.UserEmail,
                    UserPhoneNumber = model.UserPhoneNumber
                };

                var created = await _userService.CreateAdminAccountAsync(GetCurrentUserId(), dto);

                TempData["Success"] = $"Admin account '{created.UserUsername}' created successfully.";
                return RedirectToAction(nameof(CreateAdmin));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }


    }



}