using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Authentication
{
    public class ChalkableIdentity : IIdentity
    {
        public string Name { get; private set; }
        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public ChalkableIdentity(UserContext context)
        {
            Name = context.Login;
            AuthenticationType = "Forms";
            IsAuthenticated = true;
        }
    }
}