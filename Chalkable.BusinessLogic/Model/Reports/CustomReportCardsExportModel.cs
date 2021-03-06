﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Chalkable.StiConnector.Connectors.Model.Reports.ReportCards;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class CustomReportCardsExportModel
    {
        public string ReportDate { get; set; }
        public string CopyRight { get; set; }
        public string LogoHref { get; set; }
        public int AcadYear { get; set; }
        public string AcadSessionName { get; set; }
        public SchoolReportCardsExportModel School { get; set; }
        public IList<GradingScaleExportModel<TraditionalGradingScaleRangeExportModel>> TraditionalGradingScales { get; set; } 
        public IList<GradingScaleExportModel<StandardsGradingScaleRangeExportModel>> StandardsGradingScales { get; set; }
        public bool IdToPrint { get; set; }
        public StudentReportCardsExportModel Student { get; set; }
        public bool IncludeSignature { get; set; }
        public ReportCardsRecipientType RecipientType { get; set; }
        public bool IncludeMeritDemerit { get; set; }

        public static CustomReportCardsExportModel Create(ReportCard reportCard, Student studentData, ReportCardAddressData recipient, string logoRef, DateTime reportDate
            , ReportCardsInputModel inputModel)
        {
            var res = new CustomReportCardsExportModel
            {
                AcadYear = reportCard.AcadYear,
                LogoHref = logoRef,
                AcadSessionName = reportCard.AcadSessionName,
                ReportDate = reportDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                CopyRight = $"Copyright (c) {DateTime.Now.Year} Chalkable",
                School = new SchoolReportCardsExportModel
                {
                    Address1 = reportCard.School.Address1,
                    Address2 = reportCard.School.Address2,
                    City = reportCard.School.City,
                    Name = reportCard.School.Name,
                    Phone = FormatSchoolPhone(reportCard.School.Phone),
                    State = reportCard.School.State,
                    Zip = reportCard.School.Zip
                },
                Student = StudentReportCardsExportModel.Create(reportCard.GradingPeriod, studentData, recipient, inputModel.IncludeGradedStandardsOnly, inputModel.IncludeComments),
                IncludeSignature = inputModel.IncludeParentSignature,
                RecipientType = inputModel.RecipientType,
                IncludeMeritDemerit = inputModel.IncludeMeritDemerit,
                TraditionalGradingScales = new List<GradingScaleExportModel<TraditionalGradingScaleRangeExportModel>>(),
                StandardsGradingScales = new List<GradingScaleExportModel<StandardsGradingScaleRangeExportModel>>(),
                IdToPrint = inputModel.IdToPrint != 0 // 0 - this is NONE option on ui.
            };
            if (inputModel.IncludeGradingScaleTraditional)
                res.TraditionalGradingScales = GradingScaleExportModel<TraditionalGradingScaleRangeExportModel>.Create(reportCard.GradingScales,
                        studentData.Sections, s => s.GradingScaleId, TraditionalGradingScaleRangeExportModel.Create);
            if(inputModel.IncludeGradingScaleStandards)
                res.StandardsGradingScales = GradingScaleExportModel<StandardsGradingScaleRangeExportModel>.Create(reportCard.GradingScales,
                        studentData.Sections, s => s.StandardGradingScaleId, StandardsGradingScaleRangeExportModel.Create);
            return res;
        }

        private static string FormatSchoolPhone(string tel)
        {
            if (string.IsNullOrWhiteSpace(tel) || tel.Length != 10)
                return tel;

            StringBuilder builder = new StringBuilder();
            builder.Append(tel.Substring(0, 3)).Append(".")
                .Append(tel.Substring(3, 3)).Append(".")
                .Append(tel.Substring(6, 4));

            return builder.ToString();
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

    public class GradingScaleExportModel<TGradingScaleRange>
    {
        public IList<string> Classes { get; set; }
        public IList<TGradingScaleRange> Ranges { get; set; }

        public static IList<GradingScaleExportModel<TGradingScaleRange>> Create(IList<GradingScale> gradingScales, IEnumerable<ReportCardSectionData> sections
            , Func<ReportCardSectionData, int?> getGradingScaleId  
            , Func<GradingScaleRange, GradingScale, TGradingScaleRange> gradingScaleRangeCreator)
        {
            sections = sections.Where(x => getGradingScaleId(x).HasValue).ToList();
            if (!sections.Any()) return new List<GradingScaleExportModel<TGradingScaleRange>>();

            var gsClasses = sections.GroupBy(x => getGradingScaleId(x).Value).ToDictionary(x => x.Key, x => x.Select(s => s.Name).ToList());

            var res = new List<GradingScaleExportModel<TGradingScaleRange>>();
            foreach (var kv in gsClasses)
            {
                var gradingScale = gradingScales.FirstOrDefault(x => x.Id == kv.Key);
                if (gradingScale?.Ranges == null) continue;
                res.Add(new GradingScaleExportModel<TGradingScaleRange>
                {
                    Classes = kv.Value,
                    Ranges = gradingScale.Ranges.OrderByDescending(x => x.HighValue)
                                     .Select(x => gradingScaleRangeCreator(x, gradingScale)).ToList()
                });
            }
            return res;
        }
    }

    public class TraditionalGradingScaleRangeExportModel
    {
        public string Name { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public static TraditionalGradingScaleRangeExportModel Create(GradingScaleRange range, GradingScale gradingScale)
        {
            return new TraditionalGradingScaleRangeExportModel
            {
                MaxValue = range.HighValue,
                MinValue = range.LowValue,
                Name = range.AlphaGrade
            };
        }
    }
    public class StandardsGradingScaleRangeExportModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public static StandardsGradingScaleRangeExportModel Create(GradingScaleRange range, GradingScale gradingScale)
        {
            return new StandardsGradingScaleRangeExportModel
            {
                Name = range.AlphaGrade,
                Description = gradingScale.Description + " " + range.AlphaGrade
            };
        }
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

        public static StudentReportCardsExportModel Create(GradingPeriod gradingPeriod, Student studentData, ReportCardAddressData recipient, bool onlyGradedStandard, bool includeNote)
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
                Classes = ClassReportCardsExportModel.Create(studentData.Sections, onlyGradedStandard, includeNote),
                Attendances = AttendanceSummaryExportModel.Create(studentData.Attendance),
                GradingPeriod = new GradingPeriodExportModel
                {
                    Announcement = gradingPeriod.Announcement,
                    StartDate = gradingPeriod.StartDate,
                    EndDate = gradingPeriod.EndDate,
                    Name = gradingPeriod.Name
                },
                HomeRoomTeacher = studentData.HomeroomTeacher,
                Promoted = studentData.Promoted
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

        public static IList<ClassReportCardsExportModel> Create(IEnumerable<ReportCardSectionData> sections, bool onlyGradedStandard, bool includeNote)
        {
            return sections.Select(sectionData => new ClassReportCardsExportModel
            {
                ClassNumber = sectionData.SectionNumber,
                Name = sectionData.Name,
                Teacher = sectionData.Teacher,
                TimesTardy = sectionData.TimesTardy,
                GradingPeriods = GradingGridExportModel.Create(sectionData.GradingPeriods, onlyGradedStandard, includeNote)
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

        public static IList<GradingGridExportModel> Create(IEnumerable<StudentGradingPeriod> studentGradingPeriods, bool onlyGradedStandard, bool includeNote)
        {
            return studentGradingPeriods.Select(x => new GradingGridExportModel
            {
                GradingPeriodId = x.GradingPeriodId,
                GradingPeriodName = x.GradingPeriodName,
                Note = includeNote ? x.Note : null,
                GradedItems = x.GradedItems != null ? GradedItemExportModel.Create(x.GradedItems) : new List<GradedItemExportModel>(),
                Standards = x.Standards?
                            .Where(s=> !onlyGradedStandard || !string.IsNullOrWhiteSpace(s.Grade))
                            .Select(s=> new StandardGradeExportModel
                            {
                                Description = s.Description,
                                Name = s.Name,
                                Grade = s.Grade,
                                Comment = s.Comments?.FirstOrDefault()?.Comment
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
                NumericGrade = x.NumericGrade == null ? (decimal?)null : decimal.Round(x.NumericGrade.Value),
                Comments = x.Comments != null 
                           ? CommentReportCardsExportModel.Create(x.Comments) 
                           : new List<CommentReportCardsExportModel>(),
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
        public decimal Enrolled { get; set; }
        public decimal Present { get; set; }
        public decimal Absences { get; set; }
        public decimal Tardies { get; set; }
        public decimal ExcusedAbsences { get; set; }
        public decimal ExcusedTardies { get; set; }
        public int GradingPeriodId { get; set; }
        public string GradingPeriodName { get; set; }
        public decimal UnexcusedAbsences { get; set; }
        public decimal UnexcusedTardies { get; set; }

        public static IList<AttendanceSummaryExportModel> Create(IEnumerable<ReportCardAttendanceData> attendances)
        {
            return attendances.Select(attendance => Create(attendance)).ToList();
        }

        public static AttendanceSummaryExportModel Create(ReportCardAttendanceData attendance)
        {
            var res = new AttendanceSummaryExportModel
            {
                GradingPeriodId = attendance.GradingPeriodId,
                UnexcusedAbsences = attendance.UnexcusedAbsences,
                ExcusedAbsences = attendance.ExcusedAbsences,
                ExcusedTardies = attendance.ExcusedTardies,
                UnexcusedTardies = attendance.UnexcusedTardies,
                Enrolled = attendance.DaysEnrolled,
                GradingPeriodName = attendance.GradingPeriodName,
                Absences = attendance.UnexcusedAbsences + attendance.ExcusedAbsences,
                Tardies = attendance.ExcusedTardies + attendance.UnexcusedTardies
            };
            res.Present = res.Enrolled - (res.Absences + res.Tardies);

            return res;
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
