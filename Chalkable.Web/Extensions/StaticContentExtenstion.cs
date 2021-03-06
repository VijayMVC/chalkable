﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Extensions
{
    public static class StaticContentExtenstion
    {
        public static string StaticContent(this UrlHelper helper, string url)
        {
#if DEBUG
            return "https:" + CompilerHelper.ScriptsRoot + url;
#else
            return CompilerHelper.ScriptsRoot + url;
#endif
        }
    }
}