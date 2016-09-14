using System;

namespace Chalkable.StiConnector.Attributes
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
