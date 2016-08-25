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

        public int IncludeOptions { get; set; }
        public ReportCardsAddionalOptions IncludeOptionsType
        {
            get { return (ReportCardsAddionalOptions)IncludeOptions; }
            set { IncludeOptions = (int)value; }
        }
        public int IdToPrint { get; set; }
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
        Distric = 1,
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

    [Flags]
    public enum ReportCardsAddionalOptions
    {
        Announcement = 0,
        Attendance = 1,
        GradginPeriodNotes = 2,
        GradingScaleTraditional = 4,
        GraginScaleStandards = 8,
        MeritDemerit = 16,
        PerentSignature = 32,
        PromotionStatus = 64,
        WithdrawnStudents = 128,
        YearToDateInformation = 256    
    }
}
