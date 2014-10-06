using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common.Exceptions
{
    public class NoClassAnnouncementTypeException : ChalkableException
    {
        public NoClassAnnouncementTypeException() : base("There is no ClassAnnouncementTypes")
        { 
        }
        public NoClassAnnouncementTypeException(string message)
            : base(message)
        {
            
        }
    }
}
