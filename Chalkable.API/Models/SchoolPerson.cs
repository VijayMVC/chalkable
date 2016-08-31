using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class SchoolPerson
    {
        [JsonProperty("districtid")]
        public Guid DistrictId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("role")]
        public CoreRole Role{ get; set; }

        [JsonProperty("currentschoolyearid")]
        public int CurrentSchoolYearId { get; set; }

        [JsonProperty("currentschoolyearname")]
        public string CurrentSchoolYearName { get; set; }

        [JsonProperty("schoolid")]
        public int SchoolId { get; set; }

        [JsonProperty("schoolname")]
        public string SchoolName { get; set; }

        [JsonProperty("districttimezone")]
        public string DistrictTimeZone { get; set; }
        
        [JsonProperty("districtname")]
        public string DistrictName { get; set; }

        [JsonProperty("statecode")]
        public string StateCode { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("birthdate")]
        public DateTime? BirthDate { get; set; }

        [JsonProperty("salutation")]
        public string Salutation { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("schoollocalid")]
        public int SchoolLocalId { get; set; }

        [JsonProperty("claims")]
        public IEnumerable<PersonClaim> Claims { get; set; }
        
        [JsonIgnore]
        public string UniquePersonId => GetUniquePersonId(Role, DistrictId, Id, CurrentSchoolYearId);

        public bool IsDistrictAdmin => Role.Id == CoreRoles.DISTRICT_ADMIN_ROLE.Id;
        public bool IsTeacher => Role.Id == CoreRoles.TEACHER_ROLE.Id;
        public bool IsStudent => Role.Id == CoreRoles.STUDENT_ROLE.Id;

        public static string GetUniquePersonId(CoreRole role, Guid districtId, int id, int currentSchoolYearId = -1)
        {
            return role.LoweredName + "_" + districtId + "--" + id + "--" + currentSchoolYearId;
        }

        public static readonly SchoolPerson SYSADMIN = new SchoolPerson {
                    CurrentSchoolYearId = -1,
                    DisplayName = "SysAdmin",
                    DistrictId = Guid.Empty,
                    FirstName = "SysAdmin",
                    LastName = "SysAdmin",
                    Id = -1,
                    Role = CoreRoles.SUPER_ADMIN_ROLE,
                    SchoolLocalId = -1,
                    Gender = string.Empty
            };
        }
}