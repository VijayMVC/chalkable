using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common.Exceptions
{
    public class NoAnnouncementException : ChalkableException
    {
        public NoAnnouncementException() : base("There is no announcement")
        { 
        }
        public NoAnnouncementException(string message)
            : base(message)
        {
            
        }
    }
}
