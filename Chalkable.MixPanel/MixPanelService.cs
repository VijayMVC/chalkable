using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.Common;
using Mixpanel.NET.Engage;
using Mixpanel.NET.Events;

namespace Chalkable.MixPanel
{
    public static class MixPanelEvents
    {
        public const string AttachedDocument = "Attached document";
        public const string AttachedApp = "Attached App";
        public const string OpenedAnnouncement = "opened announcement";
        public const string OpenedApp = "opened app";
        public const string BoughtApp = "Bought app";
        public const string LaunchedApp = "Launched app";
        public const string ResetPassword = "Reset password";
        public const string ChangedPassword = "changed password";
        public const string ChangedEmail = "changed email";
        public const string SentMessageTo = "sent a message to";
        public const string CreatedNewItem = "created new item";
        public const string FinishedFirstStep = "finished first step";
        public const string FinishedSecondStep = "finished second step";
        public const string InvitedUser = "invited user";
        public static string LoggedInForTheFirstTime = "logged in for the first time";
        public static string SubmittedAppForApproval = "Submitted app for approval";
        public static string SelectedLive = "Selected \"Live\"";
        public static string CreatedApp = "Created App";
        public static string UpdatedDraft = "Updated draft";
    }

    public static class MixPanelService
    {
        private static string token;


        private const string MIXPANEL_TOKEN = "mixpanel-token";
        private const string MIXPANEL_USER_PREFIX = "mixpanel-user-";
        private static string MixPanelToken
        {
            get 
            {
                if (string.IsNullOrEmpty(token))
                {
                    token = ConfigurationManager.AppSettings[MIXPANEL_TOKEN];
                }
                return token;
            }
        }
        private static double unixTimeStamp(DateTime date) 
        {
            var unix_time = (date - new DateTime(1970, 1, 1, 0, 0, 0));
            return unix_time.TotalSeconds;
        }


        

        public static string MakeId(string email)
        {
            return MIXPANEL_USER_PREFIX + email;
        }


        private const string FIRST_NAME = "first_name";
        private const string LAST_NAME = "last_name";
        private const string SCHOOL = "school";
        private const string ROLE = "role";
        private const string INVITED_USER_EMAIL = "invited-user-email";

        public static void InvitedUser(string email, string inviteEmail, string firstName, string lastName, string school, string role)
        {
            var properties = new Dictionary<string, object>();
            properties[FIRST_NAME] = firstName;
            properties[LAST_NAME] = lastName;
            properties[SCHOOL] = school;
            properties[ROLE] = role;
            properties[INVITED_USER_EMAIL] = inviteEmail;
            SendEvent(email, MixPanelEvents.InvitedUser, properties);
        }

