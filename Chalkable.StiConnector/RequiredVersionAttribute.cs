using System;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Exceptions;
using PostSharp.Aspects;

namespace Chalkable.StiConnector
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredVersionAttribute : MethodInterceptionAspect
    {
        public string RequiredVersion { get; }
        public RequiredVersionAttribute(string requiredVersion)
        {
            VersionHelper.ValidateVersionFormat(requiredVersion);
            RequiredVersion = requiredVersion;
        }
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var connector = args.Instance as ConnectorBase; //Only for connectors
            if (connector != null && VersionHelper.CompareVersionTo(RequiredVersion, connector.ApiVersion) == 1)
                    throw new ChalkableSisNotSupportVersionException(RequiredVersion, connector.ApiVersion);
            
            args.Proceed();
        }
        
    }
}
