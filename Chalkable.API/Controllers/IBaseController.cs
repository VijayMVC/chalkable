using Chalkable.API.Models;

namespace Chalkable.API.Controllers
{
    public interface IBaseController
    {
        ChalkableAuthorization ChalkableAuthorization { get; }
        SchoolPerson CurrentUser { get; }
    }
}
