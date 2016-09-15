using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class ReportCardsInputModel
    {
        public Guid CustomReportTemplateId { get; set; }
        public IntList GroupIds { get; set; }
        public IntList StudentIds { get; set; }
        public string Tile { get; set; }
        public IntList GradingPeriodIds { get; set; }
        public IntList AttendanceReasonIds { get; set; }
        public bool IncludeGradedStandardsOnly { get; set; }
        public int Logo { get; set; }
        public ReportCardsLogoType LogoType
        {
            get { return (ReportCardsLogoType) Logo; }
            set { Logo = (int)value; }
        }

        public int Recipient { get; set; }
        public ReportCardsRecipientType RecipientType
        {
            get { return (ReportCardsRecipientType)Recipient; }
            set { Recipient = (int)value; }
        }

        public int OrderBy { get; set; }
        public ReportCardsOrderBy OrderByType
        {
            get { return (ReportCardsOrderBy)OrderBy; }
            set { OrderBy = (int)value; }
        }
        public int IdToPrint { get; set; }

        public IntList IncludeOptions { get; set; }
        private bool HasOption(ReportCardsAddionalOptions option)
        {
            return IncludeOptions != null && IncludeOptions.Contains((int) option);
        }
        public bool IncludeAnnouncements => HasOption(ReportCardsAddionalOptions.Announcement);
        public bool IncludeAttendance => HasOption(ReportCardsAddionalOptions.Attendance);
        public bool IncludeGradingPeriodNotes => HasOption(ReportCardsAddionalOptions.GradingPeriodNotes);
        public bool IncludeGradingScaleTraditional => HasOption(ReportCardsAddionalOptions.GradingScaleTraditional);
        public bool IncludeGradingScaleStandards => HasOption(ReportCardsAddionalOptions.GragingScaleStandards);
        public bool IncludeMeritDemerit => HasOption(ReportCardsAddionalOptions.MeritDemerit);
        public bool IncludeParentSignature => HasOption(ReportCardsAddionalOptions.ParentSignature);
        public bool IncludePromotionStatus => HasOption(ReportCardsAddionalOptions.PromotionStatus);
        public bool IncludeWithdrawnStudents => HasOption(ReportCardsAddionalOptions.WithdrawnStudents);
        public bool IncludeYearToDateInformation => HasOption(ReportCardsAddionalOptions.YearToDateInformation);
        public bool IncludeComments => HasOption(ReportCardsAddionalOptions.Comments);
        public string DefaultDataPath { get; set; }
    }

    public enum ReportCardsRecipientType
    {
        Students = 0,
        Custodians = 1,
        MailingContacts = 2
    }

    public enum ReportCardsLogoType
    {
        None = 0,
        District = 1,
        School = 2
    }

    public enum ReportCardsOrderBy
    {
        GradeLevel = 0,
        Homeroom = 1,
        PostalCode = 2,
        StudentDisplayName = 3,
        StudentId = 4
    }

    public enum ReportCardsAddionalOptions
    {
        Announcement = 0,
        Attendance = 1,
        GradingPeriodNotes = 2,
        GradingScaleTraditional = 3,
        GragingScaleStandards = 4,
        MeritDemerit = 5,
        ParentSignature = 6,
        PromotionStatus = 7,
        WithdrawnStudents = 8,
        YearToDateInformation = 9,
        Comments = 10    
    }
}
