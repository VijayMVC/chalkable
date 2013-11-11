using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Guid? DistrictId { get; private set; }
        public string Login { get; private set; }
        public CoreRole Role { get; private set; }
        public string DistrictServerUrl { get; private set; }
        public string SchoolTimeZoneId { get; private set; }

        public int? SchoolLocalId { get; set; }
        public Guid? DeveloperId { get; private set; }
        public int? UserLocalId { get; set; }
        public string SisToken { get; set; }
        public string SisUrl { get; set; }

        public DateTime NowSchoolTime
        {
            get { return DateTime.UtcNow.ConvertFromUtc(SchoolTimeZoneId ?? "UTC"); }
        }

        public bool IsOAuthUser { get; set; }
        public IList<AppPermissionType> AppPermissions { get; set; }

        public bool IsInternalApp{ get; set; }
        public string OAuthApplication{ get; set; }

        public UserContext(Guid id, Guid? districtId, Guid? schoolId, string login, string schoolTimeZoneId, 
            string schoolServerUrl, int? schoolLocalId, CoreRole role, Guid? developerId, int? localId, string sisToken, string sisUrl)
        {
            DistrictServerUrl = schoolServerUrl;
            UserId = id;
            SchoolId = schoolId;
            Login = login;
            Role = role;
            DistrictId = districtId;

            SchoolTimeZoneId = schoolTimeZoneId;
            SchoolLocalId = schoolLocalId;
            DeveloperId = developerId;
            UserLocalId = localId;
            SisToken = sisToken;
            SisUrl = sisUrl;

            if (schoolId.HasValue)
                SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, DistrictServerUrl, districtId);
            MasterConnectionString = Settings.MasterConnectionString;
        }

        public void SwitchSchool(Guid? schoolId, Guid districtId, string schoolName, string schoolTimeZoneId, int? schoolLocalId, string districtServerUrl, Guid? developerId)
        {
            if (Role != CoreRoles.SUPER_ADMIN_ROLE && !(Role == CoreRoles.DISTRICT_ROLE && districtId == DistrictId))
                throw new ChalkableSecurityException("Only sys admin or district is able to switch between schools");
            DistrictServerUrl = districtServerUrl;
            SchoolId = schoolId;
            SchoolTimeZoneId = schoolTimeZoneId;
            SchoolLocalId = schoolLocalId;
            DistrictId = districtId;
            DeveloperId = developerId;
            SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, DistrictServerUrl, districtId);   
        }

        private const char DELIMITER = '\n';
        public override string ToString()
        {
            var parameters = new []
                {
                    UserId.ToString(),
                    SchoolId.HasValue ? SchoolId.ToString() : string.Empty,
                    Login,
                    Role.Id.ToString(CultureInfo.InvariantCulture),
                    DistrictServerUrl ?? string.Empty,
                    SchoolTimeZoneId ?? string.Empty,
                    SchoolLocalId.HasValue ? SchoolLocalId.ToString() : string.Empty,
                    DistrictId.HasValue ? DistrictId.ToString() : string.Empty,
                    DeveloperId.HasValue ? DeveloperId.ToString() : string.Empty,
                    UserLocalId.HasValue ? UserLocalId.ToString() : string.Empty,
                    SisToken ?? "",
                    SisUrl ?? ""
                };
            return parameters.JoinString(DELIMITER.ToString(CultureInfo.InvariantCulture));
        }


        private const int USER_ID = 0;
        private const int SCHOOL_ID = 1;
        private const int LOGIN = 2;
        private const int ROLE_ID = 3;
        private const int SCHOOL_SERVER_URL = 4;
        private const int SCHOOL_TIMEZONE_ID = 5;
        private const int SHOOL_LOCAL_ID = 6;
        private const int DISTRICT_ID = 7;
        private const int DEVELOPER_ID = 8;
        private const int USER_LOCAL_ID = 9;
        private const int SIS_TOKEN = 10;
        private const int SIS_URL = 11;

        public static UserContext FromString(string s)
        {
            var sl = s.Split(DELIMITER);
            var userId = Guid.Parse(sl[USER_ID]);
            var schoolId = string.IsNullOrEmpty(sl[SCHOOL_ID]) ? (Guid?)null : Guid.Parse(sl[SCHOOL_ID]);
            var districtId = string.IsNullOrEmpty(sl[DISTRICT_ID]) ? (Guid?)null : Guid.Parse(sl[DISTRICT_ID]);
            var login = sl[LOGIN];

            string schoolTimeZone = null;
            string schoolServerUrl = null;
            Guid? developerId = null;
            int? localId = null;
            int? schoolLocalId = null;
            string sisToken = null;
            string sisUrl = null;
            if (schoolId.HasValue)
            {
                schoolServerUrl = sl[SCHOOL_SERVER_URL];
                schoolTimeZone = sl[SCHOOL_TIMEZONE_ID];
                schoolLocalId = string.IsNullOrEmpty(sl[SHOOL_LOCAL_ID]) ? (int?)null : int.Parse(sl[SHOOL_LOCAL_ID]);

                if (!string.IsNullOrEmpty(sl[DEVELOPER_ID]))
                    developerId = Guid.Parse(sl[DEVELOPER_ID]);
                if (!string.IsNullOrEmpty(sl[USER_LOCAL_ID]))
                    localId = int.Parse(sl[USER_LOCAL_ID]);
                sisToken = sl[SIS_TOKEN];
                sisUrl = sl[SIS_URL];
            }
            var role = CoreRoles.GetById(int.Parse(sl[ROLE_ID]));
            
            var res = new UserContext(userId, districtId, schoolId, login, schoolTimeZone, schoolServerUrl, schoolLocalId, role, developerId, localId, sisToken, sisUrl);
            return res;
        }
    }
}
