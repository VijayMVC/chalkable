using System;
using System.Collections.Generic;
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
                    ["auth.signature"] = ConnectorLocator.AuthContext.AuthSignarute,
                    ["auth.expires"] = ConnectorLocator.AuthContext.AuthExpires.ToString()
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

        protected async Task<TModel> GetOne<TModel>(string relativeUrl, NameValueCollection requestParams)
        {
            var res = await GetPage<TModel>(relativeUrl, requestParams, 0, 1);
            return res.FirstOrDefault();
        }
        protected async Task<IList<TModel>> GetList<TModel>(string relativeUrl, NameValueCollection requestParams)
        {
            return await GetPage<TModel>(relativeUrl, requestParams);
        }
        protected async Task<PaginatedList<TModel>> GetPage<TModel>(string relativeUrl, NameValueCollection requestParams, int offset = 0, int limit = int.MaxValue)
        {
            requestParams = requestParams ?? new NameValueCollection();
            requestParams.Add("offset", offset.ToString());
            requestParams.Add("limit", limit.ToString());

            var res = await CallAsync<PaginatedResponse<TModel>>(relativeUrl, requestParams);

            return res != null
                ? new PaginatedList<TModel>(res.Resources, res.Offset/(res.Limit == 0 ? 1 : res.Limit), (res.Limit == 0 ? 1 : res.Limit), res.Count)
                : new PaginatedList<TModel>(new List<TModel>(), offset/limit, limit);

        }

        protected async Task<TResponse> CallAsync<TResponse>(string relativeUrl, NameValueCollection requestParams)
            where TResponse : BaseResponse
        {
            var startTime = DateTime.Now;
            var url = $"{ConnectorLocator.ApiRoot}/{relativeUrl}";
            var client = InitWebClient();
            PrepareRequestParams(client, requestParams);
            MemoryStream stream = null;
            StreamReader reader = null;
            try
            {
                var dataTask = client.DownloadDataTaskAsync(url);
                stream = new MemoryStream(await dataTask);
                reader = new StreamReader(stream);
                var str = reader.ReadToEnd();
                var response = JsonConvert.DeserializeObject<TResponse>(str);

                var time = DateTime.Now - startTime;
                var timeString = $"{time.Minutes}:{time.Seconds}.{time.Milliseconds}";
                //Trace.TraceInformation(REQUEST_TIME_MSG_FORMAT, url, timeString);

                ProcessResponseStatus(response.Status, url);
                return response;
            }
            catch (WebException ex)
            {
                return HandleWebException<TResponse>(ex);
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
                //Trace.TraceError(string.Format(ERROR_FORMAT, url, $"{status.Category}. {status.InformationMessage}"));
                if (status.Code == (int)HttpStatusCode.Unauthorized)
                    throw new ChalkableABUnauthorizedException(status.ErrorMessage, status.InformationMessage);
            }
        }
        private T HandleWebException<T>(WebException ex)
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
