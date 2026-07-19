using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.Enums;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Validators;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BankBranchManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository employeeRepository;

        private readonly IBranchRepository branchRepository;

        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService auditLogService;


        public EmployeeService(IEmployeeRepository employeeRepository, IBranchRepository branchRepository, IUserRepository userRepository, IAuditLogService auditLogService
)
        {
            this.employeeRepository = employeeRepository;
            this.branchRepository = branchRepository;
            this._userRepository = userRepository;
            this.auditLogService = auditLogService;
        }

        public async Task<IEnumerable<EmployeeListDto>> GetAllEmployeesAsync()
        {
            var employees = await employeeRepository.GetAllAsync();
            return employees.Select(MapToListDto);
        }

        public async Task<Employee?> GetEmployeeAsync(int id)
        {
            return await employeeRepository.GetByIdAsync(id);
        }

        public async Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int id)
        {
            var employee = await employeeRepository.GetEmployeeWithBranchAsync(id);

            return new EmployeeDetailsDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeFirstName = employee.EmployeeFirstName,
                EmployeeLastName = employee.EmployeeLastName,
                EmployeeJobTitle = employee.EmployeeJobTitle,
                EmployeePhone = employee.EmployeePhone,
                EmployeeEmail = employee.EmployeeEmail,
                EmployeeHireDate = employee.EmployeeHireDate,
                EmploymentStatus = employee.EmploymentStatus,
                EmployeeBranchId = employee.EmployeeBranchId,
                EmployeeBranchName = employee.EmployeeBranch?.BranchName
                
            };
        }


        public async Task AddEmployeeAsync(Employee employee, int userId)
        {
            var branch = await branchRepository.GetByIdAsync(employee.EmployeeBranchId);

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            if (branch.BranchStatus != BranchStatus.Active.ToString())
                throw new InvalidOperationException("Only active branches can receive new employees.");

            if (!string.IsNullOrWhiteSpace(employee.EmployeeEmail) && !ContactValidator.IsValidEmail(employee.EmployeeEmail))
                throw new InvalidOperationException("Invalid email format.");

            if (!string.IsNullOrWhiteSpace(employee.EmployeePhone) && !ContactValidator.IsValidPhone(employee.EmployeePhone))
                throw new InvalidOperationException("Invalid phone number.");

            employee.EmploymentStatus = EmploymentStatus.Active.ToString();

            if (!string.IsNullOrWhiteSpace(employee.EmployeeJobTitle) && employee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Branch Manager cannot be assigned as a job title here. Assign branch managers from Branch Management instead.");
            }

            await employeeRepository.AddAsync(employee);
            await employeeRepository.SaveChangesAsync();

            await auditLogService.LogAsync(new CreateAuditLogDto
            {
                UserId = userId,
                Action = "Create",
                EntityName = "Employee",
                EmployeeId = employee.EmployeeId,
                BranchId = employee.EmployeeBranchId
            });
        }

        //public async Task DeleteEmployeeAsync(int currentUserId, int employeeId, int? newManagerId)
        //{
        //    // User performing the action
        //    var currentUser = await userRepository.GetUserWithRoleAsync(currentUserId);

        //    if (currentUser == null)
        //        throw new KeyNotFoundException("Current user not found.");

        //    // Employee to delete
        //    var employee = await employeeRepository.GetByIdAsync(employeeId);

        //    if (employee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    // Check if this employee manages a branch
        //    var branch = await branchRepository.GetBranchByManagerAsync(employeeId);

        //    if (branch != null)
        //    {
        //        // Only Admins may delete branch managers
        //        if (currentUser.UserRole == null ||
        //            currentUser.UserRole.RoleName != RoleNames.Admin)
        //        {
        //            throw new UnauthorizedAccessException(
        //                "Only administrators can delete branch managers.");
        //        }

        //        // A replacement manager is required
        //        if (newManagerId == null)
        //            throw new InvalidOperationException(
        //                "A new branch manager must be assigned before deleting the current manager.");

        //        var newManager = await employeeRepository.GetByIdAsync(newManagerId.Value);

        //        if (newManager == null)
        //            throw new KeyNotFoundException("Replacement manager not found.");

        //        // Optional: require the replacement to belong to the same branch
        //        if (newManager.EmployeeBranchId != branch.BranchId)
        //            throw new InvalidOperationException(
        //                "The new manager must belong to the same branch.");

        //        if (newManager.EmploymentStatus != EmploymentStatus.Active.ToString())
        //            throw new InvalidOperationException(
        //                "Only active employees can become branch managers.");

        //        if (newManagerId == employeeId)
        //            throw new InvalidOperationException(
        //                "The replacement manager cannot be the employee being deleted.");

        //        if (await branchRepository.IsBranchManagerAsync(newManagerId.Value))
        //            throw new InvalidOperationException(
        //                "The replacement employee is already managing another branch.");

        //        // Assign the new manager
        //        branch.BranchManager = newManagerId.Value;
        //        branchRepository.Update(branch);
        //        await branchRepository.SaveChangesAsync();
        //    }

        //    // Delete the employee
        //    employeeRepository.Delete(employee);
        //    await employeeRepository.SaveChangesAsync();
        //}

        public async Task DeleteEmployeeAsync(int currentUserId, int employeeId, int? managerId)
        {
            var currentUser = await _userRepository.GetUserWithRoleAsync(currentUserId);

            if (currentUser == null)
                throw new KeyNotFoundException("Current user not found.");

            var employee = await employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var branch = await branchRepository.GetBranchByManagerAsync(employeeId);

            if (branch != null)
            {
                if (currentUser.UserRole == null ||
                    currentUser.UserRole.RoleName != RoleNames.Admin)
                {
                    throw new UnauthorizedAccessException(
                        "Only administrators can delete branch managers.");
                }

                branch.BranchManager = null;

                branchRepository.Update(branch);
                await branchRepository.SaveChangesAsync();
            }

            var user = await _userRepository.GetByEmployeeIdAsync(employeeId);

            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }

            var deletedEmployeeId = employee.EmployeeId;
            var deletedBranchId = employee.EmployeeBranchId;

            employeeRepository.Delete(employee);
            await employeeRepository.SaveChangesAsync();

            await auditLogService.LogAsync(new CreateAuditLogDto
            {
                UserId = currentUserId,
                Action = "Delete",
                EntityName = "Employee",
                EmployeeId = employee.EmployeeId,
                BranchId = employee.EmployeeBranchId
            });
        }
        public async Task TransferEmployeeAsync(int employeeId, int newBranchId, int userId)
        {
            var employee = await employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var managedBranch = await branchRepository.GetBranchByManagerAsync(employeeId);

            if (managedBranch != null)
            {
                throw new InvalidOperationException(
                    "Branch managers cannot be transferred. Assign a different manager first.");
            }

            var branch = await branchRepository.GetByIdAsync(newBranchId);
            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            if (branch.BranchStatus != BranchStatus.Active.ToString())
                throw new InvalidOperationException("Only active branches can receive new employees.");

            if (employee.EmployeeBranchId == newBranchId)
                throw new InvalidOperationException(
                    "Employee is already assigned to this branch.");

            employee.EmployeeBranchId = newBranchId;
            employeeRepository.Update(employee);
            await employeeRepository.SaveChangesAsync();

            await auditLogService.LogAsync(new CreateAuditLogDto
            {
                UserId = userId,
                Action = "Transfer",
                EntityName = "Employee",
                EmployeeId = employee.EmployeeId,
                BranchId = employee.EmployeeBranchId
            });
        }

        public async Task<IEnumerable<EmployeeListDto>> SearchEmployeesAsync(string? searchTerm)
        {
            var employees = await employeeRepository.SearchEmployeesAsync(searchTerm);
            return employees.Select(MapToListDto);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId)
        {
            return await employeeRepository.GetEmployeesByBranchAsync(branchId);

        }

        //public async Task UpdateEmployeeAsync(Employee employee)
        //{
        //    var existingEmployee = await employeeRepository.GetByIdAsync(employee.EmployeeId);

        //    if (existingEmployee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    // Validate contact information
        //    if (!string.IsNullOrWhiteSpace(employee.EmployeeEmail) && !ContactValidator.IsValidEmail(employee.EmployeeEmail))
        //        throw new InvalidOperationException("Invalid email format.");

        //    if (!string.IsNullOrWhiteSpace(employee.EmployeePhone) && !ContactValidator.IsValidPhone(employee.EmployeePhone))
        //        throw new InvalidOperationException("Invalid phone number.");

        //    // Only validate the branch if the branch is actually changing
        //    if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
        //    {
        //        var newBranch = await branchRepository.GetByIdAsync(employee.EmployeeBranchId);

        //        if (newBranch == null)
        //            throw new KeyNotFoundException("Branch not found.");

        //        if (newBranch.BranchStatus != BranchStatus.Active.ToString())
        //            throw new InvalidOperationException(
        //                "Employees can only be transferred to active branches.");
        //    }

        //    employeeRepository.Update(employee);
        //    await employeeRepository.SaveChangesAsync();
        //}

        //public async Task UpdateEmployeeAsync(Employee employee)
        //{
        //    var existingEmployee = await employeeRepository.GetByIdAsync(employee.EmployeeId);

        //    if (existingEmployee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    existingEmployee.EmployeeFirstName = employee.EmployeeFirstName;
        //    existingEmployee.EmployeeLastName = employee.EmployeeLastName;
        //    existingEmployee.EmployeeJobTitle = employee.EmployeeJobTitle;
        //    existingEmployee.EmployeePhone = employee.EmployeePhone;
        //    existingEmployee.EmployeeEmail = employee.EmployeeEmail;
        //    existingEmployee.EmployeeHireDate = employee.EmployeeHireDate;
        //    existingEmployee.EmployeeBranchId = employee.EmployeeBranchId;
        //    existingEmployee.EmploymentStatus = employee.EmploymentStatus;

        //    await employeeRepository.SaveChangesAsync();
        //}

        //public async Task UpdateEmployeeAsync(Employee employee, int userId)
        //{
        //    var existingEmployee = await employeeRepository.GetByIdAsync(employee.EmployeeId);

        //    if (existingEmployee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    // Prevent assigning Branch Manager through Employee Management
        //    if (!string.IsNullOrWhiteSpace(employee.EmployeeJobTitle) &&
        //        employee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase))
        //    {
        //        throw new InvalidOperationException(
        //            "Branch Manager cannot be assigned here. Assign branch managers from Branch Management instead.");
        //    }

        //    var managedBranch = await branchRepository.GetBranchByManagerAsync(employee.EmployeeId);

        //    bool isBranchManager = managedBranch != null;

        //    if (!string.IsNullOrWhiteSpace(employee.EmployeeEmail) &&
        //        !ContactValidator.IsValidEmail(employee.EmployeeEmail))
        //    {
        //        throw new InvalidOperationException("Invalid email format.");
        //    }

        //    if (!string.IsNullOrWhiteSpace(employee.EmployeePhone) &&
        //        !ContactValidator.IsValidPhone(employee.EmployeePhone))
        //    {
        //        throw new InvalidOperationException("Invalid phone number.");
        //    }



        //    if (isBranchManager)
        //    {
        //        // Branch managers cannot be transferred to another branch
        //        if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
        //        {
        //            throw new InvalidOperationException(
        //                "You cannot change a branch manager's location. Change their role first.");
        //        }

        //        // Job title changed -> remove as manager and delete login
        //        if (existingEmployee.EmployeeJobTitle != employee.EmployeeJobTitle)
        //        {
        //            managedBranch!.BranchManager = null;
        //            branchRepository.Update(managedBranch);
        //            await branchRepository.SaveChangesAsync();

        //            var user = await userRepository.GetByEmployeeIdAsync(existingEmployee.EmployeeId);

        //            if (user != null)
        //            {
        //                userRepository.Delete(user);
        //                await userRepository.SaveChangesAsync();
        //            }
        //        }
        //    }

        //    if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
        //    {
        //        var branch = await branchRepository.GetByIdAsync(employee.EmployeeBranchId);

        //        if (branch == null)
        //            throw new KeyNotFoundException("Branch not found.");

        //        if (branch.BranchStatus != BranchStatus.Active.ToString())
        //            throw new InvalidOperationException(
        //                "Employees can only be transferred to active branches.");
        //    }

        //    existingEmployee.EmployeeFirstName = employee.EmployeeFirstName;
        //    existingEmployee.EmployeeLastName = employee.EmployeeLastName;
        //    existingEmployee.EmployeeJobTitle = employee.EmployeeJobTitle;
        //    existingEmployee.EmployeePhone = employee.EmployeePhone;
        //    existingEmployee.EmployeeEmail = employee.EmployeeEmail;
        //    existingEmployee.EmployeeHireDate = employee.EmployeeHireDate;
        //    existingEmployee.EmployeeBranchId = employee.EmployeeBranchId;
        //    existingEmployee.EmploymentStatus = employee.EmploymentStatus;

        //    await employeeRepository.SaveChangesAsync();

        //    await auditLogService.LogAsync(
        //        userId,
        //        "Edit",
        //        "Employee",
        //        employee.EmployeeId,
        //        employee.EmployeeBranchId);
        //}


        //public async Task UpdateEmployeeAsync(Employee employee, int userId)
        //{
        //    var existingEmployee = await employeeRepository.GetByIdAsync(employee.EmployeeId);

        //    if (existingEmployee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    bool wasAlreadyManagerTitle =
        //        !string.IsNullOrWhiteSpace(existingEmployee.EmployeeJobTitle) &&
        //        existingEmployee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase);

        //    bool incomingTitleIsManager =
        //        !string.IsNullOrWhiteSpace(employee.EmployeeJobTitle) &&
        //        employee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase);

        //    // Only block this if the edit is *introducing* "Branch Manager" as the title.
        //    // If it was already their title (they were promoted via Branch Management),
        //    // leaving it unchanged here is fine — that's just a normal edit of a manager's
        //    // other fields (phone, email, hire date, etc).
        //    if (incomingTitleIsManager && !wasAlreadyManagerTitle)
        //    {
        //        throw new InvalidOperationException(
        //            "Branch Manager cannot be assigned here. Assign branch managers from Branch Management instead.");
        //    }

        //    var managedBranch = await branchRepository.GetBranchByManagerAsync(employee.EmployeeId);

        //    bool isBranchManager = managedBranch != null;

        //    if (!string.IsNullOrWhiteSpace(employee.EmployeeEmail) &&
        //        !ContactValidator.IsValidEmail(employee.EmployeeEmail))
        //    {
        //        throw new InvalidOperationException("Invalid email format.");
        //    }

        //    if (!string.IsNullOrWhiteSpace(employee.EmployeePhone) &&
        //        !ContactValidator.IsValidPhone(employee.EmployeePhone))
        //    {
        //        throw new InvalidOperationException("Invalid phone number.");
        //    }

        //    if (isBranchManager)
        //    {
        //        // Branch managers cannot be transferred to another branch
        //        if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
        //        {
        //            throw new InvalidOperationException(
        //                "You cannot change a branch manager's location. Change their role first.");
        //        }

        //        // Job title changed -> remove as manager and delete login
        //        if (existingEmployee.EmployeeJobTitle != employee.EmployeeJobTitle)
        //        {
        //            managedBranch!.BranchManager = null;
        //            branchRepository.Update(managedBranch);
        //            await branchRepository.SaveChangesAsync();

        //            var user = await userRepository.GetByEmployeeIdAsync(existingEmployee.EmployeeId);

        //            if (user != null)
        //            {
        //                userRepository.Delete(user);
        //                await userRepository.SaveChangesAsync();
        //            }
        //        }
        //    }

        //    if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
        //    {
        //        var branch = await branchRepository.GetByIdAsync(employee.EmployeeBranchId);

        //        if (branch == null)
        //            throw new KeyNotFoundException("Branch not found.");

        //        if (branch.BranchStatus != BranchStatus.Active.ToString())
        //            throw new InvalidOperationException(
        //                "Employees can only be transferred to active branches.");
        //    }

        //    existingEmployee.EmployeeFirstName = employee.EmployeeFirstName;
        //    existingEmployee.EmployeeLastName = employee.EmployeeLastName;
        //    existingEmployee.EmployeeJobTitle = employee.EmployeeJobTitle;
        //    existingEmployee.EmployeePhone = employee.EmployeePhone;
        //    existingEmployee.EmployeeEmail = employee.EmployeeEmail;
        //    existingEmployee.EmployeeHireDate = employee.EmployeeHireDate;
        //    existingEmployee.EmployeeBranchId = employee.EmployeeBranchId;
        //    existingEmployee.EmploymentStatus = employee.EmploymentStatus;

        //    await employeeRepository.SaveChangesAsync();

        //    await auditLogService.LogAsync(
        //     userId,
        //     "Edit",
        //     "Employee",
        //     employee.EmployeeId,
        //     employee.EmployeeBranchId);
        //    }

        public async Task UpdateEmployeeAsync(Employee employee, int userId)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(employee.EmployeeId);

            if (existingEmployee == null)
                throw new KeyNotFoundException("Employee not found.");

            bool wasAlreadyManagerTitle =
                !string.IsNullOrWhiteSpace(existingEmployee.EmployeeJobTitle) &&
                existingEmployee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase);

            bool incomingTitleIsManager =
                !string.IsNullOrWhiteSpace(employee.EmployeeJobTitle) &&
                employee.EmployeeJobTitle.Trim().Equals("Branch Manager", StringComparison.OrdinalIgnoreCase);

            // Only block this if the edit is *introducing* "Branch Manager" as the title.
            // If it was already their title (they were promoted via Branch Management),
            // leaving it unchanged here is fine.
            if (incomingTitleIsManager && !wasAlreadyManagerTitle)
            {
                throw new InvalidOperationException(
                    "Branch Manager cannot be assigned here. Assign branch managers from Branch Management instead.");
            }

            var managedBranch = await branchRepository.GetBranchByManagerAsync(employee.EmployeeId);

            bool isBranchManager = managedBranch != null;

            if (!string.IsNullOrWhiteSpace(employee.EmployeeEmail) &&
                !ContactValidator.IsValidEmail(employee.EmployeeEmail))
            {
                throw new InvalidOperationException("Invalid email format.");
            }

            if (!string.IsNullOrWhiteSpace(employee.EmployeePhone) &&
                !ContactValidator.IsValidPhone(employee.EmployeePhone))
            {
                throw new InvalidOperationException("Invalid phone number.");
            }

            bool titleChanged = existingEmployee.EmployeeJobTitle != employee.EmployeeJobTitle;

            if (isBranchManager)
            {
                // Only lock the branch if they're staying a manager. If the role is
                // changing away from "Branch Manager" in this same request, the branch
                // is allowed to change too — they're being demoted and moved at once.
                if (!titleChanged && existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
                {
                    throw new InvalidOperationException(
                        "You cannot change a branch manager's location. Change their role first.");
                }

                // Job title changed -> remove as manager and delete login
                if (titleChanged)
                {
                    managedBranch!.BranchManager = null;
                    branchRepository.Update(managedBranch);
                    await branchRepository.SaveChangesAsync();

                    var user = await _userRepository.GetByEmployeeIdAsync(existingEmployee.EmployeeId);

                    if (user != null)
                    {
                        _userRepository.Delete(user);
                        await _userRepository.SaveChangesAsync();
                    }
                }
            }

            if (existingEmployee.EmployeeBranchId != employee.EmployeeBranchId)
            {
                var branch = await branchRepository.GetByIdAsync(employee.EmployeeBranchId);

                if (branch == null)
                    throw new KeyNotFoundException("Branch not found.");

                if (branch.BranchStatus != BranchStatus.Active.ToString())
                    throw new InvalidOperationException(
                        "Employees can only be transferred to active branches.");
            }

            existingEmployee.EmployeeFirstName = employee.EmployeeFirstName;
            existingEmployee.EmployeeLastName = employee.EmployeeLastName;
            existingEmployee.EmployeeJobTitle = employee.EmployeeJobTitle;
            existingEmployee.EmployeePhone = employee.EmployeePhone;
            existingEmployee.EmployeeEmail = employee.EmployeeEmail;
            existingEmployee.EmployeeHireDate = employee.EmployeeHireDate;
            existingEmployee.EmployeeBranchId = employee.EmployeeBranchId;
            existingEmployee.EmploymentStatus = employee.EmploymentStatus;

            await employeeRepository.SaveChangesAsync();

            await auditLogService.LogAsync(new CreateAuditLogDto
            {
                UserId = userId,
                Action = "Edit",
                EntityName = "Employee",
                EmployeeId = employee.EmployeeId,
                BranchId = employee.EmployeeBranchId
            });
        }

        // ... rest unchanged




        public async Task<int> GetTotalEmployeesAsync() => await employeeRepository.GetTotalEmployeesAsync();

        public async Task<Dictionary<int, int>> GetEmployeeCountsByBranchAsync() => await employeeRepository.GetEmployeeCountsByBranchAsync();

        private static EmployeeListDto MapToListDto(Employee e) => new EmployeeListDto
        {
            EmployeeId = e.EmployeeId,
            EmployeeFirstName = e.EmployeeFirstName,
            EmployeeLastName = e.EmployeeLastName,
            EmployeeJobTitle = e.EmployeeJobTitle,
            EmploymentStatus = e.EmploymentStatus,
            EmployeeBranchId = e.EmployeeBranchId,                  // NEW — comes straight off the entity, no navigation needed
            EmployeeBranchName = e.EmployeeBranch?.BranchName,
            EmployeeBranchStatus = e.EmployeeBranch?.BranchStatus,
            EmployeeBranchManagerId = e.EmployeeBranch?.BranchManager
        };

        public async Task<PagedResult<EmployeeListDto>> GetEmployeesPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (employees, totalCount) = await employeeRepository.SearchEmployeesPagedAsync(searchTerm, pageNumber, pageSize);

            return new PagedResult<EmployeeListDto>
            {
                Items = employees.Select(MapToListDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchTerm = searchTerm
            };
        }
    }
}
