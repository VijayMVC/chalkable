using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Chalkable.Common;
using Mixpanel.NET.Engage;
using Mixpanel.NET.Events;

namespace Chalkable.UserTracking
{
    public class MixPanelService:IUserTrackingService
    {
        

        private bool IsDisabled { get { return string.IsNullOrEmpty(MixPanelToken); } }

        private const string MIXPANEL_USER_PREFIX = "mixpanel-user-";
        private string MixPanelToken { get; set; }


        public MixPanelService(string mixToken)
        {
            MixPanelToken = mixToken;
        }

        private static double UnixTimeStamp(DateTime date) 
        {
            var unix_time = (date - new DateTime(1970, 1, 1, 0, 0, 0));
            return unix_time.TotalSeconds;
        }


        private IEventTracker GetEventTracker()
        {
            if (IsDisabled)
                return new NullEventTracker();
            return new MixpanelTracker(MixPanelToken);
        }

        private IEngage GetEngage()
        {
            if (IsDisabled)
                return new NullEngage();
            return new MixpanelEngage(MixPanelToken);
        }


        public static string MakeId(string email)
        {
            return MIXPANEL_USER_PREFIX + email;
        }


        private const string DISTRICT = "district";
        private const string ROLE = "role";

        public void IdentifySysAdmin(string email, string firstName, string lastName,
            DateTime? firstLoginDate, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.SUPER_ADMIN_ROLE.LoweredName);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public void IdentifyAdmin(string email, string firstName, string lastName, string schoolName,
            DateTime? firstLoginDate, string timeZoneId, string role, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string GRADE = "grade";
        public void IdentifyStudent(string email, string firstName, string lastName, string schoolName, 
            string grade, DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, CoreRoles.STUDENT_ROLE.LoweredName);
                properties[GRADE] = grade;
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string TEACHER_ROLE = "teacher";
        public void IdentifyTeacher(string email, string firstName, string lastName, string schoolName,
            List<string> gradeLevels, List<string> classes, DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, "student");
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

        private static Dictionary<string, object> PrepareBasicProperties(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate, string timeZoneId, string role)
        {
            var properties = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(lastName))
            {
                if (!string.IsNullOrEmpty(firstName))
                {
                    if (!String.Equals(email, firstName, StringComparison.CurrentCultureIgnoreCase))
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
                properties[DISTRICT] = schoolName;
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

        public void IdentifyDeveloper(string email, string userName,
            DateTime? firstLoginDate, string timeZoneId, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, userName, "", "", firstLoginDate, timeZoneId, CoreRoles.DEVELOPER_ROLE.LoweredName);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public void IdentifyParent(string email, string firstName, string lastName, DateTime? firstLoginDate, string ip)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.PARENT_ROLE.LoweredName);
                //created at
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public void FinishedStep(string email, string step)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, step, properties);
        }

        private const string DOCUMENTS = "documents";
        public void AttachedDocument(string email, List<string> docs)
        {
            var properties = new Dictionary<string, object>();
            properties[DOCUMENTS] = docs;
            SendEvent(email, UserTrackingEvents.AttachedDocument, properties);
        }

        private const string APPS = "apps";
        public void AttachedApp(string email, List<string> apps)
        {
            var properties = new Dictionary<string, object>();
            properties[APPS] = apps;
            SendEvent(email, UserTrackingEvents.AttachedApp, properties);        
        }



        private const string TITLE = "title";
        private const string CREATED_BY = "created-by";
        public void OpenedAnnouncement(string email, string announcementType, string title, string createdBy)
        {
            var properties = new Dictionary<string, object>();
            properties[TYPE] = announcementType;
            properties[TITLE] = title;
            properties[CREATED_BY] = createdBy;
            SendEvent(email, UserTrackingEvents.OpenedAnnouncement, properties);      
        }

        public void Clicked(string eventName, string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, eventName, properties);
        }

