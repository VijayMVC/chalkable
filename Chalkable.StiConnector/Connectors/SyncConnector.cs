using System;
using System.Collections.Specialized;
using System.Diagnostics;
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

        public object GetDiff(Type type, long? fromVersion)
        {
            var client = InitWebClient();
            try
            {
                var url = BaseUrl + $"sync/tables/{type.Name}/";
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

                    //var d = reader.ReadToEnd();
                    
                    var resType = (typeof(SyncResult<>)).MakeGenericType(type);
                    var res = serializer.Deserialize(jsonReader, resType);
                    unzipped?.Dispose();
                    return res;

                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse &&
                    (ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                    return null;
                
                string msg = ex.Message;
                var stream = ex.Response?.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    msg = reader.ReadToEnd();
                }
                throw new Exception(msg);
            }
        }
    }
}