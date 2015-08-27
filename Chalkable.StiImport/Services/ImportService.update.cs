using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using ClassroomOption = Chalkable.StiConnector.SyncModel.ClassroomOption;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using GradingScaleRange = Chalkable.StiConnector.SyncModel.GradingScaleRange;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;
using Person = Chalkable.StiConnector.SyncModel.Person;
using Room = Chalkable.StiConnector.SyncModel.Room;
using ScheduledTimeSlotVariation = Chalkable.StiConnector.SyncModel.ScheduledTimeSlotVariation;
using School = Chalkable.StiConnector.SyncModel.School;
using SchoolOption = Chalkable.StiConnector.SyncModel.SchoolOption;
using Staff = Chalkable.StiConnector.SyncModel.Staff;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;
using Student = Chalkable.StiConnector.SyncModel.Student;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
      
        private void UpdateSystemSettings()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.SystemSetting>().Updated == null)
                return;
            var systemSettings = context.GetSyncResult<StiConnector.SyncModel.SystemSetting>().Updated;
            var sysSettings = systemSettings.Select(x => new Data.School.Model.SystemSetting
            {
                Category = x.Category,
                Setting = x.Setting,
                Value = x.Value
            }).ToList();
            ServiceLocatorSchool.SettingsService.Edit(sysSettings);
        }
        
        private void UpdatePersonsEmails()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Updated == null)
                return;
            var personsEmails = context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Updated;
            var chlkPersonsEmails = personsEmails.Select(x => new Data.School.Model.PersonEmail
            {
                PersonRef = x.PersonID,
                Description = x.Description,
                EmailAddress = x.EmailAddress,
                IsListed = x.IsListed,
                IsPrimary = x.IsPrimary
            }).ToList();
            ServiceLocatorSchool.PersonEmailService.UpdatePersonsEmails(chlkPersonsEmails);
        }

        private void UpdateCourseTypes()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.CourseType>().Updated == null)
                return;
            var stiCourseTypes = context.GetSyncResult<StiConnector.SyncModel.CourseType>().Updated;
            var chalkableCourseTypes = stiCourseTypes.Select(courseType => new Data.School.Model.CourseType
            {
                Id = courseType.CourseTypeID,
                Name = courseType.Name,
                Description = courseType.Description,
                Code = courseType.Code,
                IsActive = courseType.IsActive,
                IsSystem = courseType.IsSystem,
                NCESCode = courseType.NCESCode,
                SIFCode = courseType.SIFCode,
                StateCode = courseType.StateCode
            }).ToList();
            ServiceLocatorSchool.CourseTypeService.Edit(chalkableCourseTypes);
        }

        private void UpdateCourses()
        {
            if (context.GetSyncResult<Course>().Updated == null)
                return;
            var courses = context.GetSyncResult<Course>().Updated.ToList();
            var classes = new List<Class>();
            var departmenPairs = PrepareChalkableDepartmentKeywords();
            foreach (var course in courses)
            {
                var closestDep = FindClosestDepartment(departmenPairs, course.ShortName.ToLower());
                classes.Add(new Class
                {
                    ChalkableDepartmentRef = closestDep != null ? closestDep.Second : (Guid?)null,
                    Description = course.FullName,
                    MinGradeLevelRef = course.MinGradeLevelID,
                    MaxGradeLevelRef = course.MaxGradeLevelID,
                    Id = course.CourseID,
                    ClassNumber = course.FullSectionNumber,
                    Name = course.ShortName,
                    SchoolYearRef = course.AcadSessionID,
                    PrimaryTeacherRef = course.PrimaryTeacherID,
                    RoomRef = course.RoomID,
                    CourseRef = course.SectionOfCourseID,
                    GradingScaleRef = course.GradingScaleID,
                    CourseTypeRef = course.CourseTypeID
                });   
            }
            ServiceLocatorSchool.ClassService.Edit(classes);
        }

        private void UpdateClassTeachers()
        {
            if (context.GetSyncResult<SectionStaff>().Updated == null)
                return;
            var teachers = context.GetSyncResult<SectionStaff>().Updated.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.EditTeachers(teachers);
        }

        private void UpdateStandardSubject()
        {
            if (context.GetSyncResult<StandardSubject>().Updated == null)
                return;
            var ss = context.GetSyncResult<StandardSubject>().Updated.Select(x => new Data.School.Model.StandardSubject
            {
                AdoptionYear = x.AdoptionYear,
                Id = x.StandardSubjectID,
                Description = x.Description,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.StandardService.EditStandardSubjects(ss);
        }

        private void UpdateStandards()
        {
            if (context.GetSyncResult<Standard>().Updated == null)
                return;
            var sts = context.GetSyncResult<Standard>().Updated.Select(x => new Data.School.Model.Standard
            {
                Description = x.Description,
                Id = x.StandardID,
                IsActive = x.IsActive,
                LowerGradeLevelRef = x.LowerGradeLevelID,
                Name = x.Name,
                ParentStandardRef = x.ParentStandardID,
                StandardSubjectRef = x.StandardSubjectID,
                UpperGradeLevelRef = x.UpperGradeLevelID,
                AcademicBenchmarkId = x.AcademicBenchmarkId
            }).ToList();
            ServiceLocatorSchool.StandardService.EditStandard(sts);
        }

        private void UpdateClassStandard()
        {
            //TODO: no way to update. delete or insert only
        }

        private void UpdateMarkingPeriodClasses()
        {
            //TODO: no way to update. delete or insert only
        }
        
        private void UpdatePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Updated == null)
                return;
            var periods = context.GetSyncResult<TimeSlot>().Updated
                .Select(x=>new Period
                {
                    Id = x.TimeSlotID,
                    Order = x.Sequence,
                    SchoolYearRef = x.AcadSessionID,
                    Name = x.Name
                })
                .ToList();
            ServiceLocatorSchool.PeriodService.Edit(periods);
        }

        private void UpdateBellSchedules()
        {
            if (context.GetSyncResult<BellSchedule>().Updated == null)
                return;
            var bellSchedules = context.GetSyncResult<BellSchedule>().Updated.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID,
                Code = x.Code,
                Description = x.Description,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                TotalMinutes = x.TotalMinutes,
                UseStartEndTime = x.UseStartEndTime
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Edit(bellSchedules);
        }

        private void UpdateScheduledTimeSlots()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ScheduledTimeSlot>().Updated == null)
                return;
            var allSts = context.GetSyncResult<StiConnector.SyncModel.ScheduledTimeSlot>().Updated
                .Select(x => new Data.School.Model.ScheduledTimeSlot
                {
                    BellScheduleRef = x.BellScheduleID,
                    Description = x.Description,
                    EndTime = x.EndTime,
                    IsDailyAttendancePeriod = x.IsDailyAttendancePeriod,
                    StartTime = x.StartTime,
                    PeriodRef = x.TimeSlotID
                })
                .ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Edit(allSts);
        }

        private void UpdateScheduledTimeSlotVariations()
        {
            if (context.GetSyncResult<ScheduledTimeSlotVariation>().Updated == null)
                return;
            var scheduledTimeSlotVariations = context.GetSyncResult<ScheduledTimeSlotVariation>().Updated
                .Select(x => new Data.School.Model.ScheduledTimeSlotVariation
                {
                    Id = x.TimeSlotVariationId,
                    BellScheduleRef = x.BellScheduleId,
                    PeriodRef = x.TimeSlotId,
                    Name = x.Name,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.EditScheduledTimeSlotVariations(scheduledTimeSlotVariations);
        }
        
        private void UpdateAttendanceReasons()
        {
            if (context.GetSyncResult<AbsenceReason>().Updated == null)
                return;
            var reasons = context.GetSyncResult<AbsenceReason>().Updated;
            var rs = reasons.Select(x => new AttendanceReason
            {
                Category = x.AbsenceCategory,
                Code = x.Code,
                Description = x.Description,
                Id = x.AbsenceReasonID,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Edit(rs);
        }

        private void UpdateAttendanceLevelReasons()
        {
            if (context.GetSyncResult<AbsenceLevelReason>().Updated == null)
                return;
            var absenceLevelReasons = context.GetSyncResult<AbsenceLevelReason>().Updated
                                                 .Select(x => new AttendanceLevelReason
                                                 {
                                                     Id = x.AbsenceLevelReasonID,
                                                     AttendanceReasonRef = x.AbsenceReasonID,
                                                     IsDefault = x.IsDefaultReason,
                                                     Level = x.AbsenceLevel
                                                 }).ToList();
            ServiceLocatorSchool.AttendanceReasonService.EditAttendanceLevelReasons(absenceLevelReasons);
        }

        private void UpdateAlphaGrades()
        {
            if (context.GetSyncResult<AlphaGrade>().Updated == null)
                return;
            var alphaGrades = context.GetSyncResult<AlphaGrade>().Updated.Select(x => new Data.School.Model.AlphaGrade
            {
                Id = x.AlphaGradeID,
                Description = x.Description,
                Name = x.Name,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.AlphaGradeService.EditAlphaGrades(alphaGrades);
        }

        private void UpdateAlternateScores()
        {
            if (context.GetSyncResult<AlternateScore>().Updated == null)
                return;
            var alternateScores = context.GetSyncResult<AlternateScore>().Updated.Select(x => new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            }).ToList();
            ServiceLocatorSchool.AlternateScoreService.EditAlternateScores(alternateScores);
        }

        private void UpdateInfractions()
        {
            if (context.GetSyncResult<Infraction>().Updated == null)
                return;
            var infractions = context.GetSyncResult<Infraction>().Updated.Select(x => new Data.School.Model.Infraction
            {
                Code = x.Code,
                Demerits = x.Demerits,
                Description = x.Description,
                Id = x.InfractionID,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                StateCode = x.StateCode,
                // The VisibleInClassroom property was added to Inow after the Infraction table was synced. 
                // At the time that this release of Chalkable goes out, some Inow apis will have this new 
                // column and some won't.  We need to take this into account and make the sync model property
                // a nullable bool.  When null, we should assume the value is true so that the infraction will
                // be displayed in the UI
                VisibleInClassroom = x.VisibleInClassroom.HasValue ? x.VisibleInClassroom.Value : true
            }).ToList();
            ServiceLocatorSchool.InfractionService.Edit(infractions);
        }

        private void UpdateGradingScales()
        {
            if (context.GetSyncResult<GradingScale>().Updated == null)
                return;
            var gs = context.GetSyncResult<GradingScale>().Updated.Select(x => new Data.School.Model.GradingScale
            {
                Id = x.GradingScaleID,
                Description = x.Description,
                HomeGradeToDisplay = x.HomeGradeToDisplay,
                Name = x.Name,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScales(gs);
        }

        private void UpdateGradingScaleRanges()
        {
            if (context.GetSyncResult<GradingScaleRange>().Updated == null)
                return;
            var gsr = context.GetSyncResult<GradingScaleRange>().Updated.Select(x => new Data.School.Model.GradingScaleRange
            {
                AlphaGradeRef = x.AlphaGradeID,
                AveragingEquivalent = x.AveragingEquivalent,
                AwardGradCredit = x.AwardGradCredit,
                GradingScaleRef = x.GradingScaleID,
                HighValue = x.HighValue,
                IsPassing = x.IsPassing,
                LowValue = x.LowValue
            }).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScaleRanges(gsr);
        }

        private void UpdateClassroomOptions()
        {
            if (context.GetSyncResult<ClassroomOption>().Updated == null)
                return;
            var cro = context.GetSyncResult<ClassroomOption>().Updated.Select(x => new Data.School.Model.ClassroomOption()
            {
                Id = x.SectionID,
                DefaultActivitySortOrder = x.DefaultActivitySortOrder,
                GroupByCategory = x.GroupByCategory,
                AveragingMethod = x.AveragingMethod,
                CategoryAveraging = x.CategoryAveraging,
                IncludeWithdrawnStudents = x.IncludeWithdrawnStudents,
                DisplayStudentAverage = x.DisplayStudentAverage,
                DisplayTotalPoints = x.DisplayTotalPoints,
                RoundDisplayedAverages = x.RoundDisplayedAverages,
                DisplayAlphaGrade = x.DisplayAlphaGrade,
                DisplayStudentNames = x.DisplayStudentNames,
                DisplayMaximumScore = x.DisplayMaximumScore,
                StandardsGradingScaleRef = x.StandardsGradingScaleID,
                StandardsCalculationMethod = x.StandardsCalculationMethod,
                StandardsCalculationRule = x.StandardsCalculationRule,
                StandardsCalculationWeightMaximumValues = x.StandardsCalculationWeightMaximumValues,
                DefaultStudentSortOrder = x.DefaultStudentSortOrder,
                SeatingChartRows = x.SeatingChartRows,
                SeatingChartColumns = x.SeatingChartColumns
            }).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Edit(cro);
        }

        private void UpdateGradingComments()
        {
            if (context.GetSyncResult<GradingComment>().Updated == null)
                return;
            var gc = context.GetSyncResult<GradingComment>().Updated.Select(x => new Data.School.Model.GradingComment
            {
                Code = x.Code,
                Comment = x.Comment,
                Id = x.GradingCommentID,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingCommentService.Edit(gc);
        }

        private void UpdateAttendanceMonthes()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.AttendanceMonth>().Updated == null)
                return;
            var attendanceMonthes = context.GetSyncResult<StiConnector.SyncModel.AttendanceMonth>().Updated;
            var res = attendanceMonthes.Select(x => new Data.School.Model.AttendanceMonth
            {
                Id = x.AttendanceMonthID,
                SchoolYearRef = x.AcadSessionID,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EndTime = x.EndTime,
                IsLockedAttendance = x.IsLockedAttendance,
                IsLockedDiscipline = x.IsLockedDiscipline
            }).ToList();
            ServiceLocatorSchool.AttendanceMonthService.Edit(res);
        }

        private void UpdateGradedItems()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.GradedItem>().Updated == null)
                return;
            var gradedItems = context.GetSyncResult<StiConnector.SyncModel.GradedItem>().Updated;
            var res = gradedItems.Select(x => new Data.School.Model.GradedItem
            {
                Id = x.GradedItemID,
                GradingPeriodRef = x.GradingPeriodID,
                AllowExemption = x.AllowExemption,
                AlphaOnly = x.AlphaOnly,
                AppearsOnReportCard = x.AppearsOnReportCard,
                AveragingRule = x.AveragingRule,
                Description = x.Description,
                Name = x.Name,
                DetGradeCredit = x.DetGradCredit,
                DetGradePoints = x.DetGradePoints
            }).ToList();
            ServiceLocatorSchool.GradedItemService.Edit(res);
        }

        private void UpdateAnnouncementAttribues()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ActivityAttribute>().Updated == null)
                return;
            var announcementAttributes = context.GetSyncResult<StiConnector.SyncModel.ActivityAttribute>().Updated.Select(x => new Data.School.Model.AnnouncementAttribute()
            {
                Id = x.ActivityAttributeID,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                StateCode = x.StateCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Edit(announcementAttributes);
        }

        private void UpdateContactRelationships()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ContactRelationship>().Updated == null)
                return;
            var contactRelationships = context.GetSyncResult<StiConnector.SyncModel.ContactRelationship>().Updated.Select(x => new Data.School.Model.ContactRelationship()
            {
                Id = x.ContactRelationshipID,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                ReceivesMailings = x.ReceivesMailings,
                IsFamilyMember = x.IsFamilyMember,
                IsCustodian = x.IsCustodian,
                IsEmergencyContact = x.IsEmergencyContact,
                StateCode = x.StateCode,
                CanPickUp = x.CanPickUp,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            }).ToList();
            ServiceLocatorSchool.ContactService.EditContactRelationship(contactRelationships);
        }

        private void UpdateStudentContacts()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.StudentContact>().Updated == null)
                return;
            var studentContacts = context.GetSyncResult<StiConnector.SyncModel.StudentContact>().Updated.Select(x => new Data.School.Model.StudentContact
            {
                StudentRef = x.StudentID,
                ContactRef = x.ContactID,
                ContactRelationshipRef = x.RelationshipID,
                Description = x.Description,
                ReceivesMailings = x.ReceivesMailings,
                IsFamilyMember = x.IsFamilyMember,
                IsCustodian = x.IsCustodian,
                IsEmergencyContact = x.IsEmergencyContact,
                CanPickUp = x.CanPickUp,
                IsResponsibleForBill = x.IsResponsibleForBill,
                ReceivesBill = x.ReceivesBill,
                StudentVisibleInHome = x.StudentVisibleInHome
            }).ToList();
            ServiceLocatorSchool.ContactService.EditStudentContact(studentContacts);
        }
    }
}
