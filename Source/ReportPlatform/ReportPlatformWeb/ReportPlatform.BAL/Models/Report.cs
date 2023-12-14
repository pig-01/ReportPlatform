using ReportService2010;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ReportPlatform.BAL.Models
{
    internal class Report(string name, string description) : IReport
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;

        public void DownloadReport()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetEmbedReportViewer()
        {
            NetworkCredential credential = new("ASUS", "jason001", "LAPTOP-CFJ4JF2E");
            HttpClientHandler handler = new() { Credentials = credential };
            HttpClient client = new(handler)
            {
                BaseAddress = new Uri("http://127.0.0.1:8090/")
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync($"/reportserver?/Report/{Name}&rs:Command=Embed&rs:Format=HTML5");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "";
            }
        }

        public async Task<string> GetEmbedReportViewer(int pageIndex)
        {
            NetworkCredential credential = new("ASUS", "jason001", "LAPTOP-CFJ4JF2E");
            HttpClientHandler handler = new() { Credentials = credential };
            HttpClient client = new(handler)
            {
                BaseAddress = new Uri("http://127.0.0.1:8090/")
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync($"/reportserver?/Report/{Name}&rs:Command=Embed&rc:Section={pageIndex}&rs:Format=HTML5");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "";
            }
        }
    }
}
