using Bogus;
using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Data;
using BankBranchManagementSystem.Enums;
using BankBranchManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankBranchManagementSystem.Seed
{
    public static class DummyDataSeeder
    {
        public static async Task SeedAsync(BankDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;


            //Seed Roles
            var adminRole = new Role
            {
                RoleName = RoleNames.Admin
            };

            var managerRole = new Role
            {
                RoleName = RoleNames.BranchManager
            };

            context.Roles.AddRange(adminRole, managerRole);
            await context.SaveChangesAsync();

            //Seed Branch
            var branchFaker = new Faker<Branch>()
                 .RuleFor(b => b.BranchCode, f => $"BR{1000 + f.UniqueIndex}")
                 .RuleFor(b => b.BranchName, f => $"{f.Address.City()} Branch")
                 .RuleFor(b => b.BranchAddress, f => f.Address.StreetAddress())
                 .RuleFor(b => b.BranchCity, f => f.Address.City())
                 .RuleFor(b => b.BranchPhone, f => $"01{f.Random.ReplaceNumbers("#########")}")
                 .RuleFor(b => b.BranchEmail, f => f.Internet.Email())
                 .RuleFor(b => b.BranchOpeningDate,
                     f => DateOnly.FromDateTime(f.Date.Past(15)))
                 .RuleFor(b => b.BranchStatus,
                     BranchStatus.Active.ToString());

            var branches = branchFaker.Generate(20);

            context.Branches.AddRange(branches);

            await context.SaveChangesAsync();



            //Generate Employees
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.EmployeeFirstName, f => f.Name.FirstName())
                .RuleFor(e => e.EmployeeLastName, f => f.Name.LastName())
                .RuleFor(e => e.EmployeeJobTitle, f => f.Name.JobTitle())
                .RuleFor(e => e.EmployeePhone,
                    f => $"01{f.Random.ReplaceNumbers("#########")}")
                .RuleFor(e => e.EmployeeEmail, f => f.Internet.Email())
                .RuleFor(e => e.EmployeeHireDate,
                    f => DateOnly.FromDateTime(f.Date.Past(8)))
                .RuleFor(e => e.EmployeeBranchId,
                    f => f.PickRandom(branches).BranchId)
                .RuleFor(e => e.EmploymentStatus,
                    EmploymentStatus.Active.ToString());

            var employees = employeeFaker.Generate(1000);

            context.Employees.AddRange(employees);

            await context.SaveChangesAsync();


            //Assign one manager to each branch
            
            var random = new Random();

            foreach (var branch in branches)
            {
                var branchEmployees = employees
                    .Where(e => e.EmployeeBranchId == branch.BranchId)
                    .ToList();

                if (!branchEmployees.Any())
                    continue;

                var manager = branchEmployees[random.Next(branchEmployees.Count)];
                branch.BranchManager = manager.EmployeeId;
            }

            context.Branches.UpdateRange(branches);
            await context.SaveChangesAsync();




            //Create Admin User
            var adminUser = new User
            {
                UserFirstName = "System",
                UserLastName = "Administrator",
                UserUsername = "admin",
                UserPassword = "admin123", // Plain text for now
                UserEmail = "admin@bank.com",
                UserPhoneNumber = "01000000000",
                UserRoleId = adminRole.RoleId,
                EmployeeId = null // Admin is not linked to an employee
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();


            //Create Manager Users
            var managerUsers = new List<User>();

            foreach (var branch in branches)
            {
                var manager = employees.FirstOrDefault(e => e.EmployeeId == branch.BranchManager);

                if (manager == null)
                    continue;

                managerUsers.Add(new User
                {
                    UserFirstName = manager.EmployeeFirstName,
                    UserLastName = manager.EmployeeLastName,
                    UserUsername = $"manager{manager.EmployeeId}",
                    UserPassword = "manager123",   // Plain text for now
                    UserEmail = manager.EmployeeEmail,
                    UserPhoneNumber = manager.EmployeePhone,
                    UserRoleId = managerRole.RoleId,
                    EmployeeId = manager.EmployeeId
                });
            }

            context.Users.AddRange(managerUsers);
            await context.SaveChangesAsync();


            //Generating Audit logs

            var users = await context.Users.ToListAsync();

            var auditLogFaker = new Faker<AuditLog>()
                .RuleFor(a => a.UserId, f => f.PickRandom(users).UserId)
                .RuleFor(a => a.EmployeeId, f => f.PickRandom(employees).EmployeeId)
                .RuleFor(a => a.BranchId, f => f.PickRandom(branches).BranchId)
                .RuleFor(a => a.EntityName, f => f.PickRandom(
                    "Employee",
                    "Branch",
                    "User"))
                .RuleFor(a => a.Action, f => f.PickRandom(
                    "Created",
                    "Updated",
                    "Deleted",
                    "Transferred",
                    "Login"))
                .RuleFor(a => a.ActionDate, f => f.Date.Recent(90));

            var auditLogs = auditLogFaker.Generate(5000);

            context.AuditLogs.AddRange(auditLogs);
            await context.SaveChangesAsync();

        }
    }
}