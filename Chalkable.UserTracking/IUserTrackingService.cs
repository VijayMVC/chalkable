using System;
using System.Collections.Generic;

namespace Chalkable.UserTracking
{
    public interface IUserTrackingService
    {
        void IdentifySysAdmin(string email, string firstName, string lastName,
            DateTime? firstLoginDate, string ip);

        void IdentifyDistrictAdmin(string email, string firstName, string lastName, string schoolName,
            DateTime? firstLoginDate, string timeZoneId, string role, string ip, bool isStudyCenterEnabled);

        void IdentifyStudent(string email, string firstName, string lastName, string schoolName, 
            string grade, DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled);

        void IdentifyTeacher(string email, string firstName, string lastName, string schoolName, List<string> classes,
            DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled);

        void IdentifyDeveloper(string email, string userName,
            DateTime? firstLoginDate, string timeZoneId, string ip);

        void IdentifyParent(string email, string firstName, string lastName, DateTime? firstLoginDate, string ip);
        void FinishedStep(string email, string step);
        void AttachedDocument(string email, List<string> docs);
        void AttachedApp(string email, List<string> apps);
        void OpenedAnnouncement(string email, string announcementType, string title, string createdBy);
        void Clicked(string eventName, string email);
        void OpenedApp(string email, string appName);
        void SelectedLive(string email, string appName);
        void SubmittedForApprooval(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass);
        void UpdatedDraft(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass);
        void CreatedApp(string email, string appName);
        void BoughtApp(string email, string appName, List<string> classes);
        void LaunchedApp(string email, string appName);
        void ResetPassword(string email);
        void ChangedPassword(string email);
        void ChangedEmail(string email, string newEmail);
        void UserLoggedInForFirstTime(string email, string firstName, string lastName, string schoolName, 
            DateTime? firstLoginDate, string timeZoneId, string role, bool isStudyCenterEnabled);
        void SentMessageTo(string email, string userName);

        void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached);
        void CreateNewLessonPlan(string email, string sClass, int appsAttached, int docsAttached);
        void CreateNewAdminItem(string email, string adminName, int appsAttached, int docsAttached);
        void CreatedReport(string email, string reportType);
        void SetDiscipline(string login, int? classId, DateTime date, string description, int studentId);
        void SetFinalGrade(string login, int classId, int studentId, int gradingPeriodId, string averageValue, bool exempt, string note);
        void SetAttendance(string login, int classId);
        void PostedGrades(string login, int classId, int gradingPeriodId);
        void LoggedIn(string login);
        void AttachedAssessment(string login, int announcementId);
        void AttachedStandard(string login, string name);
        void UsedStandardsExplorer(string login, string explorerType);
        void AutoGradedItem(string login, int announcementId, int studentId, string grade);

        void CopiedLessonPlanFromGallery(string login);
        void SavedLessonPlanToGallery(string login, string lessonPlanTitle);
    }
}