using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Chalkable.StiConnector.Connectors
{
    public class ReportConnector : ConnectorBase
    {
        public ReportConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public byte[] Download<T>(string url, T obj, HttpMethod httpMethod = null)
        {

            httpMethod = httpMethod ?? HttpMethod.Post;
            var client = InitWebClient();
            
            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            var stream = new MemoryStream();
            try
            {
                var serializer = new JsonSerializer();
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, obj);
                writer.Flush();
                var data = client.UploadData(url, httpMethod.Method, stream.ToArray());
                return data;
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
            finally
            {
                stream.Dispose();
            }
        }

        public class ProgressReportParams
        {
            public int AcadSessionId { get; set; }
            public int GradingPeriodId { get; set; }
            public int IdToPrint { get; set; }
            public int SectionId { get; set; }
            public int[] StudentIds { get; set; }
        }

        public byte[] ProgressReport(ProgressReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/progress");
            var res = Download(url, ps);
            return res;
        }
    }
}