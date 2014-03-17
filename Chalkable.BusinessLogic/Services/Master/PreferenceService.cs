using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;


namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IPreferenceService
    {
        IList<Preference> List(PreferenceCategoryEnum? category = null);
        IList<Preference> ListPublic();
        Preference Set(string key, object value, bool isPublic);

    }

    public class PreferenceService : MasterServiceBase, IPreferenceService
    {
        private static Dictionary<string, Preference> cache = new Dictionary<string, Preference>();

        static PreferenceService()
        {
            var defaultList = DefaultList();
            using (var uow = new UnitOfWork(Settings.MasterConnectionString, false))
            {
                var preferences = new PreferenceDataAccess(uow).GetAll();
                foreach (var defaultelem in defaultList)
                {
                    var preference = preferences.FirstOrDefault(x => x.Key == defaultelem.Key);
                    var p = preference ?? CreatePreference(false, defaultelem.Key, defaultelem.Value);
                    cache.Add(p.Key, p);
                }
            }
            
        }


        public PreferenceService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<Preference> List(PreferenceCategoryEnum? category = null)
        {
            var res = cache.Values.AsEnumerable();
            if (category.HasValue)
                res = res.Where(x => x.Category == category.Value);
            return res.ToList();
        }

        public static Preference Get(string key)
        {
            return cache[key];
        }

        public static T GetTyped<T>(string key)
        {
            var preference = Get(key);
            var res = (T)Activator.CreateInstance(typeof(T), new object[] { preference.Value });
            return res;
        }

        public Preference Set(string key, object value, bool isPublic)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException(ChlkResources.ERR_PREFERENCE_INVALID_RIGHTS);

            using (var uow = Update())
            {
                var da = new PreferenceDataAccess(uow);
                var preference = da.GetPreferenceOrNull(key);
                if (preference == null)
                {
                    preference = CreatePreference(isPublic, key, new PreferenceInfo
                    {
                        Category = cache[key].Category,
                        Type = cache[key].Type,
                        Value = value.ToString(),
                        Hint = cache[key].Hint
                    });
                    da.Insert(preference);
                }
                else
                {
                    preference.Value = value.ToString();
                    preference.IsPublic = isPublic;
                    da.Update(preference);
                }
                cache[key] = preference;
                uow.Commit();
                return preference;
            }

        }

        public IList<Preference> ListPublic()
        {
            return cache.Values.Where(x => x.IsPublic).ToList();
        }

        public static Preference GetPublic(string key)
        {
            var res = cache[key];
            return res.IsPublic ? res : null;
        }

        private static Preference CreatePreference(bool ispublic, string key, PreferenceInfo preferenceInfo)
        {
            return new Preference
                          {
                              Id = Guid.NewGuid(),
                              Category = preferenceInfo.Category,
                              Type = preferenceInfo.Type,
                              Value = preferenceInfo.Value,
                              Hint = preferenceInfo.Hint,
                              IsPublic = ispublic,
                              Key = key
                          };
        }

        private static IDictionary<string, PreferenceInfo> DefaultList()
        {
            var res = new Dictionary<string, PreferenceInfo>();
            res.Add(Preference.DEMO_DISTRICT_ID, new PreferenceInfo { Value = "4562e5bb-f5f2-42bd-aab4-3c61ba775581" });
            res.Add(Preference.DEMO_SCHOOL_ADMIN_GRADE, new PreferenceInfo { Value = "user2735_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com" });
            res.Add(Preference.DEMO_SCHOOL_ADMIN_EDIT, new PreferenceInfo { Value = "" });
            res.Add(Preference.DEMO_SCHOOL_ADMIN_VIEW, new PreferenceInfo { Value = "" });
            res.Add(Preference.DEMO_SCHOOL_TEACHER, new PreferenceInfo { Value = "user1195_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com" });
            res.Add(Preference.DEMO_SCHOOL_STUDENT, new PreferenceInfo { Value = "user19_4562e5bb-f5f2-42bd-aab4-3c61ba775581@chalkable.com" });
            res.Add(Preference.DEMO_USER_PASSWORD, new PreferenceInfo {Value = "Qwerty1@"});
            res.Add(Preference.DEMO_SCHOOL_PROCESSING_PARAMETERS, new PreferenceInfo { Value = "20|10|10|5000|3600" });
            res.Add(Preference.SERVER_NAME, new PreferenceInfo { Value = "smtp.sendgrid.net" });
            res.Add(Preference.SERVER_PORT, new PreferenceInfo { Value = "587" });
            res.Add(Preference.SYSTEM_EMAIL, new PreferenceInfo { Value = "info@chalkable.com|chalkable|chalktemp1" });
            res.Add(Preference.NOTIFICATION_SYSTEM_EMAIL, new PreferenceInfo { Value = "info@chalkable.com|chalkable|chalktemp1" });
            res.Add(Preference.FORGOTPASSWORD_SYSTEM_EMAIL, new PreferenceInfo { Value = "info@chalkable.com|chalkable|chalktemp1" });
            res.Add(Preference.SYSTEM_NAME, new PreferenceInfo { Value = "Chalkable" });
            res.Add(Preference.CROCODOC_TOKEN, new PreferenceInfo { Value = "xEyUsXCwkjRMsNehL6CL7pyR" });
            res.Add(Preference.CROCODOC_API_URL, new PreferenceInfo { Value = "https://crocodoc.com/api/v2/" });
            res.Add(Preference.CROCODOC_URL, new PreferenceInfo { Value = "https://crocodoc.com/" });
            res.Add(Preference.APPLICATION_URL, new PreferenceInfo { Value = "http://dev.chalkable.com" });
            res.Add(Preference.VIDEO_GETING_INFO_CHALKABLE, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_GETING_INFO_READY, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_UPLOAD_TEACHER_INFO, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_UPLOAD_STUDENT_INFO, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_UPLOAD_STAFF_INFO, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_UPLOAD_SCHEDULE_INFO, new PreferenceInfo { Value = "" });
            res.Add(Preference.VIDEO_UPLOAD_IMAGE_INFO, new PreferenceInfo { Value = "" });
            res.Add(Preference.ADIMN_VIDEO_TOUR, new PreferenceInfo {Value = "https://vimeo.com/42082407"});
            res.Add(Preference.TEACHER_VIDEO_TOUR, new PreferenceInfo {Value = "https://vimeo.com/42082407"});
            res.Add(Preference.STUDENT_VIDEO_TOUR, new PreferenceInfo {Value = "https://vimeo.com/42082407"});
            res.Add(Preference.VIDEO_INVITE, new PreferenceInfo { Value = "" });
            res.Add(Preference.MAIL_NOTIFICATIO_FREQUENCY, new PreferenceInfo { Value = "12" });
            res.Add(Preference.MARKING_PERIOD_END_REMIDER, new PreferenceInfo { Value = "7" });
            res.Add(Preference.EMAIL_DEFAULT_SUBJECT, new PreferenceInfo { Value = "Chalkable Demo - App store for school" });
            res.Add(Preference.FACEBOOK_APLICATION_ID, new PreferenceInfo { Value = "379449568773831" });
            res.Add(Preference.GOOGLE_APPLICATION_ID, new PreferenceInfo {Value = ""});
            res.Add(Preference.LINKEDIN_APPLICATION_ID, new PreferenceInfo {Value = ""});
            res.Add(Preference.TWITTER_APPLICATION_ID, new PreferenceInfo { Value = "XTmvCzItf0wQFWzZu5jA" });
            res.Add(Preference.CLEVER_API_KEY, new PreferenceInfo { Value = "DEMO_KEY" });
            res.Add(Preference.NOTIFICATIONS_POST_TIME, new PreferenceInfo { Value = "17" });
            
            res.Add(Preference.CONTACTS_FOR_PURCHASE_ORDER, new PreferenceInfo { Value = "Chalkable\n304 Hadson st.\n 6th Floor\n New York, NY 11230\n billing@chalkable.com" });
            res.Add(Preference.TERMS_FOR_PURCHASE_ORDER, new PreferenceInfo { Value = "Net 30 days" });
            res.Add(Preference.PHONE_DEFAULT_PREFIX, new PreferenceInfo { Value = "+1" });
            res.Add(Preference.TWILIO_ACCOUNT_SID, new PreferenceInfo { Value = "AC8c76afeb9e4d15270076f6ec718e422a" });
            res.Add(Preference.TWILIO_OAUTH_TOKEN, new PreferenceInfo { Value = "472e1237551255926a9f69b8a26ec050" });
            res.Add(Preference.TWILIO_FROM_PHONE_NUMBER, new PreferenceInfo {Value = "+16464900500"});
            res.Add(Preference.PASSWORD_RESET_FREQUENCY_MINUTES, new PreferenceInfo {Value = "20"});

            res.Add(Preference.SCHOOL_REGISTRATION_TO_SYSADMIN_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-school id;\n{1}- school name;\n{2},{3},{4} - admin id, username, email",
                Value = "New School was just registered. ID: {0}, Name: {1}, Administrator: {2} {3} {4}"
            });
            res.Add(Preference.EMAIL_CHANGE_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-first name;\n{1}- new email",
                Value = "Hi {0},<br/><br/>Your email address has been updated for Chalkable. {1} will be your username for signing in at <a href=\"http://chalkable.com\">http://chalkable.com</a>."
            });
            res.Add(Preference.SCHOOL_REGISTRATION_BEGIN_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-school id;\n{1}- school name;\n{2},{3},{4} - admin id, role, email",
                Value = "New School was started registering. ID: {0}, Name: {1}, Administrator: {2}<br/> Role Selected: {3}<br/> Email {4}<br/>"
            });
            res.Add(Preference.SCHOOL_SETUP_FIRST_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-sign up person id;\n {1} - sing up person name;\n {2} - role name;\n {3} - email;/n" +
                       "{4} - school name;\n {5}-ip address;\n {6}-url;\n {7} operating system;\n " +
                       "{8}- browser name,{9}- browser version;\n {10}X{11}-screen resolution",

                Value = "New School was started registering.<br/><br/> ID: {0}<br/> Name: {1}<br/> Role Selected: {2}<br/> Email: " +
                                      "{3}<br/> School Name: {4}<br/> IP Address: {5}<br/>{6} Operating System {7}<br/> " +
                                      "Browser {8} {9}<br/> Screen Resolution {10}X{11}"
            });
            res.Add(Preference.SCHOOL_SETUP_SECOD_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-school name;\n {1}-sis user name;\n {2} - sis url;\n {3}-system type",
                Value = "Entered SIS data.<br/><br/> School Name: {0}<br/> SIS User Name: {1}<br/> SIS URL: {2}<br/> System Type: {3}"
            });
            res.Add(Preference.SCHOOL_SETUP_DATA_UPLOADED, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-school id;\n{1}- school name;\n{2},{3},{4} - admin id,username,email",
                Value = "New School just uploaded setup data. ID: {0}, Name: {1}, Administrator: {2} {3} {4}"
            });
            res.Add(Preference.RESETTED_PASSWORD_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0},{1} - person first name, last name;\n {2}-confirmation Uri",
                Value = "Hi  {0} {1}.<br/> Looks like you forgot your password and " +
                        "asked for a reset password link. No worries, it happens. <br/>  " +
                        "Click here to enter a new password {2}.<br/><br/> Chalkable"
            });
            res.Add(Preference.PURCHASE_ORDER_CONFIGURATION_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "",
                Value = "Hello . Your purchase order was accepted.  Please open attachment for details"
            });
            res.Add(Preference.STUDENT_INVITE_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-student first name;\n {1}-sender message;\n{2}-confirmation url;\n{3}-school name",
                Value = "Hi  {0}, <br/><br/>{3} is now using Chalkable! <br/>{1}" +
                        "You can see all of your assignments and grades in one place. " +
                        "<br/>Click here to check it out - {2} "
            });
            res.Add(Preference.TEACHER_INVITE_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0}-teacher first name;\n {1}-sender message;\n{2}-confirmation url;\n{3}-school name",
                Value = "Hi  {0}, <br/><br/>{3} is now using Chalkable! <br/><br/> {1}. <br/><br/> Getting started takes less than 2 minutes. <br/>Click here - {2}"
            });
            res.Add(Preference.ACTION_LINK_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - full person name;\n {1}-link",
                Value = "Hi  {0}, here is link for school setup continuation. {1}"
            });

            //application email body
            res.Add(Preference.APPLICATION_SUBMITTED_EMAIL_SUBJECT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Chalkable - Your application state was changed"
            });
            res.Add(Preference.APPLICATION_SUBMITTED_EMAIL_BODY, new PreferenceInfo
                {
                    Category = PreferenceCategoryEnum.EmailText,
                    Type = PreferenceTypeEnum.LongText,
                    Hint = "{0} - application name;\n",
                    Value = "The {0} app was submitted and is awaiting aproval.",
                });
            res.Add(Preference.APPLICATION_APPROVED_EMAIL_SUBJECT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Chalkable - Your application state was changed"
            });
            
            res.Add(Preference.APPLICATION_APPROVED_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - application name;\n {1} - developer name;\n {2} - developer link page",
                Value = "Hi {1}, <br> Your app has been aprooved and is ready to go live. All you have to do is click Go live. {2}. <br/> Thank you, <br/> The Chalkable Team",
            });
            res.Add(Preference.APPLICATION_REJECTED_EMAIL_SUBJECT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Chalkable - Your application state was changed"
            });

            res.Add(Preference.APPLICATION_REJECTED_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - application name;\n {1} - developer name",
                Value = "Hi {1}, <br/> Your app {0} has been rejected. <br/> Please contact developers@chalkable.com for more information on the rejection and how to resubmit your app.",
            });

            res.Add(Preference.APPLICATION_GO_LIVE_EMAIL_SUBJECT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Chalkable - Your application state was changed"
            });
           
            res.Add(Preference.APPLICATION_GO_LIVE_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - application name;\n {1} - developer name",
                Value = "The {0} app is now live in the App Store. {1} clicked live.",
            });

            res.Add(Preference.APPLICATION_UNINSTALL_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - application name;\n {1} - developer name",
                Value = "{1} removed {0} from the App Store.",
            });
           
            res.Add(Preference.APPLICATION_DELETED_BY_SYSADMIN_EMAIL_SUBJECT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Chalkable - Your application state was changed"
            });
           
            res.Add(Preference.APPLICATION_DELETED_BY_SYSADMIN_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Hint = "{0} - application name;\n {1} - developer name",
                Value = "You {0} app was deleted from chalkable by sysadmin.",
            });

            res.Add(Preference.ATTENDANCE_SYNC_FAIL_EMAIL_BODY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.EmailText,
                Type = PreferenceTypeEnum.LongText,
                Value = "Syncing with SIS database is completed. See atteachment for details",
            });


            //api descriptions

            res.Add(Preference.API_DESCR_ANNOUNCEMENT_READ, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns announcement data"
            });
           
            res.Add(Preference.API_DESCR_FEED_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns list of user items"
            });

            res.Add(Preference.API_DESCR_SCHOOL_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns list of accessible schools(returns 1 school for all of these roles)"
            });

            res.Add(Preference.API_DESCR_STUDENT_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns summary data (current classes, statistics etc...) for a particular student"
            });

            res.Add(Preference.API_DESCR_STUDENT_INFO, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns student profile info(name, email, etc)"
            });

            res.Add(Preference.API_DESCR_STUDENT_GRADING_STAT, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns student grading statistics"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_STUDENT_ATTENDANCE_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns student attendance summary"
            });

            res.Add(Preference.API_DESCR_CLASS_DISCIPLINE_STUDENT_DISCIPLINE_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "Returns student discipline summary"
            });

            res.Add(Preference.API_DESCR_SEARCH_SEARCH, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.LongText,
                Value = "Returns search result as list of items of next type:  " +
                "id - object id" +
                "description, bigpictureid, smallpictureid - picture ids, " +
                "searchtype(0 -schoolperson, 1 - application, 2 - announcement, 3 - attachment, 4 - class)"
            });

            res.Add(Preference.API_DESCR_GRADING_STYLE_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns grading styles (ABC, check+-, complete/incomplete) mapping to 1-100 grades for current school"
            });

            res.Add(Preference.API_DESCR_SCHOOLYEAR_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of school years"
            });

            res.Add(Preference.API_DESCR_MARKING_PERIOD_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns marking periods in year"
            });
        
            res.Add(Preference.API_DESCR_GENERAL_PERIOD_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns period's schedule"
            });

            res.Add(Preference.API_DESCR_SCHEDULE_SECTION_LIST_FOR_MARKING_PERIODS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns schedule section for a set of MP"
            });

            res.Add(Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns items calendar view for a month"
            });

            res.Add(Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_WEEK, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns items calendar view for a week"
            });

            res.Add(Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_DAY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns day's schedule"
            });

            res.Add(Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_ADMIN_DAY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns day's schedule(school wide)"
            });

            res.Add(Preference.API_DESCR_GET_APP_ANNOUNCEMENT_APPLICATION, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "gets announcement application"
            });

            res.Add(Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns discipline records for particular student for given date"
            });


            res.Add(Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.LongText,
                Value = "sets attendance for particular student and period(1 - N/A, 2 - Present, 4 - Excused, 8 - Absent, Late - 16)"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_REASON_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "gets attendance reasons list"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns attendance summary"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_SEATING_CHART, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns attendance seating chart"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_LIST_CLASS_ATTENDANCE, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns student attendance data for a particular date/class"
            });

            res.Add(Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "sets students attendance for whole class"
            });
            

            res.Add(Preference.API_DESCR_CLASS_DISCIPLINE_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of discipline records for some date"
            });

            res.Add(Preference.API_DESCR_CLASS_DISCIPLINE_LIST_POSSIBLE_STUDENTS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns discipline records for students with name matching \"query\""
            });

            res.Add(Preference.API_DESCR_DISCIPLINE_TYPE_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of possible discipline records in particular school"
            });

            res.Add(Preference.API_DESCR_PRIVATE_MESSAGES_LIST_POSSIBLE_RECIPIENTS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of possible recipients with name that contains query parameter"
            });


            res.Add(Preference.API_DESCR_PRIVATE_MESSAGES_SEND, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "sends message to somebody"
            });

            res.Add(Preference.API_DESCR_GRADE_LIST_ITEMS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of grades received for particular announcement (assignment)"
            });

            res.Add(Preference.API_DESCR_GRADE_UPDATE_ITEM, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "updates particular student's grade in item"
            });

            res.Add(Preference.API_DESCR_SET_AUTO_GRADE, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "allows application to set student's grade"
            });

            res.Add(Preference.API_DESCR_GRADING_CLASS_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of items for particular class"
            });

            res.Add(Preference.API_DESCR_GRADING_CLASS_SUMMARY_GRID, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of items for particular class"
            });

            res.Add(Preference.API_DESCR_TEACHER_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns general summary about teacher"
            });

            res.Add(Preference.API_DESCR_ADMIN_SUMMARY, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns general summary about administrator"
            });

            res.Add(Preference.API_DESCR_CLASS_GENERAL_PERIOD_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns class schedule for teacher"
            });

            res.Add(Preference.API_DESCR_CLASS_LIST, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = " returns class list for user"                                            
            });

            res.Add(Preference.API_DESCR_ADMIN_GET_USERS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of users in a school"
            });

            res.Add(Preference.API_DESCR_STUDENT_GET_STUDENTS, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns list of students"
            });

            res.Add(Preference.API_DESCR_USER_ME, new PreferenceInfo
            {
                Category = PreferenceCategoryEnum.ControllerDescriptions,
                Type = PreferenceTypeEnum.ShortText,
                Value = "returns currently logged in user"
            });

            return res;
        }

    }
    public class PreferenceInfo
    {
        public string Value { get; set; }
        public PreferenceCategoryEnum Category { get; set; }
        public PreferenceTypeEnum Type { get; set; }
        public string Hint { get; set; }

        public PreferenceInfo()
        {
            Category = PreferenceCategoryEnum.Common;
            Type = PreferenceTypeEnum.ShortText;
        }
    }
}
