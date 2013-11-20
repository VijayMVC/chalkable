using System;
using System.Text;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{

    public enum PreferenceCategoryEnum
    {
        Common = 0,
        EmailText = 1,
        ControllerDescriptions = 2
    }
    public enum PreferenceTypeEnum
    {
        ShortText = 0,
        LongText = 1,
    }

    public class Preference
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsPublic { get; set; }
        public PreferenceCategoryEnum Category { get; set; }
        public PreferenceTypeEnum Type { get; set; }
        public string Hint { get; set; }


        //Constants
        public const string DEMO_DISTRICT_ID = "demodistrictid";
        public const string DEMO_SCHOOL_ADMIN_GRADE = "demoschooladmingrade";
        public const string DEMO_SCHOOL_ADMIN_EDIT = "demoschooladminedit";
        public const string DEMO_SCHOOL_ADMIN_VIEW = "demoschooladminview";
        public const string DEMO_SCHOOL_TEACHER = "demoschoolteacher";
        public const string DEMO_SCHOOL_STUDENT = "demoschoolstudent";
        public const string DEMO_SCHOOL_PROCESSING_PARAMETERS = "demoshoolprocessingparameters";
        public const string DEMO_USER_PASSWORD = "demouserpassword";

        public const string SERVER_PORT = "serverport";

        public const string NOTIFICATIONS_POST_TIME = "notificationsposttime";
        public const string SERVER_NAME = "servername";
        public const string SYSTEM_EMAIL = "systememail";
        public const string NOTIFICATION_SYSTEM_EMAIL = "notificationsystememail";
        public const string NOTIFICATION_EMAIL_PASSWORD = "notificationemailpassword";
        public const string FORGOTPASSWORD_SYSTEM_EMAIL = "forgotpaswordsystememail";
        public const string FORGOTPASWORD_EMAIL_PASSWORD = "forgotpaswordemailpassword";
        public const string SYSTEM_PASSWORD = "systempassword";
        public const string SYSTEM_NAME = "systemname";

        public const string CROCODOC_TOKEN = "crocodoctoken";
        public const string CROCODOC_API_URL = "crocodocapiurl";
        public const string CROCODOC_URL = "crocodocurl";
        public const string APPLICATION_URL = "applicationurl";

        public const string VIDEO_GETING_INFO_READY = "videogetinginfoready";
        public const string VIDEO_GETING_INFO_CHALKABLE = "videogetinginfoschalkable";
        public const string VIDEO_UPLOAD_STUDENT_INFO = "videouploadstudentinfo";
        public const string VIDEO_UPLOAD_TEACHER_INFO = "videouploadteacherinfo";
        public const string VIDEO_UPLOAD_STAFF_INFO = "videouploadstaffinfo";
        public const string VIDEO_UPLOAD_SCHEDULE_INFO = "videouploadscheduleinfo";
        public const string VIDEO_UPLOAD_IMAGE_INFO = "videouploadimageinfo";
        public const string VIDEO_INVITE = "videoinvite";

        public const string ADIMN_VIDEO_TOUR = "adminvideotour";
        public const string TEACHER_VIDEO_TOUR = "teachervideotour";
        public const string STUDENT_VIDEO_TOUR = "studentvideotour";

        public const string MAIL_NOTIFICATIO_FREQUENCY = "mailnotificationfrequency";
        public const string MARKING_PERIOD_END_REMIDER = "markingperiodendremider";

        public const string EMAIL_DEFAULT_SUBJECT = "emaildefaultsubject";

        public const string FACEBOOK_APLICATION_ID = "facebookaplicationid";
        public const string GOOGLE_APPLICATION_ID = "googleapplicationid";
        public const string LINKEDIN_APPLICATION_ID = "linkedinapplicationid";
        public const string TWITTER_APPLICATION_ID = "twitterapplicationid";

        public const string CONTACTS_FOR_PURCHASE_ORDER = "contacts for PO";
        public const string TERMS_FOR_PURCHASE_ORDER = "terms for PO";

        public const string CLEVER_API_KEY = "cleverapikey";

        public const string PHONE_DEFAULT_PREFIX = "phonedefaultprefix";
        public const string TWILIO_ACCOUNT_SID = "twilioaccountsid";
        public const string TWILIO_OAUTH_TOKEN = "twiliooauthtoken";
        public const string TWILIO_FROM_PHONE_NUMBER = "twiliofromphonenumber";

        public const string PASSWORD_RESET_FREQUENCY_MINUTES = "passwordresetfrequencyminutes";

        public const string SCHOOL_SETUP_FIRST_EMAIL_BODY = "schoolsetupfirstemailbody";
        public const string SCHOOL_SETUP_SECOD_EMAIL_BODY = "schoolsetupsecondemailbody";
        public const string SCHOOL_REGISTRATION_TO_SYSADMIN_EMAIL_BODY = "schoolregistrationto sysadmin emailbody";
        public const string SCHOOL_REGISTRATION_BEGIN_EMAIL_BODY = "schoolregistrationbeginemailbody";
        public const string SCHOOL_SETUP_DATA_UPLOADED = "schoolsetupdatauploaded";
        public const string RESETTED_PASSWORD_EMAIL_BODY = "resettedpasswordemailbody";
        public const string NOTIFICATION_EMAIL_BODY = "notificationemailbody";
        public const string MAIL_TO_FRIEND_EMAIL_BODY = "mailtofriendmailbody";
        public const string PURCHASE_ORDER_CONFIGURATION_EMAIL_BODY = "purchaseorderconfigurationemailbody";
        public const string ACTION_LINK_EMAIL_BODY = "actionlinkemailbody";
        public const string STUDENT_INVITE_EMAIL_BODY = "studentinviteemailbody";
        public const string TEACHER_INVITE_EMAIL_BODY = "teacherinviteemailbody";
        public const string EMAIL_CHANGE_EMAIL_BODY = "emailchangeemailbody";

        public const string ATTENDANCE_SYNC_FAIL_EMAIL_BODY = "attendacesyncfailemailbody";

        //applicatios
        public const string APPLICATION_SUBMITTED_EMAIL_BODY = "application_submitted_email_body";
        public const string APPLICATION_APPROVED_EMAIL_BODY = "application_approved_email_body";
        public const string APPLICATION_REJECTED_EMAIL_BODY = "application_rejected_email_body";
        public const string APPLICATION_GO_LIVE_EMAIL_BODY = "application_go_live_email_body";
        public const string APPLICATION_UNINSTALL_EMAIL_BODY = "application_uninstall_email_body";
        public const string APPLICATION_DELETED_BY_SYSADMIN_EMAIL_BODY = "application_deleted_by_sysadmin_email_body";

        public const string APPLICATION_SUBMITTED_EMAIL_SUBJECT = "application_submitted_email_subject";
        public const string APPLICATION_APPROVED_EMAIL_SUBJECT = "application_approved_email_subject";
        public const string APPLICATION_REJECTED_EMAIL_SUBJECT = "application_rejected_email_subject";
        public const string APPLICATION_GO_LIVE_EMAIL_SUBJECT = "application_go_live_email_subject";
        public const string APPLICATION_DELETED_BY_SYSADMIN_EMAIL_SUBJECT = "application_deleted_by_sysadmin_email_subject";


        //api

        public const string API_DESCR_ANNOUNCEMENT_READ = "api_desc_announcement_read";
        public const string API_DESCR_FEED_LIST = "api_desc_feed_list";
        public const string API_DESCR_SCHOOL_LIST = "api_desc_schools_list";
        public const string API_DESCR_STUDENT_SUMMARY = "api_desc_student_summary";
        public const string API_DESCR_STUDENT_INFO = "api_desc_student_info";
        public const string API_DESCR_STUDENT_GRADING_STAT = "api_desc_student_grading_stat";
        public const string API_DESCR_ATTENDANCE_STUDENT_ATTENDANCE_SUMMARY = "api_desc_student_attendance_summary";
        public const string API_DESCR_CLASS_DISCIPLINE_STUDENT_DISCIPLINE_SUMMARY = "api_desc_classdiscipline_student_disciplinesummary";
        public const string API_DESCR_SEARCH_SEARCH = "api_desc_search_search";
        public const string API_DESCR_GRADING_STYLE_LIST = "api_desc_gradingstyle_list";
        public const string API_DESCR_SCHOOLYEAR_LIST = "api_desc_schoolyear_list";
        public const string API_DESCR_MARKING_PERIOD_LIST = "api_desc_markingperiod_list";
        public const string API_DESCR_GENERAL_PERIOD_LIST = "api_desc_generalperiod_list";
        public const string API_DESCR_ANNOUNCEMENT_CALENDAR_LIST = "api_desc_announcement_calendar_list";
        public const string API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_WEEK = "api_desc_announcment_calendar_week";
        public const string API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_DAY = "api_desc_announcement_calendar_day";
        public const string API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_ADMIN_DAY = "api_desc_announcement_calendar_admin_day";
        public const string API_DESCR_GET_APP_ANNOUNCEMENT_APPLICATION = "api_desc_app_get_announcement_app";
        public const string API_DESCR_ATTENDANCE_SET_ATTENDANCE = "api_desc_attendance_set_attendance";
        public const string API_DESCR_ATTENDANCE_SUMMARY = "api_desc_attendance_summary";
        public const string API_DESCR_ATTENDANCE_LIST_CLASS_ATTENDANCE = "api_desc_attendance_list_class_attendance";
        public const string API_DESCR_CLASS_DISCIPLINE_LIST = "api_desc_class_discipline_list";
        public const string API_DESCR_CLASS_DISCIPLINE_LIST_POSSIBLE_STUDENTS = "api_desc_class_discipline_list_possible_students";
        public const string API_DESCR_DISCIPLINE_TYPE_LIST = "api_desc_discipline_type_list";
        public const string API_DESCR_PRIVATE_MESSAGES_LIST_POSSIBLE_RECIPIENTS = "api_desc_private_messages_list_possible_recipients";
        public const string API_DESCR_PRIVATE_MESSAGES_SEND = "api_desc_private_messages_send";
        public const string API_DESCR_GRADE_LIST_ITEMS = "api_desc_grade_list_items";
        public const string API_DESCR_GRADE_UPDATE_ITEM = "api_desc_grade_update_item";
        public const string API_DESCR_SET_AUTO_GRADE = "api_desc_set_auto_grade";
        public const string API_DESCR_GRADING_CLASS_SUMMARY = "api_desc_grading_class_summary";
        public const string API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE = "api_desc_class_discipline_list_student_discipline";
        public const string API_DESCR_SCHEDULE_SECTION_LIST_FOR_MARKING_PERIODS = "api_desc_schedule_section_list_for_marking_periods";
        public const string API_DESCR_CLASS_LIST = "api_desc_class_list";
        public const string API_DESCR_TEACHER_SUMMARY = "api_desc_teacher_summary";
        public const string API_DESCR_ADMIN_SUMMARY = "api_desc_admin_summary";
        public const string API_DESCR_CLASS_GENERAL_PERIOD_LIST = "api_desc_class_general_period_list";
        public const string API_DESCR_ADMIN_GET_USERS = "api_desc_admin_get_users";
        public const string API_DESCR_USER_ME = "api_desc_user_me";
        public const string API_DESCR_STUDENT_GET_STUDENTS = "api_desc_student_get_students";
        public const string API_DESCR_ATTENDANCE_REASON_LIST = "api_desc_attendance_reason_list";
        public const string API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS = "api_desc_attendance_set_attendance_for_class";
    }

}
