﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model.Reports.ReportCards;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class CustomReportCardsExportModel
    {
        public string LogoHref { get; set; }
        public int AcadYear { get; set; }
        public string AcadSessionName { get; set; }
        public SchoolReportCardsExportModel School { get; set; }
        public IList<TraditionalGradingScaleExportModel> TraditionalGradingScale { get; set; } 
        public IList<StandardsGradingScaleExportModel> StandardsGradingScale { get; set; }
        public bool IdToPrint { get; set; }
        public StudentReportCardsExportModel Student { get; set; }

        public static CustomReportCardsExportModel Create(ReportCard reportCard, Student studentData, ReportCardAddressData recipient, string logoRef)
        {
            return new CustomReportCardsExportModel
            {
                AcadYear = reportCard.AcadYear,
                AcadSessionName = reportCard.AcadSessionName,
                School = new SchoolReportCardsExportModel
                {
                    Address1 = reportCard.School.Address1,
                    Address2 = reportCard.School.Address2,
                    City = reportCard.School.City,
                    Name = reportCard.School.Name,
                    Phone = reportCard.School.Phone,
                    State = reportCard.School.State,
                    Zip = reportCard.School.Zip
                },
                Student = StudentReportCardsExportModel.Create(studentData, recipient),
                TraditionalGradingScale = new List<TraditionalGradingScaleExportModel>(),
                StandardsGradingScale = new List<StandardsGradingScaleExportModel>()
            };
        }
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

        public static StudentReportCardsExportModel Create(Student studentData, ReportCardAddressData recipient)
        {
            return new StudentReportCardsExportModel
            {
                Name = studentData.Name,
                AltStudentNumber = studentData.AltStudentNumber,
                GradeLevel = studentData.GradeLevel,
                StudentId = studentData.StudentId,
                Demerits = studentData.Demerits,
                Merits = studentData.Merits,
                Recipient = RecipientsReportCardsExportModel.Create(recipient),
                Classes = ClassReportCardsExportModel.Create(studentData.Sections),
                
            };
        }
    }

    public class ClassReportCardsExportModel
    {
        public string Name { get; set; }
        public string ClassNumber { get; set; }
        public decimal? TimesTardy { get; set; }
        public string Teacher { get; set; }
        public IList<GradingGridExportModel> GradingPeriods { get; set; }

        public static IList<ClassReportCardsExportModel> Create(IEnumerable<ReportCardSectionData> sections)
        {
            return sections.Select(sectionData => new ClassReportCardsExportModel
            {
                ClassNumber = sectionData.SectionNumber,
                Name = sectionData.Name,
                Teacher = sectionData.Teacher,
                TimesTardy = sectionData.TimesTardy,
                GradingPeriods = GradingGridExportModel.Create(sectionData.GradingPeriods)
            }).ToList();
        }
    }

    public class GradingGridExportModel
    {
        public int GradingPeriodId { get; set; }
        public string GradingPeriodName { get; set; }
        public string Note { get; set; }
        public IList<GradedItemExportModel> GradedItems { get; set; }
        public IList<StandardGradeExportModel> Standards { get; set; }

        public static IList<GradingGridExportModel> Create(IEnumerable<StudentGradingPeriod> studentGradingPeriods)
        {
            return studentGradingPeriods.Select(x => new GradingGridExportModel
            {
                GradingPeriodId = x.GradingPeriodId,
                GradingPeriodName = x.GradingPeriodName,
                Note = x.Note,
                GradedItems = GradedItemExportModel.Create(x.GradedItems),
                Standards = x.Standards.Select(s=> new StandardGradeExportModel
                {
                    Description = s.Description,
                    Name = s.Name,
                    Grade = s.Grade,
                    Comment = s.Comments.FirstOrDefault()?.Comment
                }).ToList()
            }).ToList();
        } 
    }

    public class GradedItemExportModel
    {
        public int AlphaGradeId { get; set; }
        public string AlphaGrade { get; set; }
        public string GradedItemName { get; set; }
        public bool IsExempt { get; set; }
        public decimal? NumericGrade { get; set; }
        public IList<CommentReportCardsExportModel> Comments { get; set; }

        public static IList<GradedItemExportModel> Create(IEnumerable<StudentGradedItem> studentGradedItems)
        {
            return studentGradedItems.Select(x => new GradedItemExportModel
            {
                AlphaGrade = x.AlphaGrade,
                NumericGrade = x.NumericGrade,
                Comments = CommentReportCardsExportModel.Create(x.Comments),
                IsExempt = x.IsExempt,
                AlphaGradeId = x.AlphaGradeId,
                GradedItemName = x.GradedItemName
            }).ToList();
        }
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

        public static IList<CommentReportCardsExportModel> Create(IEnumerable<StudentGradingComment> comments)
        {
            return comments.Select(x => new CommentReportCardsExportModel
            {
                Comment = x.Comment,
                CommentCode = x.CommentCode,
                CommentHeaderDescription = x.CommentHeaderDescription,
                CommentHeaderName = x.CommentHeaderName
            }).ToList();
        }
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

        public static AttendanceSummaryExportModel Create(ReportCardAttendanceData attendance)
        {
            return new AttendanceSummaryExportModel
            {
                GradingPeriodId = attendance.GradingPeriodId,
                UnexcusedAbsences = attendance.UnexcusedAbsences,
                UnexcusedTardies = attendance.UnexcusedTardies,
                ExcusedAbsences = attendance.ExcusedAbsences,
                ExcusedTardies = attendance.ExcusedTardies,
                Enroller = attendance.DaysEnrolled
            };
        }

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

        public static RecipientsReportCardsExportModel Create(ReportCardAddressData recipient)
        {
            return new RecipientsReportCardsExportModel
            {
                AddressLine1 = recipient.AddressLine1,
                AddressLine2 = recipient.AddressLine2,
                State = recipient.State,
                City = recipient.City,
                PostalCode = recipient.PostalCode,
                RecipientName = recipient.RecipientName
            };
        }
    }
}
