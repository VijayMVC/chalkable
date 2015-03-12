using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using Chalkable.StiConnector.SyncModel;
using Newtonsoft.Json;

namespace Chalkable.StiConnector.Connectors
{
    public class SyncConnector : ConnectorBase
    {
        public SyncConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public object GetDiff(Type type, int? fromVersion)
        {
            var client = InitWebClient();
            try
            {
                var url = BaseUrl + string.Format("sync/tables/{0}/", type.Name);
                if (fromVersion.HasValue)
                    url = url + fromVersion;

                client.QueryString = new NameValueCollection();
                var data = client.DownloadData(url);
                
                using (var ms = new MemoryStream(data))
                {
                    StreamReader reader;
                    GZipStream unzipped = null;
                    if (client.ResponseHeaders[HttpResponseHeader.ContentType].ToLower() == "application/octet-stream")
                    {
                        unzipped = new GZipStream(ms, CompressionMode.Decompress);
                        reader = new StreamReader(unzipped);
                    }
                    else
                        reader = new StreamReader(ms);
                    var serializer = new JsonSerializer();
                    var jsonReader = new JsonTextReader(reader);
                    var resType = (typeof(SyncResult<>)).MakeGenericType(new[] { type });
                    var res = serializer.Deserialize(jsonReader, resType);
                    if (unzipped != null)
                        unzipped.Dispose();
                    return res;

                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse &&
                    (ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                    return null;
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
        }
    }
}