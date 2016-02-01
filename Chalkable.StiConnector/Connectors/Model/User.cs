using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Claim
    {
        public string Type { get; set; }
        public IEnumerable<string> Values { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int? PersonId { get; set; }
        public int? StaffId { get; set; }
        public int? StudentId { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}