        public void OpenedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, UserTrackingEvents.OpenedApp, properties);
        }

        public void SelectedLive(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, UserTrackingEvents.SelectedLive, properties);
        }

        public void SubmittedForApprooval(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = PrepareAppProperties(appName, shortDescription, subjects, price, pricePerSchool, pricePerClass);
            SendEvent(email, UserTrackingEvents.SubmittedAppForApproval, properties);
        }

        public void UpdatedDraft(string email, string appName, string shortDescription, string subjects, decimal price, decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = PrepareAppProperties(appName, shortDescription, subjects, price, pricePerSchool, pricePerClass);
            SendEvent(email, UserTrackingEvents.UpdatedDraft, properties);
        }

        private const string APP_SHORT_DESCRIPTION = "app-short-description";
        private const string APP_SUBJECTS = "app-subjects";
        private const string APP_PRICE = "app-price";
        private const string FREE = "Free";
        private const string APP_PRICE_PER_CLASS = "app-price-per-class";
        private const string APP_PRICE_PER_SCHOOL = "app-price-per-school";
        private static Dictionary<string, object> PrepareAppProperties(string appName, string shortDescription, string subjects, decimal price,
                                                       decimal? pricePerSchool, decimal? pricePerClass)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            properties[APP_SHORT_DESCRIPTION] = shortDescription;
            properties[APP_SUBJECTS] = subjects;
            properties[APP_PRICE] = price > 0 ? price.ToString() : FREE;

            if (pricePerSchool.HasValue)
            {
                properties[APP_PRICE_PER_SCHOOL] = pricePerSchool.Value > 0 ? pricePerSchool.Value.ToString(CultureInfo.InvariantCulture) : FREE;
            }
            if (pricePerClass.HasValue)
            {
                properties[APP_PRICE_PER_CLASS] = pricePerClass.Value > 0 ? pricePerClass.Value.ToString(CultureInfo.InvariantCulture) : FREE;
            }
            return properties;
        }

        public void CreatedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, UserTrackingEvents.CreatedApp, properties);
        }

        private const string INSTALLED_FOR_ALL = "installed-for-all";
        private const string CLASSES = "classes";
        private const string DEPARTMENTS = "departments";
        private const string GRADE_LEVELS = "grade-levels";
        public void BoughtApp(string email, string appName, List<string> classes, List<string> departments, List<string> gradeLevels)
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
            SendEvent(email, UserTrackingEvents.BoughtApp, properties);
        }

        private const string APP_NAME = "app-name";
        public void LaunchedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            SendEvent(email, UserTrackingEvents.LaunchedApp, properties);
        }

        public void ResetPassword(string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, UserTrackingEvents.ResetPassword, properties);
        }

        public void ChangedPassword(string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, UserTrackingEvents.ChangedPassword, properties);
        }

        private const string NEW_EMAIL = "new-email";
        public void ChangedEmail(string email, string newEmail)
        {
            if (string.CompareOrdinal(email, newEmail) == 0) return;

            var properties = new Dictionary<string, object>();
            properties[NEW_EMAIL] = newEmail;
            SendEvent(email, UserTrackingEvents.ChangedEmail, properties);
        }

        public void UserLoggedInForFirstTime(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate, string timeZoneId, string role)
        {
            var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role);
            SendEvent(email, UserTrackingEvents.LoggedInForTheFirstTime, properties);
        }

        private const string RECIPIENT = "recipient";
        public void SentMessageTo(string email, string userName)
        {
            var properties = new Dictionary<string, object>();
            properties[RECIPIENT] = userName;
            SendEvent(email, UserTrackingEvents.SentMessageTo, properties);
        }


        private const string TYPE = "type";
        private const string CLASS = "class";
        private const string APPS_ATTACHED = "apps-attached";
        private const string DOCS_ATTACHED = "docs-attached";
        public void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached)
        {
            var properties = new Dictionary<string, object>();
            properties[TYPE] = type;
            properties[CLASS] = sClass;
            properties[APPS_ATTACHED] = appsAttached;
            properties[DOCS_ATTACHED] = docsAttached;
            SendEvent(email, UserTrackingEvents.CreatedNewItem, properties);
        }

        private const string REPORT_TYPE = "report-type";
        public void CreatedReport(string email, string reportType)
        {
            var properties = new Dictionary<string, object>();
            properties[REPORT_TYPE] = reportType;
            SendEvent(email, UserTrackingEvents.CreatedReport, properties);
        }

        private const string DESCRIPTION = "description";
        private const string STUDENT_ID = "studentId";

        public void SetDiscipline(string login, int? classId, DateTime date, string description, int studentId)
        {
            var properties = new Dictionary<string, object>();
            properties[DESCRIPTION] = description;
            properties[CLASS] = classId.HasValue ? classId.Value.ToString(CultureInfo.InvariantCulture) : "";
            properties[STUDENT_ID] = studentId.ToString(CultureInfo.InvariantCulture);
            SendEvent(login, UserTrackingEvents.SetDiscipline, properties);
        }

        private const string GRADING_PERIOD_ID = "gradingPeriodId";
        private const string AVG_VALUE = "average";
        private const string NOTE = "note";
        private const string IS_EXEMPT = "exempt";

        public void SetFinalGrade(string login, int classId, int studentId, int gradingPeriodId, string averageValue, bool exempt,
            string note)
        {
            var properties = new Dictionary<string, object>();
            properties[CLASS] = classId;
            properties[STUDENT_ID] = studentId;
            properties[AVG_VALUE] = averageValue;
            properties[GRADING_PERIOD_ID] = gradingPeriodId;
            properties[NOTE] = note;
            properties[IS_EXEMPT] = exempt;
            SendEvent(login, UserTrackingEvents.SetFinalGrade, properties);
        }

        private const string ANNOUNCEMENT_ID = "announcementId";
        private const string EXTRA_CREDITS = "extraCredits";

        public void SetScore(string login, int announcementId, int studentId, string gradeValue, string extraCredits)
        {
            var properties = new Dictionary<string, object>();
            properties[ANNOUNCEMENT_ID] = announcementId;
            properties[STUDENT_ID] = studentId;
            properties[GRADE] = gradeValue;
            properties[EXTRA_CREDITS] = extraCredits;
            SendEvent(login, UserTrackingEvents.SetScore, properties);
        }

        public void SetAttendance(string login, int classId)
        {
            var properties = new Dictionary<string, object>();
            properties[CLASS] = classId;
            SendEvent(login, UserTrackingEvents.SetAttendance, properties);
        }

        private const string DISTINCT_ID = "distinct_id";
        private const string TIME = "time";
        private void SendEvent(string email, string eventName, IDictionary<string, object> properties)
        {
            Action a = delegate
                {
                    try
                    {
                        var distinctId = MakeId(email);
                        properties[DISTINCT_ID] = MakeId(email);
                        properties[TIME] = UnixTimeStamp(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture);
                        var tracker = GetEventTracker();
                        tracker.Track(eventName, properties);


                        var engageProperties = new Dictionary<string, object>();
                        engageProperties[eventName] = true;
                        var engage = GetEngage();
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

    class NullEngage : IEngage
    {
        public bool Set(string distinctId, string ip, IDictionary<string, object> setProperties)
        {
            return true;
        }

        public bool Increment(string distinctId, IDictionary<string, object> incrementProperties)
        {
            return true;
        }
    }

    class NullEventTracker : IEventTracker
    {
        public bool Track(string ev, IDictionary<string, object> properties)
        {
            return true;
        }

        public bool Track<T>(T @event)
        {
            return true;
        }

        public bool Track(MixpanelEvent @event)
        {
            return true;
        }
    }
}
