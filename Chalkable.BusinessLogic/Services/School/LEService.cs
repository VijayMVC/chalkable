using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILEService
    {
        string GetBaseUrl();
        bool IsLELinkActive();
        string BuildLECreditsUrl(int? classId);
        string BuildIntegratedSingOnUrl();
        string BuildNonIntegratedSingOnUrl(int schoolId, int userId, string firstName, string lastName);
        string BuildLESingOnUrl();
        LEParams GetLEParams();
    }

    public class LEService : SchoolServiceBase, ILEService
    {
        private const string LEARNING_EARNIGS_CATEGORY = "LearningEarnings";

        public LEService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public string GetBaseUrl()
        {
            var leUrlSetting = ServiceLocator.SettingsService.GetSetting(LEARNING_EARNIGS_CATEGORY, "Url");
            return leUrlSetting?.Value;
        }

        public bool IsLELinkActive()
        {
            var leLinkSetting = ServiceLocator.SettingsService.GetSetting(LEARNING_EARNIGS_CATEGORY, "LinkStatus");
            return leLinkSetting != null && leLinkSetting.Value == "active";
        }

        public string BuildLECreditsUrl(int? classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (!CanGiveCredits())
                throw new ChalkableSecurityException();

            var syId = Context.SchoolYearId.Value;
            IList<int> studentIds;
            if (!classId.HasValue)
                studentIds = ServiceLocator.StudentService.GetTeacherStudents(Context.PersonId.Value, syId).Select(x => x.Id).ToList();
            else
                studentIds = ServiceLocator.ClassService.GetClassPersons(null, classId.Value, null, null).Select(x => x.PersonRef).Distinct().ToList();

            var clsPersons = studentIds.JoinString(",");
            var integratedSignOnUrl = $"{GetBaseUrl()}sti/give_credits?districtGUID={Context.DistrictId}" +
                                      $"&sti_session_variable={Context.SisToken}&studentIds={clsPersons}&sti_school_id={Context.SchoolLocalId.Value}";
            return integratedSignOnUrl;
        }

        public string BuildLESingOnUrl()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (Context.LEEnabled && IsLELinkActive())
                return BuildIntegratedSingOnUrl();

            if (Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var person = ServiceLocator.StudentService.GetById(Context.PersonId.Value, Context.SchoolYearId.Value);
                return BuildNonIntegratedSingOnUrl(Context.SchoolLocalId.Value, person.UserId, person.FirstName, person.LastName);
            }
            if (BaseSecurity.IsDistrictOrTeacher(Context))
            {
                var person = ServiceLocator.StaffService.GetStaff(Context.PersonId.Value);
                return BuildNonIntegratedSingOnUrl(Context.SchoolLocalId.Value, person.UserId.Value, person.FirstName, person.LastName);
            }
            else
            {
                var person = ServiceLocator.PersonService.GetPerson(Context.PersonId.Value);
                return BuildNonIntegratedSingOnUrl(Context.SchoolLocalId.Value, person.UserId.Value, person.FirstName, person.LastName);
            }
        }


        public string BuildIntegratedSingOnUrl()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            if(!CanSignOn())
                throw new ChalkableSecurityException();

            //var syId = Context.SchoolYearId.Value;
            //IList<int> studentIds = new List<int>();
            //if (BaseSecurity.IsTeacher(Context))
            //    studentIds = ServiceLocator.StudentService.GetTeacherStudents(Context.PersonId.Value, syId).Select(x => x.Id).ToList();
            
            //else studentIds.Add(Context.PersonId.Value);
            return $"{GetBaseUrl()}sti/auth?districtGUID={Context.DistrictId}&sti_session_variable={Context.SisToken}&sti_school_id={Context.SchoolLocalId.Value}";
        }

        public string BuildNonIntegratedSingOnUrl(int schoolId, int userId, string firstName, string lastName)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return $"{GetBaseUrl()}sti/auth?districtGUID={Context.DistrictId}&schoolId={schoolId}&userid={userId}&firstname={firstName}&lastname={lastName}";
        }

        public LEParams GetLEParams()
        {
            var hasLeAccess = HasLEAccess();
            return new LEParams
            {
                LEEnabled = Context.LEEnabled,
                LESyncComplete = Context.LESyncComplete,
                LELinkStatus = IsLELinkActive(),
                LEBaseUrl = GetBaseUrl(),
                AwardLECredits = HasLECreditsPermission(),
                AwardLECreditsClassroom = HasLECreditsClassroomPermission(),
                IssueLECreditsEnabled = hasLeAccess && BaseSecurity.IsTeacher(Context),
                LEAccessEnabled = hasLeAccess,
            };
        }

        private bool CanSignOn()
        {
            return Context.LEEnabled && Context.LESyncComplete && HasLEAccess();
        }

        private bool CanGiveCredits()
        {
            return Context.LEEnabled && Context.LESyncComplete && HasLEAccess() 
                    && IsLELinkActive() && BaseSecurity.IsTeacher(Context);
        }

        private bool HasLEAccess()
        {
            return HasLECreditsPermission() || HasLECreditsClassroomPermission() || CoreRoles.STUDENT_ROLE == Context.Role;
        }

        private bool HasLECreditsPermission()
        {
            return Context.Claims.HasPermission(ClaimInfo.AWARD_LE_CREDITS);
        }

        private bool HasLECreditsClassroomPermission()
        {
            return Context.Claims.HasPermission(ClaimInfo.AWARD_LE_CREDITS_CLASSROOM);
        }

    }
}
