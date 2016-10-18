using Newtonsoft.Json.Serialization;

namespace Chalkable.Common.JsonContractTools
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
