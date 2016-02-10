using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Chalkable.API
{
    public interface IConnector
    {
        Task<T> Call<T>(string endpoint, OnWebRequestIsCreated onCreated = null, string method = null);
        Task<T> Call<T>(string endpoint, Stream stream, string method = null, string contentType = null);
        Task<T> Post<T>(string endpoint, NameValueCollection postData);
    }
}