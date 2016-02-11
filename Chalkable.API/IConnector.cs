using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Chalkable.API
{
    public interface IConnector
    {
        Task<T> Get<T>(string endpoint); 
        Task<T> Put<T>(string endpoint, Stream stream);
        Task<T> Post<T>(string endpoint, NameValueCollection postData);
    }
}