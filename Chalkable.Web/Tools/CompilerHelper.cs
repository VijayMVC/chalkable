using System;
using System.Configuration;
using Chalkable.Common;

namespace Chalkable.Web.Tools
{
    public class CompilerHelper
    {
        public static bool IsDebug
        {
            get
            {
                var res = false;
#if DEBUG
                res = true;
#endif
                return res;
            }
        }

        public static string ScriptsRoot
        {
            get
            {
                return string.Format(Settings.ScriptsRoot, Version);
            }
        }
        
        private const string VERSION = "private-build";
        public static string Version
        {
            get
            {
                return VERSION;    
            }
        }

        public static string ScriptsRootDomain
        {
            get
            {
                try
                {
                    return new Uri(ScriptsRoot).DnsSafeHost ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static bool IsProduction => Version != "private" + "-" + "build"; // ensure no TC versioning
    }
}