using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.Enums;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Validators;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository branchRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserService userService;
        private readonly IAuditLogService auditLogService;

        public BranchService(IBranchRepository branchRepository, IEmployeeRepository employeeRepository, IUserRepository userRepository, IUserService userService, IAuditLogService auditLogService)
        {
            this.branchRepository = branchRepository;
            this.employeeRepository = employeeRepository;
            this.userRepository = userRepository;
            this.userService = userService;
            this.auditLogService = auditLogService;
        }

        public async Task<IEnumerable<BranchListDto>> GetAllBranchesAsync()
        {
            var branches = await branchRepository.GetAllAsync();
            var employees = await employeeRepository.GetAllAsync();

            var managerNames = employees.ToDictionary(e => e.EmployeeId, e => $"{e.EmployeeFirstName} {e.EmployeeLastName}");

            return branches.Select(b => MapToListDto(b, managerNames));
        }

        public async Task<Branch?> GetBranchAsync(int id)
        {
            return await branchRepository.GetByIdAsync(id);
        }

        public async Task AddBranchAsync(Branch branch, int userId)
        {
            if (!await branchRepository.IsBranchCodeUniqueAsync(branch.BranchCode))
                throw new InvalidOperationException("Branch code already exists.");

            if (!string.IsNullOrWhiteSpace(branch.BranchEmail) &&
                !ContactValidator.IsValidEmail(branch.BranchEmail))
                throw new InvalidOperationException("Invalid email format.");

            if (!string.IsNullOrWhiteSpace(branch.BranchPhone) &&
                !ContactValidator.IsValidPhone(branch.BranchPhone))
                throw new InvalidOperationException("Invalid phone number.");

            if (!branch.BranchManager.HasValue)
                throw new InvalidOperationException("Please select a branch manager.");

            var employee = await employeeRepository.GetByIdAsync(branch.BranchManager.Value);

            if (employee == null)
                throw new InvalidOperationException("Selected employee does not exist.");

            if (employee.EmploymentStatus != EmploymentStatus.Active.ToString())
                throw new InvalidOperationException("Only active employees can become branch managers.");

            if (await branchRepository.IsBranchManagerAsync(employee.EmployeeId))
                throw new InvalidOperationException("Employee is already managing another branch.");

            branch.BranchStatus = BranchStatus.Active.ToString();

            await branchRepository.AddAsync(branch);
            await branchRepository.SaveChangesAsync();

            employee.EmployeeJobTitle = "Branch Manager";
            employee.EmployeeBranchId = branch.BranchId;

            employeeRepository.Update(employee);

            await userService.CreateBranchManagerAccountAsync(employee);
            await employeeRepository.SaveChangesAsync();

            await auditLogService.LogAsync(
                userId,
                "Create",
                "Branch",
                null,
                branch.BranchId);
        }

        public async Task UpdateBranchAsync(Branch branch, string? oldManagerNewJobTitle, int userId)
        {
            var existingBranch = await branchRepository.GetByIdAsync(branch.BranchId);

            if (existingBranch == null)
                throw new KeyNotFoundException("Branch not found.");

            bool isUnique = await branchRepository.IsBranchCodeUniqueAsync(
                branch.BranchCode,
                branch.BranchId);

            if (!isUnique)
                throw new InvalidOperationException("Branch code already exists.");

            if (!string.IsNullOrWhiteSpace(branch.BranchEmail) &&
                !ContactValidator.IsValidEmail(branch.BranchEmail))
                throw new InvalidOperationException("Invalid email format.");

            if (!string.IsNullOrWhiteSpace(branch.BranchPhone) &&
                !ContactValidator.IsValidPhone(branch.BranchPhone))
                throw new InvalidOperationException("Invalid phone number.");

            bool managerChanged = existingBranch.BranchManager != branch.BranchManager;

            if (managerChanged)
            {
                if (!branch.BranchManager.HasValue)
                    throw new InvalidOperationException("Please select a branch manager.");

                var newManager = await employeeRepository.GetByIdAsync(branch.BranchManager.Value);

                if (newManager == null)
                    throw new InvalidOperationException("Selected employee does not exist.");

                if (newManager.EmploymentStatus != EmploymentStatus.Active.ToString())
                    throw new InvalidOperationException("Only active employees can become branch managers.");

                if (await branchRepository.IsBranchManagerAsync(newManager.EmployeeId))
                    throw new InvalidOperationException("This employee is already managing another branch.");

                if (existingBranch.BranchManager.HasValue)
                {
                    var oldManager = await employeeRepository.GetByIdAsync(existingBranch.BranchManager.Value);

                    if (oldManager != null)
                    {
                        if (string.IsNullOrWhiteSpace(oldManagerNewJobTitle))
                            throw new InvalidOperationException("Please enter a new job title for the previous manager.");

                        oldManager.EmployeeJobTitle = oldManagerNewJobTitle;

                        employeeRepository.Update(oldManager);

                        var oldUser = await userRepository.GetByEmployeeIdAsync(oldManager.EmployeeId);

                        if (oldUser != null)
                        {
                            userRepository.Delete(oldUser);
                        }
                    }
                }

                newManager.EmployeeJobTitle = "Branch Manager";
                newManager.EmployeeBranchId = existingBranch.BranchId;

                employeeRepository.Update(newManager);

                await userService.CreateBranchManagerAccountAsync(newManager);

                existingBranch.BranchManager = newManager.EmployeeId;
            }

            existingBranch.BranchCode = branch.BranchCode;
            existingBranch.BranchName = branch.BranchName;
            existingBranch.BranchAddress = branch.BranchAddress;
            existingBranch.BranchCity = branch.BranchCity;
            existingBranch.BranchPhone = branch.BranchPhone;
            existingBranch.BranchEmail = branch.BranchEmail;
            existingBranch.BranchOpeningDate = branch.BranchOpeningDate;
            existingBranch.BranchStatus = branch.BranchStatus;

            branchRepository.Update(existingBranch);

            await branchRepository.SaveChangesAsync();

            await auditLogService.LogAsync(
                userId,
                "Edit",
                "Branch",
                null,
                branch.BranchId);
        }

        public async Task DeleteBranchAsync(int id, int userId)
        {
            var branch = await branchRepository.GetByIdAsync(id);

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            if (await branchRepository.HasActiveEmployeesAsync(id))
                throw new InvalidOperationException("Cannot delete a branch with active employees.");

            branchRepository.Delete(branch);
            await branchRepository.SaveChangesAsync();
            await auditLogService.LogAsync(
                userId,
                "Delete",
                "Branch",
                null,
                branch.BranchId);
        }

        public async Task UpdateBranchStatusAsync(int id, bool isActive, int userID)
        {
            var branch = await branchRepository.GetByIdAsync(id);

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            branch.BranchStatus = isActive
                ? BranchStatus.Active.ToString()
                : BranchStatus.Inactive.ToString();

            branchRepository.Update(branch);
            await branchRepository.SaveChangesAsync();

            await auditLogService.LogAsync(
                userID,
                branch.BranchStatus == BranchStatus.Active.ToString() ? "Activate" : "Deactivate",
                "Branch",
                null,
                branch.BranchId);
        }

        public async Task<BranchDetailsDto?> GetBranchDetailsAsync(int id)
        {
            var branch = await branchRepository.GetBranchWithEmployeesAsync(id);

            if (branch == null)
                return null;

            string? managerName = null;
            if (branch.BranchManager.HasValue)
            {
                var manager = await employeeRepository.GetByIdAsync(branch.BranchManager.Value);
                managerName = manager != null ? $"{manager.EmployeeFirstName} {manager.EmployeeLastName}" : null;
            }

            return new BranchDetailsDto
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode,
                BranchName = branch.BranchName,
                BranchAddress = branch.BranchAddress,
                BranchCity = branch.BranchCity,
                BranchPhone = branch.BranchPhone,
                BranchEmail = branch.BranchEmail,
                BranchOpeningDate = branch.BranchOpeningDate,
                BranchStatus = branch.BranchStatus,
                BranchManager = branch.BranchManager,
                BranchManagerName = managerName,
                Employees = branch.Employees?
                    .Select(e => new EmployeeSummaryDto
                    {
                        FullName = $"{e.EmployeeFirstName} {e.EmployeeLastName}",
                        EmployeeJobTitle = e.EmployeeJobTitle,
                        EmploymentStatus = e.EmploymentStatus
                    })
            .ToList()
            };
        }

        public async Task<bool> HasActiveEmployeesAsync(int id)
        {
            return await branchRepository.HasActiveEmployeesAsync(id);
        }

        public async Task<IEnumerable<BranchListDto>> SearchBranchesAsync(string searchTerm)
        {
            var branches = await branchRepository.SearchBranchesAsync(searchTerm);
            var employees = await employeeRepository.GetAllAsync();

            var managerNames = employees.ToDictionary(e => e.EmployeeId, e => $"{e.EmployeeFirstName} {e.EmployeeLastName}");

            return branches.Select(b => MapToListDto(b, managerNames));
        }

        public async Task<int> GetTotalBranchesAsync() => await branchRepository.GetTotalBranchesAsync();

        public async Task<int> GetActiveBranchesAsync() => await branchRepository.GetActiveBranchesAsync();

        public async Task<IEnumerable<Branch>> GetRecentlyAddedBranchesAsync() => await branchRepository.GetRecentlyAddedBranchesAsync();

        private static BranchListDto MapToListDto(Branch b, Dictionary<int, string> managerNames)
        {
            string? managerName = null;
            if (b.BranchManager.HasValue)
                managerNames.TryGetValue(b.BranchManager.Value, out managerName);

            return new BranchListDto
            {
                BranchId = b.BranchId,
                BranchCode = b.BranchCode,
                BranchName = b.BranchName,
                BranchCity = b.BranchCity,
                BranchStatus = b.BranchStatus,
                BranchManagerName = managerName,
                BranchManager = b.BranchManager

            };
        }
    }
}