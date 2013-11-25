using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services
{
    public class UserContext
    {
        [JsonIgnore]
        public string MasterConnectionString { get; private set; }
        [JsonIgnore]
        public string SchoolConnectionString { get; private set; }
        
        public Guid UserId { get; set; }
        public Guid? SchoolId { get; set; }
        public Guid? DistrictId { get; set; }
        public string Login { get; set; }
        public int RoleId { get; set; }
        [JsonIgnore]
        public CoreRole Role { get; set; }
        public string DistrictServerUrl { get; set; }
        public string SchoolTimeZoneId { get; set; }

        public int? SchoolLocalId { get; set; }
        public Guid? DeveloperId { get; set; }
        public int? UserLocalId { get; set; }
        [JsonIgnore]
        public string SisToken { get; set; }
        
        public string SisUrl { get; set; }
        public DateTime? SisTokenExpires { get; set; }

        [JsonIgnore]
        public DateTime NowSchoolTime
        {
            get { return DateTime.UtcNow.ConvertFromUtc(SchoolTimeZoneId ?? "UTC"); }
        }
        
        [JsonIgnore]
        public bool IsOAuthUser { get; set; }
        [JsonIgnore]
        public IList<AppPermissionType> AppPermissions { get; set; }
        [JsonIgnore]
        public bool IsInternalApp{ get; set; }
        [JsonIgnore]
        public string OAuthApplication{ get; set; }

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
                SisUrl = district.SisUrl;
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
            var serialiser = new JsonSerializer();
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                serialiser.Serialize(writer, this);
                writer.Flush();
                var res = Encoding.UTF8.GetString(stream.ToArray());
                return res;
            }
        }
        public static UserContext FromString(string s)
        {
            var serialiser = new JsonSerializer();
            var data = Encoding.UTF8.GetBytes(s);
            using (var stream = new MemoryStream(data))
            {
                var reader = new StreamReader(stream);
                var res =  serialiser.Deserialize<UserContext>(new JsonTextReader(reader));
                res.Role = CoreRoles.GetById(res.RoleId);

                if (res.SchoolId.HasValue)
                    res.SchoolConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, res.DistrictServerUrl, res.DistrictId);
                return res;
            }
        }
    }
}
