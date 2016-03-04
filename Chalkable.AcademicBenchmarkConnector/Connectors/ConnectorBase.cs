using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public class ConnectorBase
    {
        protected IConnectorLocator ConnectorLocator { get; }
        public ConnectorBase(IConnectorLocator connectorLocator)
        {
            ConnectorLocator = connectorLocator;
        }



        protected WebClient InitWebClient()
        {
            var client = new WebClient
            {
                Encoding = Encoding.UTF8,
                QueryString = new NameValueCollection
                {
                    ["partner.id"] = Settings.AcademicBenchmarkPartnerId,
                    ["auth.signature"] = ConnectorLocator.AuthSignarute,
                    ["auth.expires"] = ConnectorLocator.AuthExpires.ToString()
                }
            };
            //client.Headers.Add("Content-Type", "application/json");
            return client;
        }

        private void PrepareRequestParams(WebClient client, NameValueCollection nvc)
        {
            if(nvc != null)
                foreach (var key in nvc.AllKeys)
                {
                    if (client.QueryString.AllKeys.All(x => x != key))
                        client.QueryString.Add(key, nvc[key]);
                }
        }

        public async Task<TModel> GetOne<TModel>(string relativeUrl, NameValueCollection requestParams)
        {
            var res = await CallAsync<PaginatedResponse<TModel>>(relativeUrl, requestParams);
            return res.Resources.FirstOrDefault();
        }
        
        public async Task<TResponse> CallAsync<TResponse>(string relativeUrl, NameValueCollection requestParams)
            where TResponse : BaseResponse
        {
            var url = ConnectorLocator.ApiRoot + relativeUrl;
            var client = InitWebClient();
            PrepareRequestParams(client, requestParams);
            Debug.WriteLine("Request on: " + url);
            MemoryStream stream = null;
            StreamReader reader = null;
            try
            {
                var dataTask = client.DownloadDataTaskAsync(url);
                stream = new MemoryStream(await dataTask);
                reader = new StreamReader(stream);
                var response = JsonConvert.DeserializeObject<TResponse>(reader.ReadToEnd());
                ProcessResponse(response);
                return response;
            }
            catch (WebException ex)
            {
                return HandleWebExceprion<TResponse>(ex);
            }
            finally
            {
                stream?.Dispose();
                reader?.Dispose();
            }
        }

        private void ProcessResponse(BaseResponse response)
        {
            //TODO imp this later;
        }

        private T HandleWebExceprion<T>(WebException exception)
        {
            //TODO impl this later;
            throw exception;
        }
    }
}
