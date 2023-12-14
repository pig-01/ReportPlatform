using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReportPlatform.BAL.Factories;
using ReportPlatform.BAL.Models;

namespace ReportPlatform.Web.Pages
{
    public class OrderReportModel(ILogger<OrderReportModel> logger, IReportFactory reportFactory) : PageModel
    {
        private readonly ILogger<OrderReportModel> logger = logger;
        private readonly IReportFactory reportFactory = reportFactory;

        public MarkupString htmlContent;



        public async Task OnGet(int? pages)
        {
            logger.LogInformation("Get Order Report with GET type");

            IReport report = reportFactory.GetReport("OrderReport");

            htmlContent = new MarkupString(await report.GetEmbedReportViewer(pages ?? 1));
        }

        public async Task<IActionResult> OnPost(int? pages)
        {
            logger.LogInformation("Get Order Report with POST type");

            IReport report = reportFactory.GetReport("OrderReport");

            htmlContent = new MarkupString(await report.GetEmbedReportViewer(pages ?? 1));

            return Page();
        }
    }
}
