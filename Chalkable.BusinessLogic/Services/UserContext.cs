using System;
using System.Collections.Generic;
using System.Globalization;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;

namespace Chalkable.BusinessLogic.Services
{
    public class UserContext
    {
        public string MasterConnectionString { get; private set; }
        public string SchoolConnectionString { get; private set; }
        
        public Guid UserId { get; private set; }
        public Guid? SchoolId { get; private set; }
        public bool IsDeveloperSchool { get; private set; }
        public string Login { get; private set; }
        public string SchoolName { get; private set; }
        public CoreRole Role { get; private set; }
        public string SchoolServerUrl { get; private set; }
        public string SchoolTimeZoneId { get; private set; }
        public DateTime NowSchoolTime
        {
            get { return DateTime.UtcNow.ConvertFromUtc(SchoolTimeZoneId ?? "UTC"); }
        }

        public bool IsOAuthUser { get; set; }
        public IList<AppPermissionType> AppPermissions { get; set; }

        public UserContext(Guid id, Guid? schoolId, string login, string schoolName, string schoolTimeZoneId, string schoolServerUrl, CoreRole role)
        {
            SchoolServerUrl = schoolServerUrl;
            SchoolName = schoolName;
            UserId = id;
            SchoolId = schoolId;
            Login = login;
            Role = role;

            SchoolTimeZoneId = schoolTimeZoneId;

            //TODO: IsDeveloperSchool

            if (schoolId.HasValue)
                SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, SchoolServerUrl, schoolId);
            MasterConnectionString = Settings.MasterConnectionString;
        }

        public void SwitchSchool(Guid schoolId, string schoolName, string schoolTimeZoneId, string schoolServerUrl)
        {
            if (Role != CoreRoles.SUPER_ADMIN_ROLE)
                throw new ChalkableSecurityException("Only sys admin is able to switch between schools");
            SchoolServerUrl = schoolServerUrl;
            SchoolName = schoolName;
            SchoolId = schoolId;
            SchoolTimeZoneId = schoolTimeZoneId;
            SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, SchoolServerUrl, schoolId);   
        }

        private const char DELIMITER = '\n';
        public override string ToString()
        {
            var parameters = new []
                {
                    UserId.ToString(),
                    SchoolId.HasValue ? SchoolId.ToString() : string.Empty,
                    Login,
                    SchoolName ?? string.Empty,
                    Role.Id.ToString(CultureInfo.InvariantCulture),
                    SchoolServerUrl ?? string.Empty,
                    SchoolTimeZoneId ?? string.Empty
                };
            return parameters.JoinString(DELIMITER.ToString(CultureInfo.InvariantCulture));
        }

        public static UserContext FromString(string s)
        {
            var sl = s.Split(DELIMITER);
            var userId = Guid.Parse(sl[0]);
            var schoolId = string.IsNullOrEmpty(sl[1]) ? (Guid?)null : Guid.Parse(sl[1]);
            string schoolName = null;
            string schoolTimeZone = null;
            string schoolServerUrl = null;
            if (schoolId.HasValue)
            {
                schoolName = sl[3]; 
                schoolServerUrl = sl[5];
                schoolTimeZone = sl[6];
            }
            var role = CoreRoles.GetById(int.Parse(sl[4]));
            var res = new UserContext(userId, schoolId, sl[2], schoolName, schoolTimeZone, schoolServerUrl, role);
            return res;
        }
    }
}
