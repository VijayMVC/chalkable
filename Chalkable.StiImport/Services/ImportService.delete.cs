using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AlphaGrade = Chalkable.StiConnector.SyncModel.AlphaGrade;
using AlternateScore = Chalkable.StiConnector.SyncModel.AlternateScore;
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
using ScheduledTimeSlot = Chalkable.StiConnector.SyncModel.ScheduledTimeSlot;
using School = Chalkable.StiConnector.SyncModel.School;
using SchoolOption = Chalkable.StiConnector.SyncModel.SchoolOption;
using Staff = Chalkable.StiConnector.SyncModel.Staff;
using StaffSchool = Chalkable.StiConnector.SyncModel.StaffSchool;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StandardSubject = Chalkable.StiConnector.SyncModel.StandardSubject;
using Student = Chalkable.StiConnector.SyncModel.Student;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessDelete()
        {
            Log.LogInfo("delete schoolsOptions");
            DeleteSchoolsOptions();
            Log.LogInfo("delete grading comments");
            DeleteGradingComments();
            Log.LogInfo("delete class room options");
            DeleteClassroomOptions();
            Log.LogInfo("delete scale ranges");
            DeleteGradingScaleRanges();
            Log.LogInfo("delete infractions");
            DeleteInfractions();
            Log.LogInfo("delete alternate scores");
            DeleteAlternateScores();
            Log.LogInfo("delete alpha grades");
            DeleteAlphaGrades();
            Log.LogInfo("delete attendance level reasons");
            DeleteAttendanceLevelReasons();
            Log.LogInfo("delete attendance reasons");
            DeleteAttendanceReasons();
            Log.LogInfo("delete class persons");
            DeleteClassPersons();
            Log.LogInfo("delete class periods");
            DeleteClassPeriods();
            Log.LogInfo("delete periods");
            DeletePeriods();
            Log.LogInfo("delete schedule time slot");
            DeleteScheduledTimeSlots();
            Log.LogInfo("delete marking period classes");
            DeleteMarkingPeriodClasses();
            Log.LogInfo("delete class standards");
            DeleteClassStandard();
            Log.LogInfo("delete standards");
            DeleteStandards();
            Log.LogInfo("delete standard subjects");
            DeleteStandardSubject();
            Log.LogInfo("delete class teachers");
            DeleteClassTeachers();
            Log.LogInfo("delete courses");
            DeleteCourses();
            Log.LogInfo("delete grading scales");
            DeleteGradingScales();
            Log.LogInfo("delete rooms");
            DeleteRooms();
            Log.LogInfo("delete days");
            DeleteDays();
            Log.LogInfo("delete day types");
            DeleteDayTypes();
            Log.LogInfo("delete grading periods");
            DeleteGradingPeriods();
            Log.LogInfo("delete marking periods");
            DeleteMarkingPeriods();
            Log.LogInfo("delete grading");
            DeleteStudentSchoolYears();
            Log.LogInfo("school years");
            DeleteSchoolYears();
            Log.LogInfo("delete grading level");
            DeleteGradeLevels();
            Log.LogInfo("delete phones");
            DeletePhones();
            Log.LogInfo("delete persons emails");
            DeletePersonsEmails();

            Log.LogInfo("delete StaffSchool");
            DeleteStaffSchool();

            Log.LogInfo("delete StudentSchool");
            DeleteStudentSchool();

            Log.LogInfo("delete Student");
            DeleteStudent();

            Log.LogInfo("delete Staff");
            DeleteStaff();

            Log.LogInfo("delete persons");
            DeletePersons();
            
            Log.LogInfo("delete school users");
            DeleteSchoolUsers();

            Log.LogInfo("delete users");
            DeleteUsers();

            Log.LogInfo("delete addresses");
            DeleteAddresses();
            Log.LogInfo("delete schools");
            DeleteSchools();
        }

        private void DeleteGradingComments()
        {
            if (context.GetSyncResult<GradingComment>().Deleted == null)
                return;
            var ids = context.GetSyncResult<GradingComment>().Deleted.Select(x => x.GradingCommentID).ToList();
            ServiceLocatorSchool.GradingCommentService.Delete(ids);
        }

        private void DeleteClassroomOptions()
        {
            if (context.GetSyncResult<ClassroomOption>().Deleted == null)
                return;
            var ids = context.GetSyncResult<ClassroomOption>().Deleted.Select(x => x.SectionID).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Delete(ids);
        }

        private void DeleteGradingScaleRanges()
        {
            if (context.GetSyncResult<GradingScaleRange>().Deleted == null)
                return;
            var gsr =
                context.GetSyncResult<GradingScaleRange>().Deleted.Select(x => new Data.School.Model.GradingScaleRange
                    {
                        AlphaGradeRef = x.AlphaGradeID,
                        GradingScaleRef = x.GradingScaleID
                    }).ToList();
            ServiceLocatorSchool.GradingScaleService.DeleteGradingScaleRanges(gsr);
        }

        private void DeleteGradingScales()
        {
            if (context.GetSyncResult<GradingScale>().Deleted == null)
                return;
            var ids = context.GetSyncResult<GradingScale>().Deleted.Select(x => x.GradingScaleID).ToList();
            ServiceLocatorSchool.GradingScaleService.DeleteGradingScales(ids);
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
            if (context.GetSyncResult<AlternateScore>().Deleted == null)
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
            var students = context.GetSyncResult<StudentScheduleTerm>().Deleted
                                  .Select(x => new ClassPerson
                                      {
                                          ClassRef = x.SectionID,
                                          MarkingPeriodRef = x.TermID,
                                          PersonRef = x.StudentID
                                      })
                                  .ToList();
            ServiceLocatorSchool.ClassService.DeleteStudent(students);
        }

        private void DeleteClassPeriods()
        {
            if (context.GetSyncResult<ScheduledSection>().Deleted == null)
                return;
            var scheduleSections = context.GetSyncResult<ScheduledSection>().Deleted.ToList();
            foreach (var scheduledSection in scheduleSections)
            {
                ServiceLocatorSchool.ClassPeriodService.Delete(scheduledSection.TimeSlotID, scheduledSection.SectionID,
                                                               scheduledSection.DayTypeID);
            }
        }

        private void DeletePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Deleted == null)
                return;
            var ids = context.GetSyncResult<TimeSlot>().Deleted.Select(x => x.TimeSlotID).ToList();
            ServiceLocatorSchool.PeriodService.Delete(ids);
        }

        private void DeleteScheduledTimeSlots()
        {
            if (context.GetSyncResult<ScheduledTimeSlot>().Deleted == null)
                return;
            IList<Data.School.Model.ScheduledTimeSlot> sts =
                context.GetSyncResult<ScheduledTimeSlot>().Deleted.Select(x => new Data.School.Model.ScheduledTimeSlot
                    {
                        BellScheduleID = x.BellScheduleID,
                        TimeSlotID = x.TimeSlotID
                    }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Delete(sts);
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
            var toDelete = context.GetSyncResult<Standard>().Deleted.ToDictionary(x => x.StandardID, x => ServiceLocatorSchool.StandardService.GetStandardById(x.StandardID));
            var sorted = TopologicSort(x => x.Id, x => x.ParentStandardRef, toDelete).Reverse();
            var ids = sorted.Select(x => x.Id).ToList();
            
            foreach (var id in ids)
                ServiceLocatorSchool.AnnouncementService.RemoveAllAnnouncementStandards(id);
            ServiceLocatorSchool.StandardService.DeleteStandards(ids);
        }

        private void DeleteStandardSubject()
        {
            if (context.GetSyncResult<StandardSubject>().Deleted == null)
                return;
            var ids = context.GetSyncResult<StandardSubject>().Deleted.Select(x => x.StandardSubjectID).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandardSubjects(ids);
        }

        private void DeleteClassTeachers()
        {
            if (context.GetSyncResult<SectionStaff>().Deleted == null)
                return;
            var teachers = context.GetSyncResult<SectionStaff>().Deleted.Select(x => new ClassTeacher
                {
                    ClassRef = x.SectionID,
                    PersonRef = x.StaffID
                }).ToList();
            ServiceLocatorSchool.ClassService.DeleteTeachers(teachers);
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
            var ids = context.GetSyncResult<GradeLevel>().Deleted.Select(x => (int)x.GradeLevelID).ToList();
            ServiceLocatorSchool.GradeLevelService.DeleteGradeLevels(ids);
        }

        private void DeletePhones()
        {
            if (context.GetSyncResult<PersonTelephone>().Deleted == null)
                return;
            var phones = context.GetSyncResult<PersonTelephone>().Deleted.Select(x => new Phone
                {
                    DigitOnlyValue = x.TelephoneNumber,
                    PersonRef = x.PersonID,
                    IsPrimary = x.IsPrimary,
                    Value = x.FormattedTelephoneNumber
                }).ToList();
            ServiceLocatorSchool.PhoneService.Delete(phones);
        }

        private void DeletePersonsEmails()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Deleted == null)
                return;
            var personEmails = context.GetSyncResult<StiConnector.SyncModel.PersonEmail>().Deleted
                                      .Select(x => new Data.School.Model.PersonEmail
                                          {
                                              PersonRef = x.PersonID,
                                              Description = x.Description,
                                              EmailAddress = x.EmailAddress,
                                              IsListed = x.IsListed,
                                              IsPrimary = x.IsPrimary
                                          }).ToList();
            ServiceLocatorSchool.PersonEmailService.DeletePersonsEmails(personEmails);
        }

        private void DeleteStaffSchool()
        {
            if (context.GetSyncResult<StaffSchool>().Deleted == null)
                return;
            var ss = context.GetSyncResult<StaffSchool>().Deleted.Select(x => new Data.School.Model.StaffSchool{SchoolRef = x.SchoolID, StaffRef = x.StaffID}).ToList();
            ServiceLocatorSchool.StaffService.DeleteStaffSchools(ss);
        }

        private void DeleteStudentSchool()
        {
            if (context.GetSyncResult<StudentSchool>().Deleted == null)
                return;
            var ss = context.GetSyncResult<StudentSchool>().Deleted.Select(x => new Data.School.Model.StudentSchool { SchoolRef = x.SchoolID, StudentRef = x.StudentID}).ToList();
            ServiceLocatorSchool.StudentService.DeleteStudentSchools(ss);
        }
        
        private void DeleteStudent()
        {
            if (context.GetSyncResult<Student>().Deleted == null)
                return;
            var students = context.GetSyncResult<Student>().Deleted.Select(x => new Data.School.Model.Student { Id = x.StudentID }).ToList();
            ServiceLocatorSchool.StudentService.DeleteStudents(students);
        }
        
        private void DeleteStaff()
        {
            if (context.GetSyncResult<Staff>().Deleted == null)
                return;
            var staff = context.GetSyncResult<Staff>().Deleted.Select(x => new Data.School.Model.Staff() { Id = x.StaffID }).ToList();
            ServiceLocatorSchool.StaffService.Delete(staff);
        }
        
        private void DeletePersons()
        {
            if (context.GetSyncResult<Person>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Person>().Deleted.Select(x => x.PersonID).ToList();
            ServiceLocatorSchool.PersonService.Delete(ids);
        }

        private void DeleteSchoolUsers()
        {
            if (context.GetSyncResult<UserSchool>().Deleted == null)
                return;
            var masterSchoolUsers = context.GetSyncResult<UserSchool>().Deleted.Select(x => new Data.Master.Model.SchoolUser
                {
                    SchoolRef = x.SchoolID, 
                    UserRef = x.UserID, 
                    DistrictRef = ServiceLocatorSchool.Context.DistrictId.Value
                }).ToList();
            ServiceLocatorMaster.UserService.DeleteSchoolUsers(masterSchoolUsers);

            var districtUserSchool = context.GetSyncResult<UserSchool>().Deleted.Select(x => new Data.School.Model.UserSchool
            {
                SchoolRef = x.SchoolID,
                UserRef = x.UserID
            }).ToList();
            ServiceLocatorSchool.UserSchoolService.Delete(districtUserSchool);
        }

        private void DeleteUsers()
        {
            if (context.GetSyncResult<User>().Deleted == null)
                return;
            var ids = context.GetSyncResult<User>().Deleted.Select(x => x.UserID).ToList();
            ServiceLocatorMaster.UserService.DeleteUsers(ids, ServiceLocatorSchool.Context.DistrictId.Value);
        }

        private void DeleteAddresses()
        {
            if (context.GetSyncResult<Address>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Address>().Deleted.Select(x => x.AddressID).ToList();
            ServiceLocatorSchool.AddressService.Delete(ids);
        }

        private void DeleteSchoolsOptions()
        {
            if (context.GetSyncResult<SchoolOption>().Deleted == null)
                return;
            var ids = context.GetSyncResult<SchoolOption>().Deleted.Select(x => x.SchoolID).ToList();
            ServiceLocatorSchool.SchoolService.DeleteSchoolOptions(ids);
        }

        private void DeleteSchools()
        {
            if (context.GetSyncResult<School>().Deleted == null)
                return;
            var ids = context.GetSyncResult<School>().Deleted.Select(x => x.SchoolID).ToList();
            ServiceLocatorSchool.SchoolService.Delete(ids);
        }
    }
}
