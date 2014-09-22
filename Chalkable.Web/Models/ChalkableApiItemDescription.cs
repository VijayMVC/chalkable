using System.Collections.Generic;

namespace Chalkable.Web.Models
{

    public enum ParamType
    {
        Undefined = 0,
        Integer = 1,
        String = 2,
        Boolean = 3,
        IntList = 4,
        Guid = 5,
        GuidList = 6,
        ListOfStringList = 7,
        Date = 8
    }
    public class Param
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsNullable { get; set; }
        public ParamType ParamType { get; set; }
        
    }
    public class ChalkableApiMethodDescription
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public string Response { get; set; }
        public List<Param> Parameters { get; set; }
        public List<string> AvailableForRoles { get; set; }
    }

    public class ChalkableApiControllerDescription
    {
        public string Name { get; set; }
        public List<ChalkableApiMethodDescription> Methods{ get; set; }
    }
}