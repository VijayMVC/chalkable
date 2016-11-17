using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.EnumMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.ClassPanorama;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public enum ClassSortType
    {
        ClassAsc = 0,
        ClassDesc,

        TeacherAsc,
        TeacherDesc,

        StudentsAsc,
        StudentsDesc,

        AttendanceAsc,
        AttendanceDesc,

        DisciplineAsc,
        DisciplineDesc,

        GradesAsc,
        GradesDesc
    }

    public interface IClassService
    {
        void Add(IList<Class> classes);
        void Edit(IList<Class> classes); 
        void Delete(IList<Class> classes);

        void AddStudents(IList<ClassPerson> classPersons);
        void EditStudents(IList<ClassPerson> classPersons);
        void AddTeachers(IList<ClassTeacher> classTeachers);
        void EditTeachers(IList<ClassTeacher> classTeachers);
        void DeleteTeachers(IList<ClassTeacher> classTeachers);
        void DeleteStudent(IList<ClassPerson> classPersons);

        IList<CourseDetails> GetAdminCourses(int schoolYearId, int gradeLevelId); 
        IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null);
        IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId = null);
        IList<ClassDetails> GetClasses(int schoolYearId, int? studentId, int? teacherId, int? markingPeriodId = null); 
        IList<ClassDetails> SearchClasses(string filter);
        
        ClassDetails GetClassDetailsById(int id);
        
        IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId); 
        IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled); 
        IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId); 

        void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses);
        void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses);
        Class GetById(int id);
        IList<Class> GetByIds(IList<int> ids);
        IList<Class> GetAll();
        IList<Class> GetClassesBySchoolYear(int schoolYearId, int? gradeLevel); 
        IList<ClassDetails> GetAllSchoolsActiveClasses();

        IList<ClassStatsInfo> GetClassesStats(int schoolYearId, int? start, int? count, string filter, int? teacherId, ClassSortType? sortType);
        IList<ClassStatsInfo> GetClassesStatsForStudent(int studentId, int gradingPeriodId);
             
        IList<Class> GetClassesBySchoolYearIds(IList<int> schoolYearIds, int teacherId);
        bool IsTeacherClasses(int teacherId, params int[] classIds);
        ClassPanorama Panorama(int classId, IList<int> academicYears, IList<StandardizedTestFilter> standardizedTestFilters);

        IList<DateTime> GetDays(int classId);
        void PrepareToDelete(IList<Class> classes);
    }

    public class ClassService : SisConnectedService, IClassService
    {
        public ClassService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Class> classes)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ClassDataAccess(u).Insert(classes));
        }
        public void Edit(IList<Class> classes)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ClassDataAccess(u).Update(classes));
        }
        
        public void AssignClassToMarkingPeriod(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            using (var uow = Update())
            {
                var mpClassDa = new MarkingPeriodClassDataAccess(uow);
                mpClassDa.Insert(markingPeriodClasses);
                uow.Commit();
            }
        }
        
        public void AddStudents(IList<ClassPerson> classPersons)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ClassPersonDataAccess(u).Insert(classPersons));
        }

        public void EditStudents(IList<ClassPerson> classPersons)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ClassPersonDataAccess(u).Update(classPersons));
        }

        public void DeleteStudent(IList<ClassPerson> classPersons)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new ClassPersonDataAccess(u).Delete(classPersons));
        }

        public IList<CourseDetails> GetAdminCourses(int schoolYearId, int gradeLevelId)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetAdminCourses(schoolYearId, gradeLevelId));
        }

        public IList<ClassDetails> GetTeacherClasses(int schoolYearId, int teacherId, int? markingPeriodId = null)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetTeacherClasses(schoolYearId, teacherId, markingPeriodId));
        }

        public bool IsTeacherClasses(int teacherId, params int[] classIds)
        {
            var queryCondition = new AndQueryCondition {{nameof(ClassTeacher.PersonRef), teacherId}};
            var teacherClasses = DoRead(u => new ClassTeacherDataAccess(u).GetAll(queryCondition))
                .Select(x => x.ClassRef).ToList();
            return classIds.All(x => teacherClasses.Contains(x));
        }

        public IList<ClassDetails> GetStudentClasses(int schoolYearId, int studentId, int? markingPeriodId)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetStudentClasses(schoolYearId, studentId, markingPeriodId));
        }

        public IList<ClassDetails> SearchClasses(string filter)
        {
            var sl = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string f1 = null, f2 = null, f3 = null;
            if (sl.Length == 0)
                return new List<ClassDetails>();
            if (sl.Length > 0)
                f1 = sl[0];
            if (sl.Length > 1)
                f2 = sl[1];
            if (sl.Length > 2)
                f3 = sl[2];
            return DoRead(uow => new ClassDataAccess(uow).SearchClasses(f1, f2, f3));
        }

        public ClassDetails GetClassDetailsById(int id)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetClassDetailsById(id));
        }
        
        public Class GetById(int id)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetById(id));
        }

        public IList<Class> GetByIds(IList<int> ids)
        {
            return DoRead(uow => new ClassDataAccess(uow).GetByIds(ids));
        }

        public IList<Class> GetAll()
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(u => new ClassDataAccess(u).GetAll());
        }

        public IList<Class> GetClassesBySchoolYear(int schoolYearId, int? gradeLevel)
        {
            var conds = new AndQueryCondition {{nameof(Class.SchoolYearRef), schoolYearId}};
            if (gradeLevel.HasValue)
            {
                conds.Add(nameof(Class.MinGradeLevelRef), gradeLevel, ConditionRelation.GreaterEqual);
                conds.Add(nameof(Class.MaxGradeLevelRef), gradeLevel, ConditionRelation.LessEqual);
            }
            return DoRead(u => new ClassDataAccess(u).GetAll(conds));
        }

        public IList<ClassDetails> GetAllSchoolsActiveClasses()
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            return DoRead(u => new ClassDataAccess(u).GetAllSchoolsActiveClasses());
        }

        public void Delete(IList<Class> classes)
        {
            DoUpdate(uow => new ClassDataAccess(uow).Delete(classes.Select(x=>x.Id).ToList()));
        }

        public void DeleteMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=> new MarkingPeriodClassDataAccess(u).Delete(markingPeriodClasses));
        }

        public void AddTeachers(IList<ClassTeacher> classTeachers)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new ClassTeacherDataAccess(u).Insert(classTeachers));
        }

        public void EditTeachers(IList<ClassTeacher> classTeachers)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new ClassTeacherDataAccess(u).Update(classTeachers));
        }

        public void DeleteTeachers(IList<ClassTeacher> classTeachers)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new ClassTeacherDataAccess(u).Delete(classTeachers));
        }

        public IList<ClassTeacher> GetClassTeachers(int? classId, int? teacherId)
        {
            return DoRead(u => new ClassTeacherDataAccess(u).GetClassTeachers(classId, teacherId));
        }

        public IList<ClassPerson> GetClassPersons(int personId, bool? isEnrolled)
        {
            return GetClassPersons(personId, null, isEnrolled, null);
        }

        public IList<ClassPerson> GetClassPersons(int? personId, int? classId, bool? isEnrolled, int? markingPeriodId)
        {
            using (var uow = Read())
            {
                return new ClassPersonDataAccess(uow)
                    .GetClassPersons(new ClassPersonQuery
                        {
                            ClassId = classId,
                            IsEnrolled = isEnrolled,
                            PersonId = personId,
                            MarkingPeriodId = markingPeriodId
                        });
            }
        }

        public ClassPanorama Panorama(int classId, IList<int> academicYears, IList<StandardizedTestFilter> standardizedTestFilters)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);

            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to view class panorama");

            if (academicYears == null || academicYears.Count == 0)
                throw new ChalkableException("School years is required parameter");

            standardizedTestFilters = standardizedTestFilters ?? new List<StandardizedTestFilter>();
            var componentIds = standardizedTestFilters.Select(x => x.ComponentId);
            var scoreTypeIds = standardizedTestFilters.Select(x => x.ScoreTypeId);

            var schoolYears = ServiceLocator.SchoolYearService.GetSchoolYearsByAcadYears(academicYears);
            var c = ServiceLocator.ClassService.GetClassDetailsById(classId);
            if (c.SchoolYear != null)
                schoolYears = schoolYears.Where(x => x.SchoolRef == c.SchoolYear.SchoolRef).ToList();
            var sectionPanorama = ConnectorLocator.PanoramaConnector.GetSectionPanorama(classId, schoolYears.Select(x=>x.Id).ToList(), componentIds.ToList(), scoreTypeIds.ToList());

            return ClassPanorama.Create(sectionPanorama);
        }

        public IList<DateTime> GetDays(int classId)
        {
            return DoRead(u => new ClassDataAccess(u).GetDays(classId)).Select(x => x.Day).ToList();
        }

        public void PrepareToDelete(IList<Class> classes)
        {
            DoUpdate(u => new ClassTeacherDataAccess(u).PrepareToDelete(classes));
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

        public IList<ClassStatsInfo> GetClassesStats(int schoolYearId, int? start, int? count, string filter, int? teacherId, ClassSortType? sortType)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var iNowSortType = EnumMapperFactory.GetMapper<ClassSortType, SectionSummarySortOption>().Map(sortType ?? ClassSortType.ClassAsc);

            IList<SectionSummary> iNowRes;
            try
            {
                if (teacherId.HasValue)
                    iNowRes = ConnectorLocator.ClassesDashboardConnector.GetSectionSummariesByTeacher(schoolYearId,
                        teacherId.Value, Context.NowSchoolYearTime, start.Value + 1, start.Value + count.Value, filter, iNowSortType);
                else
                    iNowRes = ConnectorLocator.ClassesDashboardConnector.GetSectionsSummaries(schoolYearId, 
                        Context.NowSchoolYearTime, start.Value + 1, start.Value + count.Value, filter, iNowSortType);
            }
            catch (ChalkableSisNotSupportVersionException)
            {
                var chalkableRes = DoRead(u => new ClassDataAccess(u).GetClassesBySchoolYear(schoolYearId, start.Value, count.Value, 
                    filter, teacherId, (int?)sortType));
                return chalkableRes.Select(ClassStatsInfo.Create).ToList();
            }

            using (var u = Read())
            {
                var classesIds = iNowRes.Select(x => x.SectionId).ToList();
                var classes = new ClassDataAccess(u).GetByIds(classesIds);
                var classTeachers = new ClassTeacherDataAccess(u).GetClassTeachers(classesIds);

                return ClassStatsInfo.Create(iNowRes, classes, classTeachers);
            }           
        }

        public IList<ClassStatsInfo> GetClassesStatsForStudent(int studentId, int gradingPeriodId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            if(!(BaseSecurity.IsDistrictOrTeacher(Context) || studentId == Context.PersonId))
                throw new ChalkableSecurityException();

            IList<SectionSummaryForStudent> iNowRes;
            try
            {
                iNowRes = ConnectorLocator.ClassesDashboardConnector.GetSectionSummaryForStudent(Context.SchoolYearId.Value, studentId, gradingPeriodId);
            }
            catch (ChalkableSisNotSupportVersionException)
            {
                var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
                var chalkableRes = DoRead(u => new ClassDataAccess(u).GetStudentClasses(Context.SchoolYearId.Value, studentId, gp.MarkingPeriodRef));
                return chalkableRes.Select(ClassStatsInfo.Create).ToList();
            }
            using (var u = Read())
            {
                var classesIds = iNowRes.Select(x => x.SectionId).ToList();
                var classes = new ClassDataAccess(u).GetByIds(classesIds);
                var classTeachers = new ClassTeacherDataAccess(u).GetClassTeachers(classesIds);
                return ClassStatsInfo.Create(iNowRes, classes, classTeachers);
            }
        }

        public IList<Class> GetClassesBySchoolYearIds(IList<int> schoolYearIds, int teacherId)
        {
            return DoRead(u => new ClassDataAccess(u).GetClassBySchoolYearIds(schoolYearIds, teacherId));
        }
    }
}
