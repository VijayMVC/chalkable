namespace Chalkable.UserTracking
{
    public static class UserTrackingEvents
    {
        public const string LoggedInForTheFirstTime = "logged in for the first time";
        public const string SubmittedAppForApproval = "Submitted app for approval";
        public const string SelectedLive = "Selected \"Live\"";
        public const string CreatedApp = "Created App";
        public const string UpdatedDraft = "Updated draft";
        public const string LoggedIn = "Logged in";
        public const string AutoGradedItem = "Auto Graded Item";
        public const string AttachedDocument = "Attached Document";
        public const string AttachedApp = "Attached App";
        public const string OpenedAnnouncement = "Opened Item";
        public const string OpenedApp = "opened app";
        public const string BoughtApp = "Bought app";
        public const string LaunchedApp = "Launched app";
        public const string ResetPassword = "Reset password";
        public const string ChangedPassword = "changed password";
        public const string ChangedEmail = "changed email";
        public const string SentMessageTo = "sent a message to";
        public const string CreatedItem = "Created Item";
        public const string FinishedFirstStep = "finished first step";
        public const string FinishedSecondStep = "finished second step";
        public const string InvitedUser = "invited user";
        public const string CreatedReport = "Ran Report";
        public const string SetDiscipline = "Set Discipline";
        public const string SetFinalGrade = "Set Final Grade";
        public const string SetScore = "set score";
        public const string SetAttendance = "Set Attendance";
        public const string PostedGrades = "Posted Grades";
        public const string AttachedStandard = "Attached Standard";
        public const string AttachedAssessment = "Attached Assessment";
        public const string UsedStandardsExplorer = "used standards explorer";
        public const string CopiedLessonPlanFromGallery = "Imported Lesson Plan";
        public const string SavedLessonPlanToGallery = "Saved Lesson Plan to Gallery";

        public const string ViewedGradebook = "Viewed Gradebook";
        public const string ViewedClasses = "Viewed Classes";

        //todo: IMPLEMENT LATER
        public const string ViewedClassProfile = "Viewed Class Profile";

        //todo: ASSESSMENT SHOUD TRACK THIS
        public const string CreatedAssessment = "Created Assessment";
        public const string CreatedQuestion = "Created Question";
        public const string CreatedPassage = "Created Passage";
        public const string AssignedAssessment = "Assigned Assessment";

        public const string OpenedNotification = "Opened Notification";
        public const string SentNotification = "Sent Notification";
    }
}