using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class CustomReportCardsExportModel
    {
        public int AcadYear { get; set; }
        public string AcademicSessionName { get; set; }
        public SchoolReportCardsExportModel School { get; set; }
        public IList<TraditionalGradingScaleExportModel> TraditionalGradingScale { get; set; } 
        public IList<StandardsGradingScaleExportModel> StandardsGradingScale { get; set; }
        public bool IdToPrint { get; set; }
        public StudentReportCardsExportModel Student { get; set; } 
    }

    public class SchoolReportCardsExportModel
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class TraditionalGradingScaleExportModel
    {
        public string Name { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

    }

    public class StandardsGradingScaleExportModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class StudentReportCardsExportModel
    {
        public int StudentId { get; set; }
        public string AltStudentNumber { get; set; }
        public IList<AttendanceSummaryExportModel> Attendances { get; set; } 
        public string GradeLevel { get; set; }
        public string HomeRoomTeacher { get; set; }
        public string Name { get; set; }
        public bool Promoted { get; set; }
        public IList<ClassReportCardsExportModel> Classes { get; set; } 
        public RecipientsReportCardsExportModel Recipient { get; set; }
        public GradingPeriodExportModel GradingPeriod { get; set; }
        public decimal Merits { get; set; }
        public decimal Demerits { get; set; }
        public string ReportCardsComment { get; set; }
    }

    public class ClassReportCardsExportModel
    {
        public string Name { get; set; }
        public string ClassNumber { get; set; }
        public decimal TimesTardy { get; set; }
        public string Teacher { get; set; }
    }

    public class GradingGridExportModel
    {
        public int GradingPeriodId { get; set; }
        public string GradingPeriodName { get; set; }
        public string Note { get; set; }
        public IList<GradedItemExportModel> GradedItems { get; set; }
        public IList<StandardGradeExportModel> Standards { get; set; } 
    }

    public class GradedItemExportModel
    {
        public int AlphaGradeId { get; set; }
        public string AlphaGrade { get; set; }
        public string GradedItemName { get; set; }
        public bool IsExempt { get; set; }
        public decimal NumericGrade { get; set; }
        public IList<CommentReportCardsExportModel> Comments { get; set; } 
    }

    public class StandardGradeExportModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
    }

    public class CommentReportCardsExportModel
    {
        public string Comment { get; set; }
        public string CommentCode { get; set; }
        public string CommentHeaderDescription { get; set; }
        public string CommentHeaderName { get; set; }
    }

    public class AttendanceSummaryExportModel
    {
        public decimal Enroller { get; set; }
        public decimal Present { get; set; }
        public decimal Absences { get; set; }
        public decimal Tardies { get; set; }
        public decimal ExcusedAbsences { get; set; }
        public decimal ExcusedTardies { get; set; }
        public int GradingPeriodId { get; set; }
        public string GradingPeriodName { get; set; }
        public decimal UnexcusedAbsences { get; set; }
        public decimal UnexcusedTardies { get; set; }

    }

    public class GradingPeriodExportModel
    {
        public string Announcement { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RecipientsReportCardsExportModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string RecipientName { get; set; }
        public string State { get; set; }
    }
}
