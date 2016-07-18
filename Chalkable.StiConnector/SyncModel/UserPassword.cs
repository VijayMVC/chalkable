using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class UserPassword
    {
        public int UserPasswordID { get; set; }
        public int UserID { get; set; }
        public string Password { get; set; }
        public bool ChangeOnLogin { get; set; }
        public bool UserCannotChange { get; set; }
        public bool NeverExpires { get; set; }
        public DateTime IssueDate { get; set; }
        public bool IsEncrypted { get; set; }
        public bool HasSalt { get; set; }
        public bool IsArchived { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
