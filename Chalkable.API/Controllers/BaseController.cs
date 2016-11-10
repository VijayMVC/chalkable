using System.Web.Mvc;
using Chalkable.API.Models;

namespace Chalkable.API.Controllers
{
    public abstract class BaseController: Controller, IBaseController
    {
        public ChalkableAuthorization ChalkableAuthorization
        {
            get { return Session["ChalkableAuthoriation"] as ChalkableAuthorization; }
            set { Session["ChalkableAuthoriation"] = value; }
        }

        public SchoolPerson CurrentUser
        {
            get { return Session["CurrentUser"] as SchoolPerson; }
            set { Session["CurrentUser"] = value; }
        }
    }
}
