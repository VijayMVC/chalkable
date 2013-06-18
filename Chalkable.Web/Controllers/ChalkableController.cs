using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using Chalkable.Web.ActionResults;

namespace Chalkable.Web.Controllers
{
    public class ChalkableController : Controller
    {
        public new ActionResult Json(object data, int serializationDepth = 10)
        {
            return new ChalkableJsonResult(false){Data = data, SerializationDepth = serializationDepth};
        }
    }
}
