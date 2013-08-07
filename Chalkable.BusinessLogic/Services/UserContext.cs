﻿using System;
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
        public string Login { get; private set; }
        public string SchoolName { get; private set; }
        public CoreRole Role { get; private set; }
        public string SchoolServerUrl { get; private set; }
        public string SchoolTimeZoneId { get; private set; }

        public Guid? DeveloperId { get; private set; }

        public DateTime NowSchoolTime
        {
            get { return DateTime.UtcNow.ConvertFromUtc(SchoolTimeZoneId ?? "UTC"); }
        }

        public bool IsOAuthUser { get; set; }
        public IList<AppPermissionType> AppPermissions { get; set; }

        public UserContext(Guid id, Guid? schoolId, string login, string schoolName, string schoolTimeZoneId, string schoolServerUrl, CoreRole role, Guid? developerId)
        {
            SchoolServerUrl = schoolServerUrl;
            SchoolName = schoolName;
            UserId = id;
            SchoolId = schoolId;
            Login = login;
            Role = role;

            SchoolTimeZoneId = schoolTimeZoneId;
            DeveloperId = developerId;
            
            if (schoolId.HasValue)
                SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, SchoolServerUrl, schoolId);
            MasterConnectionString = Settings.MasterConnectionString;
        }

        public void SwitchSchool(Guid schoolId, string schoolName, string schoolTimeZoneId, string schoolServerUrl, Guid? developerId)
        {
            if (Role != CoreRoles.SUPER_ADMIN_ROLE)
                throw new ChalkableSecurityException("Only sys admin is able to switch between schools");
            SchoolServerUrl = schoolServerUrl;
            SchoolName = schoolName;
            SchoolId = schoolId;
            SchoolTimeZoneId = schoolTimeZoneId;
            DeveloperId = developerId;
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
                    SchoolTimeZoneId ?? string.Empty,
                    DeveloperId.HasValue ? DeveloperId.ToString() : string.Empty
                };
            return parameters.JoinString(DELIMITER.ToString(CultureInfo.InvariantCulture));
        }


        private const int USER_ID = 0;
        private const int SCHOOL_ID = 1;
        private const int LOGIN = 2;
        private const int SCHOOL_NAME = 3;
        private const int ROLE_ID = 4;
        private const int SCHOOL_SERVER_URL = 5;
        private const int SCHOOL_TIMEZONE_ID = 6;
        private const int DEVELOPER_ID = 7;



        public static UserContext FromString(string s)
        {
            var sl = s.Split(DELIMITER);
            var userId = Guid.Parse(sl[USER_ID]);
            var schoolId = string.IsNullOrEmpty(sl[SCHOOL_ID]) ? (Guid?)null : Guid.Parse(sl[SCHOOL_ID]);
            var login = sl[LOGIN];

            string schoolName = null;
            string schoolTimeZone = null;
            string schoolServerUrl = null;
            Guid? developerId = (Guid?)null;

            if (schoolId.HasValue)
            {
                schoolName = sl[SCHOOL_NAME];
                schoolServerUrl = sl[SCHOOL_SERVER_URL];
                schoolTimeZone = sl[SCHOOL_TIMEZONE_ID];

                if (sl.Length > 7 && !string.IsNullOrEmpty(sl[DEVELOPER_ID]))
                    developerId = Guid.Parse(sl[DEVELOPER_ID]);
            }
            var role = CoreRoles.GetById(int.Parse(sl[ROLE_ID]));
            var res = new UserContext(userId, schoolId, login, schoolName, schoolTimeZone, schoolServerUrl, role, developerId);
            return res;
        }
    }
}
