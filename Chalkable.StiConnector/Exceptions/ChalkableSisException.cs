﻿using System.Collections.Generic;
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

    public class ChalkableSisNotFoundException : ChalkableSisException
    {
        public ChalkableSisNotFoundException() { }
        public ChalkableSisNotFoundException(string messsage) : base(messsage){}
    }

    public class ChalkableSisNotSupportVersionException : ChalkableSisException
    {
        public ChalkableSisNotSupportVersionException() { }
        public ChalkableSisNotSupportVersionException(string message) : base(message) { }

        public ChalkableSisNotSupportVersionException(string reuiredVersion, string currentVersion)
            : base($"Your inow doesn't support current api. This api required inow version {reuiredVersion} or later")
        {
            
        }
    }
}
