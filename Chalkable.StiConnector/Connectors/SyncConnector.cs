using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
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
            MemoryStream stream = null;
            try
            {
                var url = BaseUrl + string.Format("sync/tables/{0}/", type.Name);
                if (fromVersion.HasValue)
                    url = url + fromVersion;
                client.QueryString = new NameValueCollection();
                var data = client.DownloadData(url);
                var res = Encoding.UTF8.GetString(data);
                //Debug.WriteLine(res);
                stream = new MemoryStream(data);
                var serializer = new JsonSerializer();
                var reader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(reader);


                var resType = (typeof (SyncResult<>)).MakeGenericType(new []{type});
                return serializer.Deserialize(jsonReader, resType);
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
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}