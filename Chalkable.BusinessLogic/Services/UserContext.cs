using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

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
        public string SchoolConnectionString {
            get
            {
                return (DistrictId.HasValue && !string.IsNullOrEmpty(DistrictServerUrl))
                    ? Settings.GetSchoolConnectionString(DistrictServerUrl, DistrictId.Value)
                    : null;
            } }
        
        public Guid UserId { get; set; }
        public Guid? DistrictId { get; set; }
        public string Login { get; set; }
        public int RoleId { get; set; }
        [Ignore]
        public CoreRole Role { get; set; }
        public string DistrictServerUrl { get; set; }
        public string DistrictTimeZone { get; set; }

        public int? SchoolLocalId { get; set; }
        public Guid? DeveloperId { get; set; }
        public int? PersonId { get; set; }
        public int? SchoolYearId { get; set; }
        public DateTime? SchoolYearStartDate { get; set; }
        public DateTime? SchoolYearEndDate { get; set; }

        [Ignore]
        public string SisToken { get; set; }
        
        public string SisUrl { get; set; }
        public DateTime? SisTokenExpires { get; set; }

        [Ignore]
        public DateTime NowSchoolTime
        {
            get
            {
                return DateTime.UtcNow.ConvertFromUtc(DistrictTimeZone ?? "UTC");
            }
        }

        public DateTime NowSchoolYearTime
        {
            get
            {
                var res = NowSchoolTime;
                if (SchoolYearEndDate.HasValue && res > SchoolYearEndDate)
                    return SchoolYearEndDate.Value;
                if (SchoolYearStartDate.HasValue && res < SchoolYearStartDate)
                    return SchoolYearStartDate.Value;
                return res;
            }
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
        public IList<ClaimInfo> Claims { get; set; } 

        public UserContext()
        {
            MasterConnectionString = Settings.MasterConnectionString;     
        }

        public UserContext(User user, CoreRole role, District district, Data.Master.Model.School school, Guid? developerId, int? personId, SchoolYear schoolYear = null)
            : this()
        {
            UserId = user.Id;
            Login = user.Login;
            SisTokenExpires = user.LoginInfo.SisTokenExpires;
            SisToken = user.LoginInfo.SisToken;
            PersonId = personId;
            Role = role;
            RoleId = role.Id;
            DeveloperId = developerId;
            if (district != null)
            {
                DistrictId = district.Id;
                DistrictTimeZone = district.TimeZone;
                DistrictServerUrl = district.ServerUrl;
                SisUrl = district.SisUrl; 
                if (school != null)
                {
                    SchoolLocalId = school.LocalId;
                }
                if (schoolYear != null)
                {
                    SchoolYearId = schoolYear.Id;
                    SchoolYearStartDate = schoolYear.StartDate;
                    SchoolYearEndDate = schoolYear.EndDate;
                }
            }
        }

        public void SwitchSchool(Guid districtId, string schoolName, string schoolTimeZoneId, int? schoolLocalId, string districtServerUrl, Guid? developerId)
        {
            if (Role != CoreRoles.SUPER_ADMIN_ROLE && !(Role == CoreRoles.DISTRICT_ROLE && districtId == DistrictId))
                throw new ChalkableSecurityException(ChlkResources.ERR_SWITCH_SCHOOL_INVALID_RIGTHS);
            DistrictServerUrl = districtServerUrl;
            DistrictTimeZone = schoolTimeZoneId;
            SchoolLocalId = schoolLocalId;
            DistrictId = districtId;
            DeveloperId = developerId;
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
            res.Role = CoreRoles.GetById(res.RoleId);
            return res;
        }
    }
}
