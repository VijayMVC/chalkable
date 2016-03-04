using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Exceptions;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public class ConnectorBase
    {
        private const string ERROR_FORMAT = "Error calling : '{0}' ;\n  Message: {1}";
        private const string REQUEST_TIME_MSG_FORMAT = "Request on : '{0}' \n Time : {1}";
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
            return res != null ? res.Resources.FirstOrDefault() : default(TModel);
        }
        
        public async Task<TResponse> CallAsync<TResponse>(string relativeUrl, NameValueCollection requestParams)
            where TResponse : BaseResponse
        {
            var startTime = DateTime.Now;
            var url = ConnectorLocator.ApiRoot + relativeUrl;
            var client = InitWebClient();
            PrepareRequestParams(client, requestParams);
            MemoryStream stream = null;
            StreamReader reader = null;
            try
            {
                var dataTask = client.DownloadDataTaskAsync(url);
                stream = new MemoryStream(await dataTask);
                reader = new StreamReader(stream);
                var response = JsonConvert.DeserializeObject<TResponse>(reader.ReadToEnd());

                var time = DateTime.Now - startTime;
                var timeString = $"{time.Minutes}:{time.Seconds}.{time.Milliseconds}";
                Trace.TraceInformation(REQUEST_TIME_MSG_FORMAT, url, timeString);

                ProcessResponseStatus(response.Status, url);
                return response;
            }
            catch (WebException ex)
            {
                return HandleWebExceprion<TResponse>(ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream?.Dispose();
                reader?.Dispose();
            }
        }

        private void ProcessResponseStatus(Status status, string url)
        { 
            if (status.Code != (int) HttpStatusCode.OK)
            {
                Trace.TraceError(string.Format(ERROR_FORMAT, url, $"{status.Category}. {status.InformationMessage}"));
                if (status.Code == (int)HttpStatusCode.Unauthorized)
                    throw new ChalkableABUnauthorizedException(status.ErrorMessage, status.InformationMessage);
            }
        }
        private T HandleWebExceprion<T>(WebException ex)
        {
            var reader = new StreamReader(ex.Response.GetResponseStream());
            var msg = reader.ReadToEnd();
            Trace.TraceError(string.Format(ERROR_FORMAT, ex.Response.ResponseUri, msg));
            var response = ex.Response as HttpWebResponse;
            if (response == null)
                throw new ChalkableException(msg);

            var status = response.StatusCode;
            throw new HttpException((int)status, ex.Message + Environment.NewLine + msg);
        }
    }
}
