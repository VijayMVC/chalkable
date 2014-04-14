using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
using DayType = Chalkable.StiConnector.SyncModel.DayType;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingPeriod = Chalkable.StiConnector.SyncModel.GradingPeriod;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;
using Person = Chalkable.StiConnector.SyncModel.Person;
using Room = Chalkable.StiConnector.SyncModel.Room;
using School = Chalkable.StiConnector.SyncModel.School;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessDelete()
        {
            DeleteInfractions();
            DeleteAlternateScores();
            DeleteAlphaGrades();
            DeleteAttendanceLevelReasons();
            DeleteAttendanceReasons();
            DeleteClassPersons();
            DeleteClassPeriods();
            DeletePeriods();
            DeleteClassAnnouncementTypes();
            DeleteMarkingPeriodClasses();
            DeleteClassStandard();
            DeleteStandards();
            DeleteStandardSubject();
            DeleteCourses();
            DeleteRooms();
            DeleteDays();
            DeleteDayTypes();
            DeleteGradingPeriods();
            DeleteMarkingPeriods();
            DeleteStudentSchoolYears();
            DeleteSchoolYears();
            DeleteGradeLevels();
            DeletePhones();
            DeleteSchoolPersons();
            DeletePersons();
            DeleteAddresses();
            DeleteSchools();
        }

        private void DeleteInfractions()
        {
            if (context.GetSyncResult<Infraction>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Infraction>().Deleted.Select(x => x.InfractionID).ToList();
            ServiceLocatorSchool.InfractionService.Delete(ids);
        }

        private void DeleteAlternateScores()
        {
            if (context.GetSyncResult<AlternateScore>().Deleted  == null)
                return;
            var ids = context.GetSyncResult<AlternateScore>().Deleted.Select(x => x.AlternateScoreID).ToList();
            ServiceLocatorSchool.AlternateScoreService.Delete(ids);
        }

        private void DeleteAlphaGrades()
        {
            if (context.GetSyncResult<AlphaGrade>().Deleted == null)
                return;
            var ids = context.GetSyncResult<AlphaGrade>().Deleted.Select(x => x.AlphaGradeID).ToList();
            ServiceLocatorSchool.AlphaGradeService.Delete(ids);
        }

        private void DeleteAttendanceLevelReasons()
        {
            if (context.GetSyncResult<AbsenceLevelReason>().Deleted == null)
                return;
            var ids = context.GetSyncResult<AbsenceLevelReason>().Deleted.Select(x => (int)x.AbsenceReasonID).ToList();
            ServiceLocatorSchool.AttendanceReasonService.DeleteAttendanceLevelReasons(ids);
        }

        private void DeleteAttendanceReasons()
        {
            if (context.GetSyncResult<AbsenceReason>().Deleted == null)
                return;
            var ids = context.GetSyncResult<AbsenceReason>().Deleted.Select(x => (int)x.AbsenceReasonID).ToList();
            ServiceLocatorSchool.AttendanceReasonService.Delete(ids);
        }

        private void DeleteClassPersons()
        {
            if (context.GetSyncResult<StudentScheduleTerm>().Deleted == null)
                return;
            var students = context.GetSyncResult<StudentScheduleTerm>().Deleted.ToList();
            foreach (var student in students)
            {
                ServiceLocatorSchool.ClassService.DeleteStudent(student.SectionID, student.StudentID);
            }
            
        }

        private void DeleteClassPeriods()
        {
            if (context.GetSyncResult<ScheduledSection>().Deleted == null)
                return;
            var scheduleSections = context.GetSyncResult<ScheduledSection>().Deleted.ToList();
            foreach (var scheduledSection in scheduleSections)
            {
                ServiceLocatorSchool.ClassPeriodService.Delete(scheduledSection.TimeSlotID, scheduledSection.SectionID, scheduledSection.DayTypeID);
            }
        }

        private void DeletePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Deleted == null)
                return;
            var ids = context.GetSyncResult<TimeSlot>().Deleted.Select(x => x.TimeSlotID).ToList();
            ServiceLocatorSchool.PeriodService.Delete(ids);
        }

        private void DeleteClassAnnouncementTypes()
        {
            if (context.GetSyncResult<ActivityCategory>().Deleted == null)
                return;
            var ids = context.GetSyncResult<ActivityCategory>().Deleted.Select(x => x.ActivityCategoryID).ToList();
            ServiceLocatorSchool.ClassAnnouncementTypeService.Delete(ids);
        }

        private void DeleteMarkingPeriodClasses()
        {
            if (context.GetSyncResult<SectionTerm>().Deleted == null)
                return;
            var mps = context.GetSyncResult<SectionTerm>().Deleted.Select(x => new MarkingPeriodClass
                {
                    ClassRef = x.SectionID, 
                    MarkingPeriodRef = x.TermID
                }).ToList();
            ServiceLocatorSchool.ClassService.DeleteMarkingPeriodClasses(mps);
        }
        
        private void DeleteClassStandard()
        {
            if (context.GetSyncResult<CourseStandard>().Deleted == null)
                return;
            var classStandards = context.GetSyncResult<CourseStandard>().Deleted.Select(x => new ClassStandard
                {
                    ClassRef = x.CourseID,
                    StandardRef = x.StandardID
                }).ToList();
            ServiceLocatorSchool.StandardService.DeleteClassStandards(classStandards);
        }

        private void DeleteStandards()
        {
            if (context.GetSyncResult<Standard>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Standard>().Deleted.Select(x => x.StandardID).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandards(ids);
        }

        private void DeleteStandardSubject()
        {
            if (context.GetSyncResult<StandardSubject>().Deleted == null)
                return;
            var ids = context.GetSyncResult<StandardSubject>().Deleted.Select(x => x.StandardSubjectID).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandardSubjects(ids);
        }

        private void DeleteCourses()
        {
            if (context.GetSyncResult<Course>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Course>().Deleted.Select(x => x.CourseID).ToList();
            ServiceLocatorSchool.ClassService.Delete(ids);
        }

        private void DeleteRooms()
        {
            if (context.GetSyncResult<Room>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Room>().Deleted.Select(x => x.RoomID).ToList();
            ServiceLocatorSchool.RoomService.DeleteRooms(ids);
        }

        private void DeleteDays()
        {
            if (context.GetSyncResult<CalendarDay>().Deleted == null)
                return;
            var dates = context.GetSyncResult<CalendarDay>().Deleted.Select(x => x.Date.Date).ToList();
            ServiceLocatorSchool.CalendarDateService.Delete(dates);
        }

        private void DeleteDayTypes()
        {
            if (context.GetSyncResult<DayType>().Deleted == null)
                return;
            var ids = context.GetSyncResult<DayType>().Deleted.Select(x => x.DayTypeID).ToList();
            ServiceLocatorSchool.DayTypeService.Delete(ids);
        }

        private void DeleteGradingPeriods()
        {
            if (context.GetSyncResult<GradingPeriod>().Deleted == null)
                return;
            var ids = context.GetSyncResult<GradingPeriod>().Deleted.Select(x => x.GradingPeriodID).ToList();
            ServiceLocatorSchool.GradingPeriodService.Delete(ids);
        }

        private void DeleteMarkingPeriods()
        {
            if (context.GetSyncResult<Term>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Term>().Deleted.Select(x => x.TermID).ToList();
            ServiceLocatorSchool.MarkingPeriodService.DeleteMarkingPeriods(ids);
        }

        private void DeleteStudentSchoolYears()
        {
            if (context.GetSyncResult<StudentAcadSession>().Deleted == null)
                return;
            var assignments = context.GetSyncResult<StudentAcadSession>().Deleted
                                     .Where(x => x.GradeLevelID.HasValue) //TODO: what about this? GL is not assigned?
                                     .Select(x => new StudentSchoolYear
                                         {
                                             GradeLevelRef = x.GradeLevelID.Value,
                                             SchoolYearRef = x.AcadSessionID,
                                             StudentRef = x.StudentID
                                         }).ToList();
            ServiceLocatorSchool.SchoolYearService.UnassignStudents(assignments);
        }

        private void DeleteSchoolYears()
        {
            if (context.GetSyncResult<AcadSession>().Deleted == null)
                return;
            var ids = context.GetSyncResult<AcadSession>().Deleted.Select(x => x.AcadSessionID).ToList();
            ServiceLocatorSchool.SchoolYearService.Delete(ids);
        }

        private void DeleteGradeLevels()
        {
            if (context.GetSyncResult<GradeLevel>().Deleted == null)
                return;
            var ids = context.GetSyncResult<GradeLevel>().Deleted.Select(x=>(int)x.GradeLevelID).ToList();
            ServiceLocatorSchool.GradeLevelService.DeleteGradeLevels(ids);
        }

        private void DeletePhones()
        {
            if (context.GetSyncResult<PersonTelephone>().Deleted == null)
                return;
            var phones = context.GetSyncResult<PersonTelephone>().Deleted;
            foreach (var phone in phones)
            {
                ServiceLocatorSchool.PhoneService.Delete(phone.TelephoneNumber, phone.PersonID);
            }
        }

        private void DeleteSchoolPersons()
        {
            //TODO: implement
        }

        private void DeletePersons()
        {
            if (context.GetSyncResult<Person>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Person>().Deleted.Select(x => x.PersonID).ToList();
            ServiceLocatorSchool.PersonService.Delete(ids);
        }

        private void DeleteAddresses()
        {
            if (context.GetSyncResult<Address>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Address>().Deleted.Select(x => x.AddressID).ToList();
            ServiceLocatorSchool.AddressService.Delete(ids);
        }

        private void DeleteSchools()
        {
            if (context.GetSyncResult<School>().Deleted == null)
                return;
            var ids = context.GetSyncResult<School>().Deleted.Select(x=>x.SchoolID).ToList();
            ServiceLocatorSchool.SchoolService.Delete(ids);
        }
    }
}
