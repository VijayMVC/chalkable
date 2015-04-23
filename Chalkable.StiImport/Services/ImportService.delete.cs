using System.Collections.Generic;
using System.Linq;
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
using ScheduledTimeSlot = Chalkable.StiConnector.SyncModel.ScheduledTimeSlot;
using ScheduledTimeSlotVariation = Chalkable.StiConnector.SyncModel.ScheduledTimeSlotVariation;
using School = Chalkable.StiConnector.SyncModel.School;
using SchoolOption = Chalkable.StiConnector.SyncModel.SchoolOption;
using SectionTimeSlotVariation = Chalkable.StiConnector.SyncModel.SectionTimeSlotVariation;
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
            //TODO: wait for fix student contact from Inow
            Log.LogInfo("delete studentContacts");
            DeleteStudentContacts();
            Log.LogInfo("delete contactRelationship");
            DeleteContactRelationships();
            Log.LogInfo("delete AnnouncementAttributes");
            DeleteAnnouncementAttributes();
            Log.LogInfo("delete gradedItems");
            DeleteGradedItems();
            Log.LogInfo("delete attendanceMonths");
            DeleteAttendanceMonthes();
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
            Log.LogInfo("delete section time slot variations");
            DeleteSectionTimeSlotVariation();
            Log.LogInfo("delete scheduled time slot variations");
            DeleteScheduledTimeSlotVariations();
            Log.LogInfo("delete class periods");
            DeleteClassPeriods();
            Log.LogInfo("delete schedule time slot");
            DeleteScheduledTimeSlots();
            Log.LogInfo("delete periods");
            DeletePeriods();
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
            Log.LogInfo("delete bell schedules");
            DeleteBellSchedules();
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

        private void DeleteContactRelationships()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ContactRelationship>().Deleted == null)
                return;
            var contactRelationships = context.GetSyncResult<StiConnector.SyncModel.ContactRelationship>().Deleted
                .Select(x => new Data.School.Model.ContactRelationship { Id = x.ContactRelationshipID }).ToList();
            ServiceLocatorSchool.ContactService.DeleteContactRelationship(contactRelationships);
        }

        private void DeleteStudentContacts()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.StudentContact>().Deleted == null)
                return;
            var contacts = context.GetSyncResult<StiConnector.SyncModel.StudentContact>().Deleted
                .Select(x => new Data.School.Model.StudentContact
                    {
                        ContactRef = x.ContactID,
                        StudentRef = x.StudentID
                    }).ToList();
            ServiceLocatorSchool.ContactService.DeleteStudentContact(contacts);
        }

        private void DeleteAttendanceMonthes()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.AttendanceMonth>().Deleted == null)
                return;
            var attendanceMonthes = context.GetSyncResult<StiConnector.SyncModel.AttendanceMonth>().Deleted
                .Select(x => new Data.School.Model.AttendanceMonth { Id = x.AttendanceMonthID }).ToList();
            ServiceLocatorSchool.AttendanceMonthService.Delete(attendanceMonthes);
        }

        private void DeleteAnnouncementAttributes()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.ActivityAttribute>().Deleted == null)
                return;
            var annAttributes = context.GetSyncResult<StiConnector.SyncModel.ActivityAttribute>().Deleted
                .Select(x => new Data.School.Model.AnnouncementAttribute() { Id = x.ActivityAttributeID }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Delete(annAttributes);
        }

        private void DeleteGradedItems()
        {
            if (context.GetSyncResult<StiConnector.SyncModel.GradedItem>().Deleted == null)
                return;
            var gradedItems = context.GetSyncResult<StiConnector.SyncModel.GradedItem>().Deleted
                .Select(x => new Data.School.Model.GradedItem { Id = x.GradedItemID }).ToList();
            ServiceLocatorSchool.GradedItemService.Delete(gradedItems);
        }
        
        private void DeleteGradingComments()
        {
            if (context.GetSyncResult<GradingComment>().Deleted == null)
                return;
            var gradingComments = context.GetSyncResult<GradingComment>().Deleted
                .Select(x => new Data.School.Model.GradingComment{Id = x.GradingCommentID}).ToList();
            ServiceLocatorSchool.GradingCommentService.Delete(gradingComments);
        }

        private void DeleteClassroomOptions()
        {
            if (context.GetSyncResult<ClassroomOption>().Deleted == null)
                return;
            var classroomOptions = context.GetSyncResult<ClassroomOption>().Deleted
                .Select(x => new Data.School.Model.ClassroomOption{Id = x.SectionID}).ToList();
            ServiceLocatorSchool.ClassroomOptionService.Delete(classroomOptions);
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
            var gradingScales = context.GetSyncResult<GradingScale>()
                .Deleted.Select(x => new Data.School.Model.GradingScale{Id = x.GradingScaleID}).ToList();
            ServiceLocatorSchool.GradingScaleService.DeleteGradingScales(gradingScales);
        }

        private void DeleteInfractions()
        {
            if (context.GetSyncResult<Infraction>().Deleted == null)
                return;
            var infractions = context.GetSyncResult<Infraction>().Deleted.Select(x => new Data.School.Model.Infraction { Id = x.InfractionID }).ToList();
            ServiceLocatorSchool.InfractionService.Delete(infractions);
        }

        private void DeleteAlternateScores()
        {
            if (context.GetSyncResult<AlternateScore>().Deleted == null)
                return;
            var alternateScores = context.GetSyncResult<AlternateScore>().Deleted.Select(x => new Data.School.Model.AlternateScore { Id = x.AlternateScoreID }).ToList();
            ServiceLocatorSchool.AlternateScoreService.Delete(alternateScores);
        }

        private void DeleteAlphaGrades()
        {
            if (context.GetSyncResult<AlphaGrade>().Deleted == null)
                return;
            var alphaGrades = context.GetSyncResult<AlphaGrade>().Deleted.Select(x => new Data.School.Model.AlphaGrade { Id = x.AlphaGradeID }).ToList();
            ServiceLocatorSchool.AlphaGradeService.Delete(alphaGrades);
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

        private void DeleteSectionTimeSlotVariation()
        {
            if (context.GetSyncResult<SectionTimeSlotVariation>().Deleted == null)
                return;
            var sectionTimeSlotVariations = context.GetSyncResult<SectionTimeSlotVariation>().Deleted
                                  .Select(x => new Data.School.Model.SectionTimeSlotVariation
                                  {
                                      ClassRef = x.SectionID,
                                      ScheduledTimeSlotVariationRef = x.TimeSlotVariationID
                                  })
                                  .ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.DeleteSectionTimeSlotVariations(sectionTimeSlotVariations);
        }

        private void DeleteScheduledTimeSlotVariations()
        {
            if (context.GetSyncResult<ScheduledTimeSlotVariation>().Deleted == null)
                return;
            var sectionTimeSlotVariations = context.GetSyncResult<ScheduledTimeSlotVariation>().Deleted
                                  .Select(x => new Data.School.Model.ScheduledTimeSlotVariation
                                  {
                                      Id = x.TimeSlotVariationId
                                  }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.DeleteScheduledTimeSlotVariations(sectionTimeSlotVariations);
        }

        private void DeleteClassPeriods()
        {
            if (context.GetSyncResult<ScheduledSection>().Deleted == null)
                return;
            var classPeriods = context.GetSyncResult<ScheduledSection>().Deleted
                .Select(x=> new ClassPeriod
                {
                    ClassRef = x.SectionID,
                    DayTypeRef = x.DayTypeID,
                    PeriodRef = x.TimeSlotID
                }).ToList();
            ServiceLocatorSchool.ClassPeriodService.Delete(classPeriods);
        }

        private void DeleteScheduledTimeSlots()
        {
            if (context.GetSyncResult<ScheduledTimeSlot>().Deleted == null)
                return;
            IList<Data.School.Model.ScheduledTimeSlot> sts =
                context.GetSyncResult<ScheduledTimeSlot>().Deleted.Select(x => new Data.School.Model.ScheduledTimeSlot
                {
                    BellScheduleRef = x.BellScheduleID,
                    PeriodRef = x.TimeSlotID
                }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Delete(sts);
        }

        private void DeletePeriods()
        {
            if (context.GetSyncResult<TimeSlot>().Deleted == null)
                return;
            var ids = context.GetSyncResult<TimeSlot>().Deleted.Select(x => x.TimeSlotID).ToList();
            ServiceLocatorSchool.PeriodService.Delete(ids);
        }

        private void DeleteBellSchedules()
        {
            if (context.GetSyncResult<BellSchedule>().Deleted == null)
                return;
            var bs = context.GetSyncResult<BellSchedule>().Deleted.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Delete(bs);
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
            var toDelete = 
                context.GetSyncResult<Standard>().Deleted.ToList()
                .OrderBy(x => x.SYS_CHANGE_VERSION)
                .Select(x=>new Data.School.Model.Standard
                {
                    Id = x.StandardID
                }).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandards(toDelete);
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
            var courses = context.GetSyncResult<Course>().Deleted.ToList()
                .OrderBy(x=>x.SYS_CHANGE_VERSION)
                .Select(x=>new Class
                {
                    Id = x.CourseID
                }).ToList();
            ServiceLocatorSchool.ClassService.Delete(courses);
        }

        private void DeleteRooms()
        {
            if (context.GetSyncResult<Room>().Deleted == null)
                return;
            var ids = context.GetSyncResult<Room>().Deleted
                .Select(x => new Data.School.Model.Room { Id = x.RoomID }).ToList();
            ServiceLocatorSchool.RoomService.DeleteRooms(ids);
        }

        private void DeleteDays()
        {
            if (context.GetSyncResult<CalendarDay>().Deleted == null)
                return;
            var dates = context.GetSyncResult<CalendarDay>().Deleted.Select(x => new Date
            {
                Day = x.Date, SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.CalendarDateService.Delete(dates);
        }

        private void DeleteDayTypes()
        {
            if (context.GetSyncResult<DayType>().Deleted == null)
                return;
            var dayTypes = context.GetSyncResult<DayType>().Deleted.Select(x => new Data.School.Model.DayType
            {
                Id = x.DayTypeID
            }).ToList();
            ServiceLocatorSchool.DayTypeService.Delete(dayTypes);
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
            var ids = context.GetSyncResult<Term>().Deleted.Select(x => new MarkingPeriod { Id = x.TermID }).ToList();
            ServiceLocatorSchool.MarkingPeriodService.DeleteMarkingPeriods(ids);
        }

        private void DeleteStudentSchoolYears()
        {
            if (context.GetSyncResult<StudentAcadSession>().Deleted == null)
                return;
            var assignments = context.GetSyncResult<StudentAcadSession>().Deleted
                                     .Select(x => new StudentSchoolYear
                                         {
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
            var ids = context.GetSyncResult<GradeLevel>().Deleted
                .Select(x => new Data.School.Model.GradeLevel { Id = (int)x.GradeLevelID }).ToList();
            ServiceLocatorSchool.GradeLevelService.Delete(ids);
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
            var staff = context.GetSyncResult<Staff>().Deleted.Select(x => new Data.School.Model.Staff { Id = x.StaffID }).ToList();
            ServiceLocatorSchool.StaffService.Delete(staff);
        }
        
        private void DeletePersons()
        {
            if (context.GetSyncResult<Person>().Deleted == null)
                return;
            var persons = context.GetSyncResult<Person>().Deleted.Select(x => 
                new Data.School.Model.Person{Id = x.PersonID} ).ToList();
            ServiceLocatorSchool.PersonService.Delete(persons);
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
            var addresses = context.GetSyncResult<Address>()
                .Deleted.Select(x => new Data.School.Model.Address { Id = x.AddressID }).ToList();
            ServiceLocatorSchool.AddressService.Delete(addresses);
        }

        private void DeleteSchoolsOptions()
        {
            if (context.GetSyncResult<SchoolOption>().Deleted == null)
                return;
            var schoolOptions = context.GetSyncResult<SchoolOption>()
                .Deleted.Select(x => new Data.School.Model.SchoolOption { Id = x.SchoolID }).ToList();
            ServiceLocatorSchool.SchoolService.DeleteSchoolOptions(schoolOptions);
        }

        private void DeleteSchools()
        {
            if (context.GetSyncResult<School>().Deleted == null)
                return;
            var ids = context.GetSyncResult<School>()
                .Deleted.Select(x => new Data.School.Model.School { Id = x.SchoolID }).ToList();
            ServiceLocatorSchool.SchoolService.Delete(ids);
        }
    }
}
