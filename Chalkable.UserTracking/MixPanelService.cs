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
        
        private bool IsDisabled => string.IsNullOrEmpty(MixPanelToken);

        private const string MIXPANEL_USER_PREFIX = "mixpanel-user-";
        private string MixPanelToken { get; }


        public MixPanelService(string mixToken)
        {
            MixPanelToken = mixToken;
        }

        private static double UnixTimeStamp(DateTime date) 
        {
            var unixTime = (date - new DateTime(1970, 1, 1, 0, 0, 0));
            return unixTime.TotalSeconds;
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
                var properties = PrepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.SUPER_ADMIN_ROLE.LoweredName, false);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        public void IdentifyDistrictAdmin(string email, string firstName, string lastName, string schoolName,
            DateTime? firstLoginDate, string timeZoneId, string role, string ip, bool isStudyCenterEnabled)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role, isStudyCenterEnabled);
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string GRADE = "grade";
        public void IdentifyStudent(string email, string firstName, string lastName, string schoolName, 
            string grade, DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, 
                    timeZoneId, CoreRoles.STUDENT_ROLE.LoweredName, isStudyCenterEnabled);
                properties[GRADE] = grade;
                engage.Set(MakeId(email), ip, properties);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private const string TEACHER_ROLE = "teacher";
        public void IdentifyTeacher(string email, string firstName, string lastName, string schoolName, List<string> classes, 
            DateTime? firstLoginDate, string timeZoneId, string ip, bool isStudyCenterEnabled)
        {
            try
            {
                var engage = GetEngage();
                var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, "student", isStudyCenterEnabled);
                properties[CLASSES] = classes;
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
        private const string IS_STUDYCENTER_CUSTOMER = "Study Center Customer";

        private static Dictionary<string, object> PrepareBasicProperties(string email, string firstName, string lastName, string schoolName, DateTime? firstLoginDate, string timeZoneId, string role, bool isStudyCenterCustomer)
        {
            var properties = new Dictionary<string, object>
            {
                [IS_STUDYCENTER_CUSTOMER] = isStudyCenterCustomer.ToString()
            };


            if (string.IsNullOrEmpty(lastName))
            {
                if (!string.IsNullOrEmpty(firstName))
                {
                    if (!string.Equals(email, firstName, StringComparison.CurrentCultureIgnoreCase))
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
                var properties = PrepareBasicProperties(email, userName, "", "", firstLoginDate, timeZoneId, CoreRoles.DEVELOPER_ROLE.LoweredName, false);
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
                var properties = PrepareBasicProperties(email, firstName, lastName, "", firstLoginDate, "", CoreRoles.PARENT_ROLE.LoweredName, false);
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

        public void SubmittedForApprooval(string email, string appName, string shortDescription, string subjects)
        {
            var properties = PrepareAppProperties(appName, shortDescription, subjects);
            SendEvent(email, UserTrackingEvents.SubmittedAppForApproval, properties);
        }

        public void UpdatedDraft(string email, string appName, string shortDescription, string subjects)
        {
            var properties = PrepareAppProperties(appName, shortDescription, subjects);
            SendEvent(email, UserTrackingEvents.UpdatedDraft, properties);
        }

        private const string APP_SHORT_DESCRIPTION = "app-short-description";
        private const string APP_SUBJECTS = "app-subjects";
        private const string APP_PRICE = "app-price";
        private const string FREE = "Free";
        private const string APP_PRICE_PER_CLASS = "app-price-per-class";
        private const string APP_PRICE_PER_SCHOOL = "app-price-per-school";
        private static Dictionary<string, object> PrepareAppProperties(string appName, string shortDescription, string subjects)
        {
            var properties = new Dictionary<string, object>();
            properties[APP_NAME] = appName;
            properties[APP_SHORT_DESCRIPTION] = shortDescription;
            properties[APP_SUBJECTS] = subjects;

            return properties;
        }

        public void CreatedApp(string email, string appName)
        {
            var properties = new Dictionary<string, object>
            {
                [APP_NAME] = appName
            };
            SendEvent(email, UserTrackingEvents.CreatedApp, properties);
        }

        private const string CLASSES = "classes";
        public void BoughtApp(string email, string appName, List<string> classes)
        {
            var properties = new Dictionary<string, object>
            {
                [APP_NAME] = appName
            };
            if (classes.Count > 0) properties[CLASSES] = classes;
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

        public void UserLoggedInForFirstTime(string email, string firstName, string lastName, string schoolName, 
            DateTime? firstLoginDate, string timeZoneId, string role, bool isStudyCenterEnabled)
        {
            var properties = PrepareBasicProperties(email, firstName, lastName, schoolName, firstLoginDate, timeZoneId, role, isStudyCenterEnabled);
            SendEvent(email, UserTrackingEvents.LoggedInForTheFirstTime, properties);
        }

        private const string RECIPIENT = "recipient";
        public void SentMessageTo(string email, string userName)
        {
            var properties = new Dictionary<string, object>
            {
                [RECIPIENT] = userName            
            };
            SendEvent(email, UserTrackingEvents.SentMessageTo, properties);
        }

        private const string TYPE = "type";
        private const string CLASS = "class";
        private const string APPS_ATTACHED = "apps-attached";
        private const string DOCS_ATTACHED = "docs-attached";
        private const string NUMBER_OF_STUDENTS = "number-of-students";
        private const string CLASS_DISCUSSION = "class-discussion";
        public void CreatedNewItem(string email, string type, string sClass, int appsAttached, int docsAttached, bool includeDiscussion)
        {
            var properties = new Dictionary<string, object>
            {
                [TYPE] = type,
                [CLASS] = sClass,
                [APPS_ATTACHED] = appsAttached,
                [DOCS_ATTACHED] = docsAttached,
                [CLASS_DISCUSSION] = includeDiscussion
            };
            SendEvent(email, UserTrackingEvents.CreatedNewItem, properties);
        }

        public void CreateNewLessonPlan(string email, string sClass, int appsAttached, int docsAttached, bool includeDiscussion)
        {
            var properties = new Dictionary<string, object>
            {
                [CLASS] = sClass,
                [APPS_ATTACHED] = appsAttached,
                [DOCS_ATTACHED] = docsAttached,
                [CLASS_DISCUSSION] = includeDiscussion
            };
            SendEvent(email, UserTrackingEvents.CreatedNewLessonPlan, properties);
        }

        public void CreateNewSupplemental(string email, string sClass, int studentsCount, int appsAttached, int docsAttached, bool includeDiscussion)
        {
            var properties = new Dictionary<string, object>
            {
                [NUMBER_OF_STUDENTS] = studentsCount,
                [CLASS] = sClass,
                [APPS_ATTACHED] = appsAttached,
                [DOCS_ATTACHED] = docsAttached,
                [CLASS_DISCUSSION] = includeDiscussion
            };
            SendEvent(email, UserTrackingEvents.CreateNewSupplemental, properties);
        }

        private const string ADMIN = "admin";
        public void CreateNewAdminItem(string email, string adminName,  int appsAttached, int docsAttached)
        {
            var properties = new Dictionary<string, object>
            {
                [ADMIN] = adminName,
                [APPS_ATTACHED] = appsAttached,
                [DOCS_ATTACHED] = docsAttached
            };
            SendEvent(email, UserTrackingEvents.CreatedNewAdminItem, properties);       
        }

        private const string NUMBER_OF_ITEMS = "number-of-items";
        private const string FROM_CLASS = "from-class";
        private const string TO_CLASS = "to-class";
        public void CopyItems(string email, string toClass, int itemsCount)
        {
            var properties = new Dictionary<string, object>
            {
                [NUMBER_OF_ITEMS] = itemsCount,
                [TO_CLASS] = toClass
            };
            SendEvent(email, UserTrackingEvents.CopyItems, properties);
        }

        public void ImportItems(string email, string fromClass, int itemsCount)
        {
            var properties = new Dictionary<string, object>
            {
                [NUMBER_OF_ITEMS] = itemsCount,
                [FROM_CLASS] = fromClass
            };
            SendEvent(email, UserTrackingEvents.ImportItems, properties);
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
            properties[CLASS] = classId?.ToString(CultureInfo.InvariantCulture) ?? "";
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
        private const string CALLED_FROM = "calledFrom";

        public void SetAttendance(string login, int classId)
        {
            var properties = new Dictionary<string, object>();
            properties[CLASS] = classId;
            SendEvent(login, UserTrackingEvents.SetAttendance, properties);
        }

        public void PostedGrades(string login, int classId, int gradingPeriodId)
        {
            var properties = new Dictionary<string, object>();
            properties[CLASS] = classId;
            properties[GRADING_PERIOD_ID] = gradingPeriodId;
            SendEvent(login, UserTrackingEvents.PostedGrades, properties);
        }

        public void LoggedIn(string login)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(login, UserTrackingEvents.LoggedIn, properties);
        }

        public void AttachedAssessment(string login, int announcementId)
        {
            var properties = new Dictionary<string, object>();
            properties[ANNOUNCEMENT_ID] = announcementId;
            SendEvent(login, UserTrackingEvents.AttachedAssessment, properties);
        }

        private const string STANDARD_NAME = "standard-name";
        public void AttachedStandard(string login, string standardName)
        {
            var properties = new Dictionary<string, object>();
            properties[STANDARD_NAME] = standardName;
            SendEvent(login, UserTrackingEvents.AttachedStandard, properties);
        }

        private const string STANDARD_EXPLORER_TYPE = "explorer-type";
        public void UsedStandardsExplorer(string login, string explorerType)
        {
            var properties = new Dictionary<string, object>();
            properties[STANDARD_EXPLORER_TYPE] = explorerType;
            SendEvent(login, UserTrackingEvents.UsedStandardsExplorer, properties);
        }

        public void AutoGradedItem(string login, int announcementId, int studentId, string grade)
        {
            var properties = new Dictionary<string, object>
            {
                [ANNOUNCEMENT_ID] = announcementId,
                [STUDENT_ID] = studentId,
                [GRADE] = grade
            };
            SendEvent(login, UserTrackingEvents.AutoGradedItem, properties);
        }

        public void CopiedLessonPlanFromGallery(string email)
        {
            var properties = new Dictionary<string, object>();
            SendEvent(email, UserTrackingEvents.CopiedLessonPlanFromGallery, properties);
        }


        private const string LESSON_PLAN_TITLE = "lesson plan title";
        public void SavedLessonPlanToGallery(string email, string lessonPlanTitle)
        {
            var properties = new Dictionary<string, object> {[LESSON_PLAN_TITLE] = lessonPlanTitle};
            SendEvent(email, UserTrackingEvents.SavedLessonPlanToGallery, properties);
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
