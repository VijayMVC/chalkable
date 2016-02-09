using System.Net;
using System.Threading.Tasks;

namespace Chalkable.API
{
    public interface IConnector
    {
        Task<T> Call<T>(string endpoint, OnWebRequestIsCreated onCreated = null, string method = null);
    }
}