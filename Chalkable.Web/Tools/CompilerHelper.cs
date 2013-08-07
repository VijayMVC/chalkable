﻿namespace Chalkable.Web.Tools
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
                var res = "https://az374501.vo.msecnd.net/static-" + Version;
                return res;
            }
        }
        
        private const string VERSION = "private-build";
        public static string Version
        {
            get
            {
                return VERSION.Replace('.', '-');    
            }
        }

    }
}