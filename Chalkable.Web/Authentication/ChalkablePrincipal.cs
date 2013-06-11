﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Authentication
{
    public class ChalkablePrincipal : IPrincipal
    {
        public UserContext Context { get; private set; }
        public ChalkablePrincipal(UserContext context)
        {
            Identity = new ChalkableIdentity(context);
            Context = context;
        }

        public bool IsInRole(string role)
        {
            return Context.Role.LoweredName == role.ToLower();
        }

        public IIdentity Identity { get; private set; }
    }
}