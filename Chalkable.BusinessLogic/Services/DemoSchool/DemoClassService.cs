using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.ClassPanorama;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using ClassroomOption = Chalkable.Data.School.Model.ClassroomOption;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassPersonStorage : BaseDemoIntStorage<ClassPerson>
    {
        public DemoClassPersonStorage()
            : base(null, true)
        {
        }

        public void Delete(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);

            foreach (var classPerson in classPersons)
            {
                var item = data.First(x => x.Value == classPerson);
                data.Remove(item.Key);
            }
        }

        public ClassPerson GetClassPerson(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);
            return classPersons.First();
        }

        public bool Exists(ClassPersonQuery classPersonQuery)
        {
            var classPersons = GetClassPersons(classPersonQuery);
            var classPersonsList = classPersons as IList<ClassPerson> ?? classPersons.ToList();

            return (classPersonQuery.ClassId.HasValue || classPersonQuery.MarkingPeriodId.HasValue ||
                    classPersonQuery.PersonId.HasValue) && classPersonsList.Count > 0;
        }

        public bool Exists(int? classId, int? personId)
        {
            return Exists(new ClassPersonQuery
            {
                ClassId = classId,
                PersonId = personId
            });
        }

        public IEnumerable<ClassPerson> GetClassPersons(int classId)
        {
            return GetClassPersons(new ClassPersonQuery { ClassId = classId });
        }

        public IEnumerable<ClassPerson> GetClassPersons(ClassPersonQuery classPersonQuery)
        {
            var classPersons = data.Select(x => x.Value);

            if (classPersonQuery.ClassId.HasValue)
                classPersons = classPersons.Where(x => x.ClassRef == classPersonQuery.ClassId);
            if (classPersonQuery.MarkingPeriodId.HasValue)
                classPersons = classPersons.Where(x => x.MarkingPeriodRef == classPersonQuery.MarkingPeriodId);
            if (classPersonQuery.PersonId.HasValue)
                classPersons = classPersons.Where(x => x.PersonRef == classPersonQuery.PersonId);

            if (classPersonQuery.IsEnrolled.HasValue)
                classPersons = classPersons.Where(x => x.IsEnrolled);

            return classPersons;
        }
    }

    public class DemoClassStorage : BaseDemoIntStorage<Class>
    {
        public DemoClassStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoClassTeacherStorage : BaseDemoIntStorage<ClassTeacher>
    {
        public DemoClassTeacherStorage()
            : base(null, true)
        {
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            var res = data.Select(x => x.Value);
            if (classId.HasValue)
                res = res.Where(x => x.ClassRef == classId.Value);
            if (teacherId.HasValue)
                res = res.Where(x => x.PersonRef == teacherId.Value);
            return res.ToList();
        }

        public bool Exists(int? classId, int? teacherId)
        {
            return GetClassTeachers(classId, teacherId).Count > 0;
        }

    }

    public class DemoClassService : DemoSchoolServiceBase, IClassService
    {
        private DemoClassStorage ClassStorage { get; set; }
        
        private DemoClassTeacherStorage ClassTeacherStorage { get; set; }
        private DemoClassPersonStorage ClassPersonStorage { get; set; }
        public DemoClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ClassStorage = new DemoClassStorage();
            ClassPersonStorage = new DemoClassPersonStorage();
            ClassTeacherStorage = new DemoClassTeacherStorage();
        }
        
        public void Add(IList<Class> classes)
        {
            ClassStorage.Add(classes);
        }

        public void AddClass(int id, string name, string classNumber)
        {
            ClassStorage.Add(new Class
            {
                Id = id,
                Name = name,
                Description = name,
                ChalkableDepartmentRef = null,
                ClassNumber = classNumber,
                PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId
            });

            var mpcList = new List<MarkingPeriodClass>
            {
                new MarkingPeriodClass
                {
                    MarkingPeriodRef = DemoSchoolConstants.FirstMarkingPeriodId,
                    ClassRef = id
                },
                new MarkingPeriodClass
                {
                    MarkingPeriodRef = DemoSchoolConstants.SecondMarkingPeriodId,
                    ClassRef = id
                }
            };
            AssignClassToMarkingPeriod(mpcList);


            var clsAnnouncementTypeList = new List<ClassAnnouncementType>
            {
                new ClassAnnouncementType
                {
                    ClassRef = id,
                    Description = "Academic Achievement",
                    Gradable = true,
                    Name = "Academic Achievement",
                    Percentage = 50
                },
                new ClassAnnouncementType
                {
                    ClassRef = id,
                    Description = "Academic Practice",
                    Gradable = true,
                    Name = "Academic Practice",
                    Percentage = 50
                }
            };

            ((DemoClassAnnouncementTypeService) ServiceLocator.ClassAnnouncementTypeService).Add(clsAnnouncementTypeList);
       

            var periods = ServiceLocator.PeriodService.GetPeriods(DemoSchoolConstants.CurrentSchoolYearId);

            ServiceLocator.ClassPeriodService.Add(periods.Select(period => new ClassPeriod()
            {
                ClassRef = id,
                DayTypeRef = DemoSchoolConstants.DayTypeId1,
                PeriodRef = period.Id,
                Period = period
            }).ToList());

            AddTeachers(new[] {new ClassTeacher
            {
                ClassRef = id,
                PersonRef = DemoSchoolConstants.TeacherId,
                IsPrimary = true
            }});

            ServiceLocator.ClassroomOptionService.Add(new[] {new ClassroomOption()
            {
                Id = id,
                SeatingChartColumns = 3,
                SeatingChartRows = 3,
                AveragingMethod = "P",
                DefaultActivitySortOrder = "D",
                StandardsCalculationMethod = "A",
                StandardsCalculationRule = "G",
                DisplayStudentAverage = true
            }});

            for (var gp = DemoSchoolConstants.GradingPeriodQ1; gp <= DemoSchoolConstants.GradingPeriodQ4; ++gp)
                AddGradeBookForClass(id, gp);
        }

        private void AddGradeBookForClass(int classId, int gradingPeriodId)
        {
            var studentAverages = ClassPersonStorage.GetClassPersons(new ClassPersonQuery()
            {
                ClassId = classId
            }).Select(x => x.PersonRef).Distinct().Select(x => new StudentAverage()
            {
                IsGradingPeriodAverage = true,
                GradingPeriodId = gradingPeriodId,
                StudentId = x,
                SectionId = classId
            });

            ((DemoGradingStatisticService) ServiceLocator.GradingStatisticService).AddGradeBook(new Gradebook()
            {
                SectionId = classId,
                Options = new StiConnector.Connectors.Model.ClassroomOption(),
                StudentAverages = studentAverages
            });
        }

        public void Edit(IList<Class> classes)
        {
            ClassStorage.Update(classes);
        }
        
        public void Delete(IList<Class> classes)
        {
            ClassStorage.Delete(classes);
        }

        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            ((DemoMarkingPeriodService)ServiceLocator.MarkingPeriodService).AddMarkingPeriodClasses(markingPeriodClasses);
        }
        
        public void AddStudents(IList<ClassPerson> classPersons)
        {
            ClassPersonStorage.Add(classPersons);
        }

        public void EditStudents(IList<ClassPerson> classPersons)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(IList<ClassPerson> classPersons)
        {
            throw new NotImplementedException();
        }

        public IList<CourseDetails> GetAdminCourses(int schoolYearId, int gradeLevelId)
        {
            var classes = ClassStorage.GetAll()
                          .Where(c => c.SchoolYearRef == schoolYearId
                                      && c.MinGradeLevelRef <= gradeLevelId && c.MaxGradeLevelRef >= gradeLevelId)
                          .Select(c => GetClassDetailsById(c.Id)).ToList();

            return  ClassStorage.GetAll().Where(c => !c.CourseRef.HasValue && classes.Any(y => y.CourseRef == c.Id))
                            .Select(c => new CourseDetails
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    ClassNumber = c.ClassNumber,
                                    CourseRef = c.CourseRef,
                                    CourseTypeRef = c.CourseTypeRef,
                                    Description = c.Description,
                                    GradingScaleRef = c.GradingScaleRef,
                                    MinGradeLevelRef = c.MinGradeLevelRef,
                                    MaxGradeLevelRef = c.MaxGradeLevelRef,
                                    ChalkableDepartmentRef = c.ChalkableDepartmentRef,
                                    PrimaryTeacherRef = c.PrimaryTeacherRef,
                                    RoomRef = c.RoomRef,
                                    SchoolYearRef = c.SchoolYearRef,
                                    Classes = classes.Where(x => x.CourseRef == c.Id).ToList()
                                }).ToList();
        }

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null)
        {
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var classteachers = ClassTeacherStorage.GetAll();
            foreach (var @class in classes)
            {
                if (@class.SchoolYearRef != schoolYearId) continue;
                if (classteachers.Any(x => x.ClassRef == @class.Id && x.PersonRef == teacherId))
                {
                    var cd = GetClassDetailsById(@class.Id);
                    if (!markingPeriodId.HasValue || cd.MarkingPeriodClasses.Any(x => x.MarkingPeriodRef == markingPeriodId.Value))
                        res.Add(cd);
                }
            }
            return res;
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId = null)
        {
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var classPersons = ClassPersonStorage.GetAll();
            foreach (var @class in classes)
            {
                if (@class.SchoolYearRef != schoolYearId) continue;
                if (classPersons.Any(x => x.ClassRef == @class.Id && x.PersonRef == studentId))
                {
                    var cd = GetClassDetailsById(@class.Id);
                    if (!markingPeriodId.HasValue || cd.MarkingPeriodClasses.Any(x => x.MarkingPeriodRef == markingPeriodId.Value))
                        res.Add(cd);
                }
            }
            return res;
        }

        public IList<ClassDetails> SearchClasses(string filter)
        {
            var res = new List<ClassDetails>();
            var classes = ClassStorage.GetAll();
            var words = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var @class in classes)
            {
                if (words.Any(x => @class.Name.Contains(x) || @class.ClassNumber.Contains(x)))
                    res.Add(GetClassDetailsById(@class.Id));
            }
            return res;
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            var clazz = GetById(id);
            var clsDetails = new ClassDetails
            {
                ChalkableDepartmentRef = clazz.ChalkableDepartmentRef,
                CourseRef = clazz.CourseRef,
                ClassNumber = clazz.ClassNumber,
                Description = clazz.Description,
                Id = clazz.Id,
                Name = clazz.Name,
                RoomRef = clazz.RoomRef,
                SchoolYearRef = clazz.SchoolYearRef,
                PrimaryTeacher = ServiceLocator.PersonService.GetPerson(DemoSchoolConstants.TeacherId),
                PrimaryTeacherRef = DemoSchoolConstants.TeacherId,
                StudentsCount = 10
            };

            var markingPeriodClasses = ((DemoMarkingPeriodService)ServiceLocator.MarkingPeriodService).GetMarkingPeriodClassById(clsDetails.Id).ToList();
            if (clsDetails.PrimaryTeacherRef.HasValue)
                clsDetails.PrimaryTeacher = ServiceLocator.PersonService.GetPersonDetails(clsDetails.PrimaryTeacherRef.Value);
            clsDetails.MarkingPeriodClasses = markingPeriodClasses;
            var classTeachers = GetClassTeachers(clsDetails.Id, null);
            clsDetails.ClassTeachers = classTeachers;
            return clsDetails;
        }

        public void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            throw new NotImplementedException();
        }

        public Class GetById(int id)
        {
            return ClassStorage.GetById(id);
        }

        public IList<Class> GetAll()
        {
            return ClassStorage.GetAll();
        }

        public IList<ClassDetails> GetAllSchoolsActiveClasses()
        {
            throw new NotImplementedException();
        }

        public IList<ClassStatsInfo> GetClassesBySchoolYear(int schoolYearId, int? start, int? count, string filter, int? teacherId, ClassSortType? sortyType)
        {
            throw new NotImplementedException();
        }

        public IList<Class> GetClassesBySchoolYearIds(IList<int> schoolYearIds, int teacherId)
        {
            throw new NotImplementedException();
        }

        public bool IsTeacherClasses(int teacherId, params int[] classIds)
        {
            throw new NotImplementedException();
        }

        public ClassPanorama Panorama(int classId, IList<int> schoolYearIds, IList<StandardizedTestFilter> standardizedTestFilters)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDetails> GetClassesSortedByPeriod()
        {
            IList<ClassDetails> classes;
            int? teacherId = null;
            int? studentId = null;
            if (Context.RoleId == CoreRoles.TEACHER_ROLE.Id)
            {
                teacherId = Context.PersonId;
                classes = GetTeacherClasses(Context.SchoolYearId.Value, Context.PersonId.Value);
            }
            else if (Context.RoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                studentId = Context.PersonId;
                classes = GetStudentClasses(Context.SchoolYearId.Value, Context.PersonId.Value);
            }
            else
                throw new NotImplementedException();

            var schedule = ServiceLocator.ClassPeriodService.GetSchedule(teacherId, studentId, null,
                Context.NowSchoolYearTime.Date, Context.NowSchoolYearTime.Date).OrderBy(x => x.PeriodOrder);
            var res = new List<ClassDetails>();
            foreach (var classPeriod in schedule)
            {
                var c = classes.FirstOrDefault(x => x.Id == classPeriod.ClassId);
                if (c != null && res.All(x => x.Id != c.Id))
                    res.Add(c);
            }
            classes = classes.Where(x => res.All(y => y.Id != x.Id)).OrderBy(x => x.Name).ToList();


            var l = res.Concat(classes);
            var classDetailses = l as IList<ClassDetails> ?? l.ToList();
            foreach (var cls in classDetailses)
            {
                cls.ClassTeachers = ClassTeacherStorage.GetClassTeachers(cls.Id, null);
            }
            return classDetailses.ToList();
        }

        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = personId,
                IsEnrolled = isEnrolled
            }).ToList();
        }
        
        public void AddTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Add(classTeachers);
        }

        public void EditTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Update(classTeachers);
        }

        public void DeleteTeachers(IList<ClassTeacher> classTeachers)
        {
            ClassTeacherStorage.Delete(classTeachers);
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            return ClassTeacherStorage.GetClassTeachers(classId, teacherId);
        }
        
        public IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId)
        {
            return ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                ClassId = classId,
                IsEnrolled = isEnrolled,
                MarkingPeriodId = markingPeriodId,
                PersonId = personId
            }).ToList();
        }

        public IList<ClassPerson> GetClassPersons(int? classId)
        {
            return ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                ClassId = classId
            }).ToList();
        }

        public IList<ClassDetails> GetClasses(int schoolYearId, int? studentId, int? teacherId, int? markingPeriodId = null)
        {
            IList<ClassDetails> classes = new List<ClassDetails>();
            if (studentId.HasValue)
                classes = GetStudentClasses(schoolYearId, studentId.Value, markingPeriodId);
            if (teacherId.HasValue)
                classes = GetTeacherClasses(schoolYearId, teacherId.Value, markingPeriodId);
            return classes;
            
        }

        public bool ClassTeacherExists(int classRef, int userId)
        {
            return ClassTeacherStorage.Exists(classRef, userId);
        }

        public bool ClassPersonExists(int classRef, int? userId)
        {
            return ClassPersonStorage.Exists(classRef, userId);
        }

        public IList<ClassPerson> GetClassPersonsByMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            return ClassPersonStorage.GetAll().Where(x => markingPeriods.Any(mp => mp.Id == x.MarkingPeriodRef)).ToList();
        }

        public IList<ClassPerson> GetByClasses(IList<ClassDetails> classes)
        {
            return ClassPersonStorage.GetAll().Where(cp => classes.Any(c => c.Id == cp.ClassRef)).ToList();
        }

        public IList<ClassPerson> GetClassPersons(IList<ClassDetails> classes)
        {
            var clsIds = classes.Select(x => x.Id);
            return ClassPersonStorage.GetAll().Where(x => clsIds.Contains(x.ClassRef)).ToList();
        }

        public IList<ClassPerson> GetClassPersons()
        {
            return ClassPersonStorage.GetAll();
        }
    }
}
