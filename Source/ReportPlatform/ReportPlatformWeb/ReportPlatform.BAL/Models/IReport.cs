namespace ReportPlatform.BAL.Models
{
    public interface IReport
    {
        public string Name { get; }
        public string Description { get; }

        public abstract void DownloadReport();
        public abstract Task<string> GetEmbedReportViewer();
        public abstract Task<string> GetEmbedReportViewer(int pageIndex);
    }
}
