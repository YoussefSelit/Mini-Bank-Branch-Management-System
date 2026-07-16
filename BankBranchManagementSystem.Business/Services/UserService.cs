using BankBranchManagementSystem.Interfaces;
using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Repositories;
using BankBranchManagementSystem.Validators;

namespace BankBranchManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IRoleRepository roleRepository;

        public UserService(IUserRepository userRepository, IEmployeeRepository employeeRepository, IRoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            this.employeeRepository = employeeRepository;
            this.roleRepository = roleRepository;
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

        public async Task<User?> GetUserAsync(int id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await userRepository.GetAllAsync();
        }


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

        public async Task DeleteUserAsync(int adminId, int userId)
        {
            // Get the admin performing the delete
            var admin = await userRepository.GetUserWithRoleAsync(adminId);

            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");

            // Make sure the user performing the action is an Admin
            if (admin.UserRole == null || admin.UserRole.RoleName != "Admin")
                throw new UnauthorizedAccessException("Only administrators can delete users.");

            // Get the user to delete
            var user = await userRepository.GetUserWithRoleAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Only Branch Managers can be deleted
            if (user.UserRole == null || user.UserRole.RoleName != "Branch Manager")
                throw new InvalidOperationException("Only Branch Managers can be deleted.");

            // Delete the user
            userRepository.Delete(user);
            await userRepository.SaveChangesAsync();
        }


        public async Task<User> CreateUserAccountAsync(int adminId, int employeeId, string username, string initialPassword, int roleId)
        {
            var admin = await userRepository.GetUserWithRoleAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");

            if (admin.UserRole == null || admin.UserRole.RoleName != "Admin")
                throw new UnauthorizedAccessException("Only administrators can create user accounts.");

            var employee = await employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var existingAccount = await userRepository.GetByEmployeeIdAsync(employeeId);
            if (existingAccount != null)
                throw new InvalidOperationException("This employee already has a user account.");

            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Username is required.");

            if (await userRepository.UsernameExistsAsync(username))
                throw new InvalidOperationException("Username already exists.");

            var role = await roleRepository.GetByIdAsync(roleId);
            if(role == null)
                throw new KeyNotFoundException("Role not found.");

            var newUser = new User
            {
                EmployeeId = employeeId,
                UserUsername = username,
                UserPassword = initialPassword, 
                UserRoleId = roleId
            };

            await userRepository.AddAsync(newUser);
            await userRepository.SaveChangesAsync();
            return newUser;
        }



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
    }
}
