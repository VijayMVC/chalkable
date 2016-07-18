using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;

namespace Chalkable.Web.Tools
{
    static public class PasswordTools
    {
        public static bool IsSecurePassword(string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                return false;

            if (newPassword.Length < Settings.MinPasswordLength)
                return false;

            return true;
        }
    }
}