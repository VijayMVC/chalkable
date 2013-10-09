using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public class AddressSecurity
    {
        public static bool CanModify(Address address, UserContext context)
        {
            return BaseSecurity.IsAdminEditor(context) || address.PersonRef == context.UserId;
        }
    }
}
