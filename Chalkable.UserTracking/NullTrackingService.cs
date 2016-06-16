using System;
using System.Collections.Generic;

namespace Chalkable.UserTracking
{
    public class NullTrackingService: IUserTrackingService
    {
        public void InvitedUser(string email, string inviteEmail, string firstName, string lastName, string school, string role)
        {
        }

        public void IdentifySysAdmin(string email, string firstName, string lastName, DateTime? firstLoginDate, string ip)
        {
        }

        public void IdentifyDistrictAdmin(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate,
            string timeZoneId, string role, string ip, bool isStudyCenterEnabled)
        {
        }

        public void IdentifySignUpUser(string email, string username, string schoolName, DateTime firstLoginDate, string timeZoneId,
            string role, string ip)
        {
        }

        public void IdentifyStudent(string email, string firstName, string lastName, string schoolName, string grade,
            DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled)
        {
        }

        public void IdentifyTeacher(string email, string firstName, string lastName, string schoolName, List<string> classes,
            DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled)
        {
        }

        public void IdentifyDeveloper(string email, string userName, DateTime? firstLoginDate, string timeZoneId, string ip)
        {
        }

        public void IdentifyParent(string email, string firstName, string lastName, DateTime? firstLoginDate, string ip)
        {
        }

        public void FinishedStep(string email, string step)
        {
        }

        public void AttachedDocument(string email, List<string> docs)
        {
        }

        public void AttachedApp(string email, List<string> apps)
        {
        }

        public void OpenedAnnouncement(string email, string announcementType, string title, string createdBy)
        {
        }

        public void Clicked(string eventName, string email)
        {
        }

        public void OpenedApp(string email, string appName)
        {
        }

        public void SelectedLive(string email, string appName)
        {
        }

        public void SubmittedForApprooval(string email, string appName, string shortDescription, string subjects, decimal price,
            decimal? pricePerSchool, decimal? pricePerClass)
        {
        }

        public void UpdatedDraft(string email, string appName, string shortDescription, string subjects, decimal price,
            decimal? pricePerSchool, decimal? pricePerClass)
        {
        }

        public void CreatedApp(string email, string appName)
        {
        }

        public void BoughtApp(string email, string appName, List<string> classes)
        {
        }

        public void LaunchedApp(string email, string appName)
        {
        }

        public void ResetPassword(string email)
        {
        }

        public void ChangedPassword(string email)
        {
        }

        public void ChangedEmail(string email, string newEmail)
        {
        }

        public void UserLoggedInForFirstTime(string email, string firstName, string lastName, string schoolName,
            DateTime? firstLoginDate, string timeZoneId, string role, bool isStudyCenterEnabled)
        {
        }

        public void SentMessageTo(string email, string userName)
        {
        }

        public void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached)
        {
        }

        public void CreatedReport(string email, string reportType)
        {
        }

        public void SetDiscipline(string login, int? classId, DateTime date, string description, int studentId)
        {
        }

        public void SetFinalGrade(string login, int classId, int studentId, int gradingPeriodId, string averageValue, bool exempt,
            string note)
        {
        }

        public void SetAttendance(string login, int classId)
        {
        }

        public void PostedGrades(string login, int classId, int gradingPeriodId)
        {
        }

        public void LoggedIn(string login)
        {
        }

        public void AttachedAssessment(string login, int announcementId)
        {
        }

        public void TookAssessment(string login)
        {
        }

        public void AttachedStandard(string login, string name)
        {
        }

        public void UsedStandardsExplorer(string login, string explorerType)
        {
        }

        public void AutoGradedItem(string login, int announcementId, int studentId, string grade)
        {

        }

        public void CopiedLessonPlanFromGallery(string login)
        {
        }

        public void SavedLessonPlanToGallery(string login, string lessonPlanTitle)
        {
        }

        public void CreateNewLessonPlan(string email, string sClass, int appsAttached, int docsAttached)
        {
        }

        public void CreateNewAdminItem(string email, string adminName, int appsAttached, int docsAttached)
        {
        }
    }
}
