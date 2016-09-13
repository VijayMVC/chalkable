using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SisMinVersionAttribute : Attribute
    {
        public SisMinVersionAttribute(string minVersion)
        {
            MinVersion = minVersion;
        }

        public string MinVersion { get; }
    }
}
