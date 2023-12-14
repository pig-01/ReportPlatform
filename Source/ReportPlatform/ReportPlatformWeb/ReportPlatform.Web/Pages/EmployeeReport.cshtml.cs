using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReportPlatform.BAL.Factories;
using ReportPlatform.BAL.Models;

namespace ReportPlatform.Web.Pages
{
    [Authorize]
    public class EmployeeReportModel(ILogger<EmployeeReportModel> logger, IReportFactory reportFactory) : PageModel
    {
        private readonly ILogger<EmployeeReportModel> logger = logger;
        private readonly IReportFactory reportFactory = reportFactory;

        public MarkupString htmlContent;

        public async Task OnGet()
        {
            logger.LogInformation("Get Employee Report with GET type");

            IReport report = reportFactory.GetReport("EmployeesReport");

            htmlContent = new MarkupString(await report.GetEmbedReportViewer());
        }

        public async Task<IActionResult> OnPost()
        {
            logger.LogInformation("Get Employee Report with POST type");

            IReport report = reportFactory.GetReport("EmployeesReport");

            htmlContent = new MarkupString(await report.GetEmbedReportViewer());

            return Page();
        }
    }
}
