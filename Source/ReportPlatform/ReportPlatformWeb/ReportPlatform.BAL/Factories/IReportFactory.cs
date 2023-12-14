using ReportPlatform.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPlatform.BAL.Factories
{
    public interface IReportFactory
    {
        public abstract IReport GetReport(string reportName);
        public abstract IEnumerable<IReport> GetReports(params string[] reportNames);

    }
}
