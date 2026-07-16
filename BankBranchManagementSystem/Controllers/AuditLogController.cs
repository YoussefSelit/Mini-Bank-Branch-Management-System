using BankBranchManagementSystem.Constants;
using BankBranchManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            this.auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await auditLogService.GetAllAsync();
            return View(logs);
        }
    }
}