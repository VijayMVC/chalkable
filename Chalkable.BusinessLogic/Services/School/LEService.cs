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
        string BuildNonIntegratedSingOnUlr(int schoolId, int userId, string firstName, string lastName);
        string BuildLESingOnUlr();
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
            return leUrlSetting != null ? leUrlSetting.Value : null;
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
            var integratedSignOnUrl = string.Format(GetBaseUrl() + "sti/give_credits?districtGUID={0}&sti_session_variable={1}&studentIds={2}&sti_school_id={3}"
                , Context.DistrictId, Context.SisToken, clsPersons, Context.SchoolLocalId.Value);
            return integratedSignOnUrl;
        }

        public string BuildLESingOnUlr()
        {
            Trace.Assert(Context.PersonId.HasValue);
            if (Context.LEEnabled && IsLELinkActive())
                return BuildIntegratedSingOnUrl();

            var person = ServiceLocator.PersonService.GetPerson(Context.PersonId.Value);
            return BuildNonIntegratedSingOnUlr(person.SchoolRef, person.UserId.Value, person.FirstName, person.LastName);
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
            return string.Format(GetBaseUrl() + "sti/auth?districtGUID={0}&sti_session_variable={1}&sti_school_id={2}"
                , Context.DistrictId, Context.SisToken, Context.SchoolLocalId.Value);
        }

        public string BuildNonIntegratedSingOnUlr(int schoolId, int userId, string firstName, string lastName)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return string.Format(GetBaseUrl() + "sti/auth?districtGUID={0}&schoolid={1}&userid={2}&firstname={3}&lastname={4}"
                , Context.DistrictId, schoolId, userId, firstName, lastName);
        }

        public LEParams GetLEParams()
        {
            bool enabled = HasLEAccess();
            return new LEParams
            {
                LEEnabled = Context.LEEnabled,
                LESyncComplete = Context.LESyncComplete,
                LELinkStatus = IsLELinkActive(),
                LEBaseUrl = GetBaseUrl(),
                AwardLECredits = HasLECreditsPermission(),
                AwardLECreditsClassroom = HasLECreditsClassroomPermission(),
                IssueLECreditsEnabled = enabled,
                LEAccessEnabled = enabled,
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
            return HasLECreditsPermission() || HasLECreditsClassroomPermission();
        }

        private bool HasLECreditsPermission()
        {
            return ClaimInfo.HasPermissions(Context.Claims, new List<string> {ClaimInfo.AWARD_LE_CREDITS});
        }

        private bool HasLECreditsClassroomPermission()
        {
            return ClaimInfo.HasPermissions(Context.Claims, new List<string> {ClaimInfo.AWARD_LE_CREDITS_CLASSROOM});
        }

    }
}
