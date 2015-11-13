using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ApplicationBanHistory
    {
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public Guid ApplicationRef { get; set; }
        public int PersonRef { get; set; }
        public bool Banned { get; set; }
        public DateTime Date { get; set; }
        
        [NotDbFieldAttr]
        public string PersonFirstName { get; set; }
        [NotDbFieldAttr]
        public string PersonLastName { get; set; }
    }
}
