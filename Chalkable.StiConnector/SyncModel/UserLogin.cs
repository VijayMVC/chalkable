using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class UserLogin
    {
        public int UserLoginID { get; set; }
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public DateTime LoginDate { get; set; }
        public string IPAddress { get; set; }
        public string DNSHostName { get; set; }
        public bool Success { get; set; }
    }
}
