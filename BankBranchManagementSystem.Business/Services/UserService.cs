using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Dtos;
using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Repositories;
using BankBranchManagementSystem.Validators;
using System.Text.RegularExpressions;

namespace BankBranchManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IAuditLogService auditLogService;

        public UserService(IUserRepository userRepository, IEmployeeRepository employeeRepository, IRoleRepository roleRepository, IAuditLogService auditLogService)
        {
            this.userRepository = userRepository;
            this.employeeRepository = employeeRepository;
            this.roleRepository = roleRepository;
            this.auditLogService = auditLogService;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await userRepository.GetByUsernameAsync(username);

            if (user == null)
                return null;

            if (user.UserPassword != password)
                return null;

            return user;
        }


        //Edited the GetUserAsync to implement DTOs
        public async Task<UserDto?> GetUserAsync(int id)
        {
            var user = await userRepository.GetUserWithRoleAsync(id); // includes UserRole
            return user == null ? null : MapToDto(user);
        }


        //public async Task<User?> GetUserAsync(int id)
        //{
        //    return await userRepository.GetByIdAsync(id);
        //}

        //public async Task<IEnumerable<User>> GetAllUsersAsync()
        //{
        //    return await userRepository.GetAllAsync();
        //}


        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            if (user.UserPassword != currentPassword)
                throw new UnauthorizedAccessException("Current password is incorrect.");
            user.UserPassword = newPassword;
            userRepository.Update(user);
            await userRepository.SaveChangesAsync();
        }


        public async Task ChangeEmailAsync(int userId, string newEmail)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(newEmail))
                throw new InvalidOperationException("Email is required.");

            if (!ContactValidator.IsValidEmail(newEmail))
                throw new InvalidOperationException("Invalid email format.");

            if (await userRepository.EmailExistsAsync(newEmail, userId))
                throw new InvalidOperationException("Email already exists.");

            user.UserEmail = newEmail;

            userRepository.Update(user);

            await userRepository.SaveChangesAsync();
        }


        public async Task ChangePhoneNumberAsync(int userId, string newPhone)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(newPhone))
                throw new InvalidOperationException("Phone number is required.");

            if (!ContactValidator.IsValidPhone(newPhone))
                throw new InvalidOperationException("Invalid phone number.");

            if (await userRepository.PhoneExistsAsync(newPhone, userId))
                throw new InvalidOperationException("Phone number already exists.");



            user.UserPhoneNumber = newPhone;

            userRepository.Update(user);

            await userRepository.SaveChangesAsync();
        }

        //public async Task DeleteUserAsync(int adminId, int userId)
        //{
        //    // Get the admin performing the delete
        //    var admin = await userRepository.GetUserWithRoleAsync(adminId);

        //    if (admin == null)
        //        throw new KeyNotFoundException("Admin not found.");

        //    // Make sure the user performing the action is an Admin
        //    if (admin.UserRole == null || admin.UserRole.RoleName != "Admin")
        //        throw new UnauthorizedAccessException("Only administrators can delete users.");

        //    // Get the user to delete
        //    var user = await userRepository.GetUserWithRoleAsync(userId);

        //    if (user == null)
        //        throw new KeyNotFoundException("User not found.");

        //    // Only Branch Managers can be deleted
        //    if (user.UserRole == null || user.UserRole.RoleName != "Branch Manager")
        //        throw new InvalidOperationException("Only Branch Managers can be deleted.");

        //    // Delete the user
        //    userRepository.Delete(user);
        //    await userRepository.SaveChangesAsync();
        //}


        //public async Task<User> CreateUserAccountAsync(int adminId, int employeeId, string username, string initialPassword, int roleId)
        //{
        //    var admin = await userRepository.GetUserWithRoleAsync(adminId);
        //    if (admin == null)
        //        throw new KeyNotFoundException("Admin not found.");

        //    if (admin.UserRole == null || admin.UserRole.RoleName != "Admin")
        //        throw new UnauthorizedAccessException("Only administrators can create user accounts.");

        //    var employee = await employeeRepository.GetByIdAsync(employeeId);
        //    if (employee == null)
        //        throw new KeyNotFoundException("Employee not found.");

        //    var existingAccount = await userRepository.GetByEmployeeIdAsync(employeeId);
        //    if (existingAccount != null)
        //        throw new InvalidOperationException("This employee already has a user account.");

        //    if (string.IsNullOrWhiteSpace(username))
        //        throw new InvalidOperationException("Username is required.");

        //    if (await userRepository.UsernameExistsAsync(username))
        //        throw new InvalidOperationException("Username already exists.");

        //    var role = await roleRepository.GetByIdAsync(roleId);
        //    if(role == null)
        //        throw new KeyNotFoundException("Role not found.");

        //    var newUser = new User
        //    {
        //        EmployeeId = employeeId,
        //        UserUsername = username,
        //        UserPassword = initialPassword, 
        //        UserRoleId = roleId
        //    };

        //    await userRepository.AddAsync(newUser);
        //    await userRepository.SaveChangesAsync();
        //    return newUser;
        //}



        public async Task CreateBranchManagerAccountAsync(Employee employee)
        {
            var existing = await userRepository.GetByEmployeeIdAsync(employee.EmployeeId);

            if (existing != null)
                return;

            var username =
                $"{employee.EmployeeFirstName}.{employee.EmployeeLastName}"
                    .Replace(" ", "")
                    .ToLower();

            if (await userRepository.UsernameExistsAsync(username))
                username += employee.EmployeeId;

            var user = new User
            {
                EmployeeId = employee.EmployeeId,

                UserFirstName = employee.EmployeeFirstName,
                UserLastName = employee.EmployeeLastName,

                UserEmail = employee.EmployeeEmail,
                UserPhoneNumber = employee.EmployeePhone,

                UserRoleId = 2,

                UserUsername = username,

                UserPassword = "123456"
            };

            await userRepository.AddAsync(user);
        }

        //Adding a private Mapper
        private static UserDto MapToDto(User u) => new UserDto
        {
            UserId = u.UserId,
            UserFirstName = u.UserFirstName,
            UserLastName = u.UserLastName,
            UserUsername = u.UserUsername,
            UserEmail = u.UserEmail,
            UserPhoneNumber = u.UserPhoneNumber,
            UserRoleId = u.UserRoleId,
            UserRoleName = u.UserRole?.RoleName,
            EmployeeId = u.EmployeeId,
            EmployeeFullName = u.Employee != null
        ? $"{u.Employee.EmployeeFirstName} {u.Employee.EmployeeLastName}"
        : null
        };


        public async Task<UserDto> CreateAdminAccountAsync(int requestingAdminId, CreateAdminDto dto)
        {
            var requestingAdmin = await userRepository.GetUserWithRoleAsync(requestingAdminId);
            if (requestingAdmin == null)
                throw new KeyNotFoundException("Requesting user not found.");

            if (requestingAdmin.UserRole == null || requestingAdmin.UserRole.RoleName != RoleNames.Admin)
                throw new UnauthorizedAccessException("Only administrators can create admin accounts.");

            if (string.IsNullOrWhiteSpace(dto.UserPassword))
                throw new InvalidOperationException("Password is required.");

            if (string.IsNullOrWhiteSpace(dto.UserEmail))
                throw new InvalidOperationException("Email is required.");

            if (!ContactValidator.IsValidEmail(dto.UserEmail))
                throw new InvalidOperationException("Invalid email format.");

            if (await userRepository.EmailExistsAsync(dto.UserEmail, null))
                throw new InvalidOperationException("Email already exists.");

            if (string.IsNullOrWhiteSpace(dto.UserPhoneNumber))
                throw new InvalidOperationException("Phone number is required.");

            if (!ContactValidator.IsValidPhone(dto.UserPhoneNumber))
                throw new InvalidOperationException("Invalid phone number.");

            if (await userRepository.PhoneExistsAsync(dto.UserPhoneNumber, null))
                throw new InvalidOperationException("Phone number already exists.");
            var adminRole = await roleRepository.GetRoleByNameAsync(RoleNames.Admin);
            if (adminRole == null)
                throw new KeyNotFoundException("Admin role not found in database.");

            var username = await GenerateNextAdminUsernameAsync();

            var newAdmin = new User
            {
                EmployeeId = null,
                UserFirstName = dto.UserFirstName,
                UserLastName = dto.UserLastName,
                UserUsername = username,
                UserPassword = dto.UserPassword, // see note below re: hashing
                UserRoleId = adminRole.RoleId,
                UserEmail = dto.UserEmail,
                UserPhoneNumber = dto.UserPhoneNumber
            };

            await userRepository.AddAsync(newAdmin);
            await userRepository.SaveChangesAsync();
            await auditLogService.LogAsync(new CreateAuditLogDto
            {
                UserId = requestingAdminId,
                Action = "Create",
                EntityName = "Employee",
                // EmployeeId and BranchId are left null — this action isn't tied to either
            });


            return MapToDto(newAdmin);
        }

        private async Task<string> GenerateNextAdminUsernameAsync()
        {
            var existingUsernames = await userRepository.GetUsernamesByPrefixAsync("admin");

            int highestSuffix = 0; // bare "admin" is treated as suffix 1
            foreach (var uname in existingUsernames)
            {
                if (string.Equals(uname, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    highestSuffix = Math.Max(highestSuffix, 1);
                    continue;
                }

                var match = Regex.Match(uname, @"^admin(\d+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var num = int.Parse(match.Groups[1].Value);
                    highestSuffix = Math.Max(highestSuffix, num);
                }
            }

            return $"admin{highestSuffix + 1}";
        }

    }
}