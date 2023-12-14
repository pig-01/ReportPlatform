using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPlatform.BAL.Models
{
    internal class EmployeesReport(Report report) : IReport
    {
        private readonly Report report = report;

        public string Name => this.report.Name;

        public string Description => this.report.Description;

        public void DownloadReport() => this.report.DownloadReport();

        public Task<string> GetEmbedReportViewer() => this.report.GetEmbedReportViewer();

        public Task<string> GetEmbedReportViewer(int pageIndex) => this.report.GetEmbedReportViewer(pageIndex);
    }
}
