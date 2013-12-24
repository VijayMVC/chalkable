using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services
{
    public class UserContext
    {
        private class Ignore : Attribute
        {
             
        }

        [Ignore]
        public string MasterConnectionString { get; private set; }
        [Ignore]
        public string SchoolConnectionString { get; private set; }
        
        public Guid UserId { get; set; }
        public Guid? SchoolId { get; set; }
        public Guid? DistrictId { get; set; }
        public string Login { get; set; }
        public int RoleId { get; set; }
        [Ignore]
        public CoreRole Role { get; set; }
        public string DistrictServerUrl { get; set; }
        public string SchoolTimeZoneId { get; set; }

        public int? SchoolLocalId { get; set; }
        public Guid? DeveloperId { get; set; }
        public int? UserLocalId { get; set; }
        [Ignore]
        public string SisToken { get; set; }
        
        public string SisUrl { get; set; }
        public DateTime? SisTokenExpires { get; set; }

        [Ignore]
        public DateTime NowSchoolTime
        {
            get { return DateTime.UtcNow.ConvertFromUtc(SchoolTimeZoneId ?? "UTC"); }
        }

        [Ignore]
        public bool IsOAuthUser { get; set; }
        [Ignore]
        public IList<AppPermissionType> AppPermissions { get; set; }
        [Ignore]
        public bool IsInternalApp{ get; set; }
        [Ignore]
        public string OAuthApplication{ get; set; }
        [Ignore]
        public IList<StiConnector.Connectors.Model.Claim> Claims { get; set; } 

        public UserContext()
        {
            MasterConnectionString = Settings.MasterConnectionString;     
        }

        public UserContext(User user, CoreRole role, District district, Data.Master.Model.School school, Guid? developerId) : this()
        {
            UserId = user.Id;
            Login = user.Login;
            UserLocalId = user.LocalId;
            SisTokenExpires = user.SisTokenExpires;
            SisToken = user.SisToken;
            Role = role;
            RoleId = role.Id;
            DeveloperId = developerId;
            if (district != null)
            {
                DistrictId = district.Id;
                SchoolTimeZoneId = district.TimeZone;
                DistrictServerUrl = district.ServerUrl;
                SisUrl = district.SisUrl; //"http://localhost/"; // "http://sandbox.sti-k12.com/Chalkable/"; //
                if (school != null)
                {
                    SchoolLocalId = school.LocalId;
                    SchoolId = school.Id;
                    SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, DistrictServerUrl, DistrictId);
                }
            }
        }

        public void SwitchSchool(Guid? schoolId, Guid districtId, string schoolName, string schoolTimeZoneId, int? schoolLocalId, string districtServerUrl, Guid? developerId)
        {
            if (Role != CoreRoles.SUPER_ADMIN_ROLE && !(Role == CoreRoles.DISTRICT_ROLE && districtId == DistrictId))
                throw new ChalkableSecurityException(ChlkResources.ERR_SWITCH_SCHOOL_INVALID_RIGTHS);
            DistrictServerUrl = districtServerUrl;
            SchoolId = schoolId;
            SchoolTimeZoneId = schoolTimeZoneId;
            SchoolLocalId = schoolLocalId;
            DistrictId = districtId;
            DeveloperId = developerId;
            SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, DistrictServerUrl, districtId);   
        }

        public override string ToString()
        {
            var t = typeof (UserContext);
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var res = new StringBuilder();
            bool first = true;
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.GetCustomAttribute<Ignore>() == null)
                {
                    if (first)
                        first = false;
                    else
                        res.Append("\n");
                    var v = propertyInfo.GetValue(this);
                    if (v != null)
                        res.Append(v);

                }
            return res.ToString();
        }

        public static UserContext FromString(string s)
        {
            var sl = s.Split('\n');

            var t = typeof(UserContext);
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var res = new UserContext();
            int i = 0;
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.GetCustomAttribute<Ignore>() == null)
                {
                    if (!string.IsNullOrEmpty(sl[i]))
                    {
                        object v;
                        if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                            v = int.Parse(sl[i]);
                        else if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
                            v = Guid.Parse(sl[i]);
                        else if (propertyInfo.PropertyType == typeof (DateTime) || propertyInfo.PropertyType == typeof (DateTime?))
                            v = DateTime.Parse(sl[i]);
                        else
                            v = sl[i];
                        propertyInfo.SetValue(res, v);
                    }
                    i++;
                }
            if (res.SchoolId.HasValue)
                res.SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, res.DistrictServerUrl, res.DistrictId);
            res.Role = CoreRoles.GetById(res.RoleId);
            return res;
        }
    }
}
