using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Model
{
    public class EmailInfo
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public EmailInfo()
        {

        }

        public EmailInfo(string s)
        {
            var sl = s.Split('|');
            if (sl.Length != 3)
                throw new ChalkableException(ChlkResources.ERR_INVALID_EMAIL_INFO_STRING);
            Email = sl[0];
            UserName = sl[1];
            Password = sl[2];
        }

        public override string ToString()
        {
            return Email + "|" + UserName + "|" + Password;
        }
    }
}
