using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.StiConnector.Exceptions
{
    public class ChalkableSisException : ChalkableException
    {
        public ChalkableSisException()
            : base("Sis request exception")
        {
            SisMessages = new List<string>();
        }
        public ChalkableSisException(string message) : base(message)
        {
            SisMessages = new List<string> {message};
        }

        public ChalkableSisException(IList<string> messages) : base(messages.JoinString())
        {
            SisMessages = messages;
        }

        public IList<string> SisMessages { get; private set; } 
    }
}
