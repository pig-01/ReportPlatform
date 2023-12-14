using ReportPlatform.BAL.Models;

namespace ReportPlatform.BAL.Factories
{
    public class ReportFactory : IReportFactory
    {
        public IReport GetReport(string reportName)
        {
            string description = $"{reportName} report description from factory!";

            return new Report(reportName, description);
        }

        public IEnumerable<IReport> GetReports(params string[] reportNames)
        {
            foreach (string reportName in reportNames)
            {
                yield return this.GetReport(reportName);
            }
        }
    }
}
