using BankBranchManagementSystem.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace BankBranchManagementSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IEmployeeService _employeeService;

        public ReportController(IBranchService branchService, IEmployeeService employeeService)
        {
            _branchService = branchService;
            _employeeService = employeeService;
        }

        // GET: /Report/GenerateReport
        public IActionResult GenerateReport() => View();

        // POST: /Report/Export
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(string reportType, string exportFormat)
        {
            // Server-side guard: never trust that the client-side dropdown
            // validation actually ran (JS can be disabled, the request can
            // be replayed, etc.) — if either choice is missing, bounce back
            // to the form with an error instead of generating a report.
            if (string.IsNullOrWhiteSpace(reportType) || string.IsNullOrWhiteSpace(exportFormat))
            {
                ModelState.AddModelError(string.Empty, "Please choose both a report type and an export format before exporting.");
                return View("GenerateReport");
            }

            (string[] headers, List<string[]> rows) data;
            try
            {
                data = await BuildReportAsync(reportType);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("GenerateReport");
            }

            var fileBaseName = reportType.Replace(" ", "_");

            return exportFormat switch
            {
                "Export as PDF" => ExportAsPdf(reportType, data.headers, data.rows, fileBaseName),
                "Export as Excel sheet" => ExportAsExcel(reportType, data.headers, data.rows, fileBaseName),
                _ => throw new ArgumentException("Unknown export format selected.")
            };
        }

        private async Task<(string[] Headers, List<string[]> Rows)> BuildReportAsync(string reportType)
        {
            switch (reportType)
            {
                case "Branch List":
                    {
                        var branches = await _branchService.GetAllBranchesAsync();
                        var headers = new[] { "Branch Code", "Branch Name", "City", "Status" };
                        var rows = branches
                            .Select(b => new[]
                            {
                                b.BranchCode ?? "-",
                                b.BranchName ?? "-",
                                b.BranchCity ?? "-",
                                b.BranchStatus ?? "-"
                            })
                            .ToList();
                        return (headers, rows);
                    }

                case "List of Active Branches":
                    {
                        var branches = await _branchService.GetAllBranchesAsync();
                        var headers = new[] { "Branch Code", "Branch Name", "City", "Opening Date" };
                        var rows = branches
                            .Where(b => b.BranchStatus == "Active")
                            .Select(b => new[]
                            {
                                b.BranchCode ?? "-",
                                b.BranchName ?? "-",
                                b.BranchCity ?? "-",
                                b.BranchOpeningDate?.ToString("yyyy-MM-dd") ?? "-"
                            })
                            .ToList();
                        return (headers, rows);
                    }

                case "List of Employees by Branch":
                    {
                        var branchNames = (await _branchService.GetAllBranchesAsync())
                            .ToDictionary(b => b.BranchId, b => b.BranchName ?? "-");
                        var employees = await _employeeService.GetAllEmployeesAsync();

                        var headers = new[] { "Branch", "Employee Name", "Job Title", "Status" };
                        var rows = employees
                            .OrderBy(e => branchNames.TryGetValue(e.EmployeeBranchId, out var name) ? name : "")
                            .Select(e => new[]
                            {
                                branchNames.TryGetValue(e.EmployeeBranchId, out var name) ? name : "Unknown",
                                $"{e.EmployeeFirstName} {e.EmployeeLastName}",
                                e.EmployeeJobTitle ?? "-",
                                e.EmploymentStatus ?? "-"
                            })
                            .ToList();
                        return (headers, rows);
                    }

                case "Employee Distribution Report":
                    {
                        var branches = await _branchService.GetAllBranchesAsync();
                        var counts = await _employeeService.GetEmployeeCountsByBranchAsync();

                        var headers = new[] { "Branch", "Employee Count" };
                        var rows = branches
                            .Select(b => new[]
                            {
                                b.BranchName ?? "-",
                                (counts.TryGetValue(b.BranchId, out var count) ? count : 0).ToString()
                            })
                            .ToList();
                        return (headers, rows);
                    }

                default:
                    throw new ArgumentException($"'{reportType}' is not a recognized report type.");
            }
        }

        private FileResult ExportAsPdf(string title, string[] headers, List<string[]> rows, string fileBaseName)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Column(col =>
                    {
                        col.Item().Text(title).FontSize(18).SemiBold();
                        col.Item().Text($"Generated {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in headers)
                                columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (var h in headers)
                            {
                                header.Cell().BorderBottom(1).Padding(4).Text(h).SemiBold();
                            }
                        });

                        foreach (var row in rows)
                        {
                            foreach (var cell in row)
                            {
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(cell);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            });

            var bytes = document.GeneratePdf();
            return File(bytes, "application/pdf", $"{fileBaseName}.pdf");
        }

        private FileResult ExportAsExcel(string title, string[] headers, List<string[]> rows, string fileBaseName)
        {
            using var workbook = new XLWorkbook();
            var sheetName = title.Length > 31 ? title[..31] : title; // Excel sheet-name length limit
            var worksheet = workbook.Worksheets.Add(sheetName);

            for (var c = 0; c < headers.Length; c++)
            {
                var cell = worksheet.Cell(1, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
            }

            for (var r = 0; r < rows.Count; r++)
            {
                for (var c = 0; c < rows[r].Length; c++)
                {
                    worksheet.Cell(r + 2, c + 1).Value = rows[r][c];
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{fileBaseName}.xlsx");
        }
    }
}
