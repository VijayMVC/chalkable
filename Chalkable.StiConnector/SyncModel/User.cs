using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class User
    {
        public int UserID { get; set; }
        public int TrusteeID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string LoginMethod { get; set; }
        public Guid UserGUID { get; set; }
        public bool LockedOut { get; set; }
        public bool IsDisabled { get; set; }
        public byte AuthenticationAttempts { get; set; }
        public DateTime? LastAuthenticationAttempt { get; set; }
        public bool UseADAuthentication { get; set; }
        public bool IsSystem { get; set; }
        public Guid DistrictGuid { get; set; }
        public int? InFocusUserId { get; set; }
    }
}
