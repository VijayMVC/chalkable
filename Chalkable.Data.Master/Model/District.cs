using System;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Data.Master.Model
{
    public class District
    {
        public const string ID_FIELD = "Id";
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DbName { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        public int Status { get; set; }
        public string TimeZone { get; set; }
        public const string DEMO_PREFIX_FIELD = "DemoPrefix";
        public string DemoPrefix { get; set; }
        public const string LAST_USED_DEMO_FIELD = "LastUseDemo";
        public DateTime? LastUseDemo { get; set; }
        public const string SERVER_URL_FIELD = "ServerUrl";
        public string ServerUrl { get; set; }
    }

    public enum DistrictStatus
    {
        Created = 0,
        DataImported = 1,
        InvitedUser = 2,
        TeacherLogged = 3,
        InvitedStudent = 4,
        StudentLogged = 5,
        PayingCustomer = 6
    }
}