        public static void IdentifySysAdmin(string email, string firstName, string lastName,
            DateTime? firstLoginDate, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.SUPER_ADMIN_ROLE.LoweredName);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public static void IdentifyAdmin(string email, string firstName, string lastName, string schoolName,
            DateTime? firstLoginDate, string timeZoneId, string role, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public static void IdentifySignUpUser(string email, string username, string schoolName, 
            DateTime firstLoginDate, string timeZoneId, string role, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, username, "", schoolName, firstLoginDate, timeZoneId, role);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);  
            }
            
        }

        private const string GRADE = "grade";
        public static void IdentifyStudent(string email, string firstName, string lastName, string schoolName, 
            string grade, DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, CoreRoles.STUDENT_ROLE.LoweredName);
                properties[GRADE] = grade;
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string TEACHER_ROLE = "teacher";
        public static void IdentifyTeacher(string email, string firstName, string lastName, string schoolName,
            List<string> gradeLevels, List<string> classes, DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, "student");
                properties[CLASSES] = classes;
                properties[GRADE_LEVELS] = gradeLevels;
                properties[ROLE] = TEACHER_ROLE;
                //created at
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string FIRST_NAME_MIX = "$first_name";
        private const string LAST_NAME_MIX = "$last_name";
        private const string EMAIL_MIX = "$email";
        private const string LAST_LOGIN_MIX = "$last_login";
        private const string CREATED_MIX = "$created";

        private static Dictionary<string, object> prepareBasicProperties(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate, string timeZoneId, string role)
        {
            var properties = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(lastName))
            {
                if (!string.IsNullOrEmpty(firstName))
                {
                    if (email.ToLower() != firstName.ToLower())
                    {
                        var parts = Regex.Split(firstName.Trim(), @"\W+");
                        if (parts.Length > 1)
                        {
                            properties[FIRST_NAME_MIX] = parts[0];
                            properties[LAST_NAME_MIX] = parts[1];
                        }
                        else if (parts.Length == 1)
                        {
                            properties[LAST_NAME_MIX] = parts[0];
                        }
                    }
                    else
                    {
                        properties[LAST_NAME_MIX] = firstName;
                    }
                    
                }
               
            }
            else
            {
                properties[FIRST_NAME_MIX] = firstName;
                properties[LAST_NAME_MIX] = lastName;
            }

            properties[EMAIL_MIX] = email;

            if (!string.IsNullOrEmpty(schoolName))
            {
                properties[SCHOOL] = schoolName;
            }
            if (!string.IsNullOrEmpty(role))
            {
                properties[ROLE] = role;    
            }
            

            if (!string.IsNullOrEmpty(timeZoneId))
            {
                if (firstLoginDate.HasValue)
                    properties[CREATED_MIX] = firstLoginDate.Value.ToShortDateString().Replace("/", "-");

                properties[LAST_LOGIN_MIX] = DateTime.UtcNow.ConvertFromUtc(timeZoneId)
                                                    .ToShortDateString()
                                                    .Replace("/", "-");
            }
            return properties;
        }

        public static void IdentifyDeveloper(string email, string userName,
            DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, userName, "", "", firstLoginDate, timeZoneId, CoreRoles.DEVELOPER_ROLE.LoweredName);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public static void IdentifyParent(string email, string firstName, string lastName, DateTime? firstLoginDate, string ip)
        {
            try
            {
                var engage = new MixpanelEngage(MixPanelToken);
                var properties = prepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.PARENT_ROLE.LoweredName);
                //created at
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public static void FinishedStep(string email, string step)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, step, properties);
        }

        private const string DOCUMENTS = "documents";
        public static void AttachedDocument(string email, List<string> docs)
        {
            var properties = new Dictionary<string, object>();
            properties[DOCUMENTS] = docs;
            SendEvent(email, MixPanelEvents.AttachedDocument, properties);
        }

        private const string APPS = "apps";
        public static void AttachedApp(string email, List<string> apps)
        {
            var properties = new Dictionary<string, object>();
            properties[APPS] = apps;
            SendEvent(email, MixPanelEvents.AttachedApp, properties);        
        }



        private const string TITLE = "title";
        private const string CREATED_BY = "created-by";
        public static void OpenedAnnouncement(string email, string announcementType, string title, string createdBy)
        {
            var properties = new Dictionary<string, object>();
            properties[TYPE] = announcementType;
            properties[TITLE] = title;
            properties[CREATED_BY] = createdBy;
            SendEvent(email, MixPanelEvents.OpenedAnnouncement, properties);      
        }

        public static void Clicked(string eventName, string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, eventName, properties);
        }

        public static void OpenedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, MixPanelEvents.OpenedApp, properties);
        }

        public static void SelectedLive(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, MixPanelEvents.SelectedLive, properties);
        }

        public static void SubmittedForApprooval(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = prepareAppProperties(appName, shortDescription, subjects, price, pricePerSchool, pricePerClass);
            SendEvent(email, MixPanelEvents.SubmittedAppForApproval, properties);
        }

        public static void UpdatedDraft(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = prepareAppProperties(appName, shortDescription, subjects, price, pricePerSchool, pricePerClass);
            SendEvent(email, MixPanelEvents.UpdatedDraft, properties);
        }

        private const string APP_SHORT_DESCRIPTION = "app-short-description";
        private const string APP_SUBJECTS = "app-subjects";
        private const string APP_PRICE = "app-price";
        private const string FREE = "Free";
        private const string APP_PRICE_PER_CLASS = "app-price-per-class";
        private const string APP_PRICE_PER_SCHOOL = "app-price-per-school";
        private static Dictionary<string, object> prepareAppProperties(string appName, string shortDescription, string subjects, decimal price,
                                                       decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            properties[APP_SHORT_DESCRIPTION] = shortDescription;
            properties[APP_SUBJECTS] = subjects;
            properties[APP_PRICE] = price > 0 ? price.ToString() : FREE;

            if (pricePerSchool.HasValue)
            {
                properties[APP_PRICE_PER_SCHOOL] = pricePerSchool.Value > 0 ? pricePerSchool.Value.ToString() : FREE;
            }
            if (pricePerClass.HasValue)
            {
                properties[APP_PRICE_PER_CLASS] = pricePerClass.Value > 0 ? pricePerClass.Value.ToString() : FREE;
            }
            return properties;
        }

        public static void CreatedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, MixPanelEvents.CreatedApp, properties);
        }

        private const string INSTALLED_FOR_ALL = "installed-for-all";
        private const string CLASSES = "classes";
        private const string DEPARTMENTS = "departments";
        private const string GRADE_LEVELS = "grade-levels";
        public static void BoughtApp(string email, string appName, List<string> classes, List<string> departments, List<string> gradeLevels)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;

            if (classes.Count == 0 && departments.Count == 0 && gradeLevels.Count == 0)
            {
                properties[INSTALLED_FOR_ALL] = true;
            }
            else
            {
                if (classes.Count > 0) properties[CLASSES] = classes;
                if (departments.Count > 0) properties[DEPARTMENTS] = departments;
                if (gradeLevels.Count > 0) properties[GRADE_LEVELS] = gradeLevels;
            }
            SendEvent(email, MixPanelEvents.BoughtApp, properties);
        }

        private const string APP_NAME = "app-name";
        public static void LaunchedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, MixPanelEvents.LaunchedApp, properties);
        }

        public static void ResetPassword(string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, MixPanelEvents.ResetPassword, properties);
        }

        public static void ChangedPassword(string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, MixPanelEvents.ChangedPassword, properties);
        }

        private const string NEW_EMAIL = "new-email";
        public static void ChangedEmail(string email, string newEmail)
        {
            if (string.Compare(email, newEmail) != 0)
            {
                var properties = new Dictionary<string, object>();
                properties[NEW_EMAIL] = newEmail;
                SendEvent(email, MixPanelEvents.ChangedEmail, properties);
            }
        }

        public static void UserLoggedInForFirstTime(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate, string timeZoneId, string role)
        {
            var properties = prepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role);
            SendEvent(email, MixPanelEvents.LoggedInForTheFirstTime, properties);
        }

        private const string RECIPIENT = "recipient";
        public static void SentMessageTo(string email, string userName)
        {
            var properties = new Dictionary<string, object>();
            properties[RECIPIENT] = userName;
            SendEvent(email, MixPanelEvents.SentMessageTo, properties);
        }


        private const string TYPE = "type";
        private const string CLASS = "class";
        private const string APPS_ATTACHED = "apps-attached";
        private const string DOCS_ATTACHED = "docs-attached";
        public static void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached)
        {
            var properties = new Dictionary<string, object>();
            properties[TYPE] = type;
            properties[CLASS] = sClass;
            properties[APPS_ATTACHED] = appsAttached;
            properties[DOCS_ATTACHED] = docsAttached;
            SendEvent(email, MixPanelEvents.CreatedNewItem, properties);
        }

        private const string DISTINCT_ID = "distinct_id";
        private const string TIME = "time";
        private static void SendEvent(string email, string eventName, IDictionary<string, object> properties)
        {


            if (DemoUserService.IsDemoUser(email)) return;

            Action a = delegate
                {
                    try
                    {
                        var distinctId = MakeId(email);
                        properties[DISTINCT_ID] = MakeId(email);
                        properties[TIME] = unixTimeStamp(DateTime.UtcNow).ToString();
                        var tracker = new MixpanelTracker(MixPanelToken);

                        tracker.Track(eventName, properties);


                        var engageProperties = new Dictionary<string, object>();
                        engageProperties[eventName] = true;
                        var engage = new MixpanelEngage(token);
                        engage.Increment(distinctId, engageProperties);

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                };
            a.BeginInvoke(null, null);
        }
    }
}
