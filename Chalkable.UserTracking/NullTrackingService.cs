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

        public void IdentifyAdmin(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate,
            string timeZoneId, string role, string ip)
        {
        }

        public void IdentifySignUpUser(string email, string username, string schoolName, DateTime firstLoginDate, string timeZoneId,
            string role, string ip)
        {
        }

        public void IdentifyStudent(string email, string firstName, string lastName, string schoolName, string grade,
            DateTime? firstLoginDate, string timeZoneId, string ip)
        {
        }

        public void IdentifyTeacher(string email, string firstName, string lastName, string schoolName, List<string> gradeLevels, List<string> classes,
            DateTime? firstLoginDate, string timeZoneId, string ip)
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

        public void BoughtApp(string email, string appName, List<string> classes, List<string> departments, List<string> gradeLevels)
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
            DateTime? firstLoginDate, string timeZoneId, string role)
        {
        }

        public void SentMessageTo(string email, string userName)
        {
        }

        public void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached)
        {
        }
    }
}
