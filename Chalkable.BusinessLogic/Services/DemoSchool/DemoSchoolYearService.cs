using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolYearStorage : BaseDemoIntStorage<SchoolYear>
    {
        public DemoSchoolYearStorage()
            : base(x => x.Id)
        {
        }

        public bool Exists(string name)
        {
            return data.Count(x => x.Value.Name == name) > 0;
        }

        public SchoolYear Add(int id, int schoolId, string name, string description, DateTime startDate, DateTime endDate)
        {
            var schoolYear = new SchoolYear
            {
                Id = id,
                Description = description,
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                SchoolRef = schoolId
            };
            data[id] = schoolYear;
            return schoolYear;
        }

        public SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate)
        {
            var schoolYear = GetById(id);
            schoolYear.Name = name;
            schoolYear.Description = description;
            schoolYear.StartDate = startDate;
            schoolYear.EndDate = endDate;
            return schoolYear;
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            return data.First().Value;
        }


        public SchoolYear GetByDate(DateTime date)
        {
            return data.First(x => date >= x.Value.StartDate && date <= x.Value.EndDate).Value;
        }
    }


    public class DemoStudentSchoolYearStorage : BaseDemoIntStorage<StudentSchoolYear>
    {
        public DemoStudentSchoolYearStorage()
            : base(null, true)
        {
        }

        public IList<StudentSchoolYear> GetAll(int personId)
        {
            return data.Where(x => x.Value.StudentRef == personId).Select(x => x.Value).ToList();
        }

        public bool Exists(IList<int> gradeLevelIds, int personId)
        {
            return GetAll(personId).Count(x => gradeLevelIds.Contains(x.GradeLevelRef)) > 0;
        }

        public IList<StudentSchoolYear> GetList(int? schoolYearId, StudentEnrollmentStatusEnum? enrollmentStatus)
        {
            var ssYears = data.Select(x => x.Value);
            if (schoolYearId.HasValue)
                ssYears = ssYears.Where(x => x.SchoolYearRef == schoolYearId);
            if (enrollmentStatus.HasValue)
                ssYears = ssYears.Where(x => x.EnrollmentStatus == enrollmentStatus);
            return ssYears.ToList();
        }
    }

    public class DemoSchoolYearService : DemoSchoolServiceBase, ISchoolYearService
    {
        private DemoStudentSchoolYearStorage StudentSchoolYearStorage { get; set; }
        private DemoSchoolYearStorage SchoolYearStorage { get; set; }

        public DemoSchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StudentSchoolYearStorage= new DemoStudentSchoolYearStorage();
            SchoolYearStorage = new DemoSchoolYearStorage();
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue, int? schoolId = null, int? acadYear = null, bool activeOnly = false)
        {
            var schoolYears = SchoolYearStorage.GetAll().AsEnumerable();
            if (schoolId.HasValue)
                schoolYears = schoolYears.Where(x => x.SchoolRef == schoolId);
            if (acadYear.HasValue)
                schoolYears = schoolYears.Where(x => x.AcadYear == acadYear);
            if (activeOnly)
                schoolYears = schoolYears.Where(x => x.IsActive);

            return new PaginatedList<SchoolYear>(schoolYears, start/count, count);
        }

        public IList<int> GetYears()
        {
            throw new NotImplementedException();
        }

        public IList<int> GetYears(bool activeOnly = true, bool withDateRange = true)
        {
            var schoolYears = ServiceLocator.SchoolYearService.GetSchoolYears().ToList();
            if (activeOnly)
                schoolYears = schoolYears.Where(x => x.IsActive).ToList();
            if (withDateRange)
                schoolYears = schoolYears.Where(x => x.StartDate.HasValue && x.EndDate.HasValue).ToList();
            return schoolYears.Select(x => x.AcadYear).Distinct().OrderBy(x => x).ToList();
        }

        public void Delete(IList<int> schoolYearIds)
        {
            SchoolYearStorage.Delete(schoolYearIds);
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            return SchoolYearStorage.GetCurrentSchoolYear();
        }

        public IList<SchoolYear> GetPreviousSchoolYears(int fromSchoolYearId, int count = 1)
        {
            throw new NotImplementedException();
        }

        public IList<SchoolYear> GetSchoolYearsByAcadYear(int year, bool activeOnly = true)
        {
            throw new NotImplementedException();
        }

        public IList<SchoolYear> GetSchoolYearsByAcadYears(IList<int> years, bool activeOnly = true)
        {
            throw new NotImplementedException();
        }

        public static SchoolYear GetDemoSchoolYear()
        {
            var currentDate = DateTime.Today;
            return new SchoolYear
            {
                Id = DemoSchoolConstants.CurrentSchoolYearId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Name = "Current School Year",
                Description = "",
                StartDate = new DateTime(currentDate.Year, 1, 1),
                EndDate = new DateTime(currentDate.Year, 12, 31)
            };
        }

        public IList<SchoolYear> Add(IList<SchoolYear> schoolYears)
        {
            return SchoolYearStorage.Add(schoolYears);
        }

        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            return SchoolYearStorage.Update(schoolYears);
        }

        public SchoolYear GetSchoolYearById(int id)
        {
            return SchoolYearStorage.GetById(id);
        }
        
        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            StudentSchoolYearStorage.Add(studentAssignments);
        }

        public void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears)
        {
            throw new NotImplementedException();
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            throw new NotImplementedException();
        }

        public IList<SchoolYear> GetDescSortedYearsByIds(IList<int> ids)
        {
            throw new NotImplementedException();
        }

        public StudentSchoolYear GetPreviousStudentSchoolYearOrNull(int studentId)
        {
            throw new NotImplementedException();
        }

        public IList<SchoolYear> GetSchoolYearsByStudent(int studentId, StudentEnrollmentStatusEnum? enrollmentStatus, DateTime? date)
        {
            throw new NotImplementedException();
        }

        public SchoolYear GetCurrentSchoolYearByStudent(int studentId)
        {
            var sys = ServiceLocator.SchoolYearService.GetSchoolYearsByStudent(studentId, StudentEnrollmentStatusEnum.CurrentlyEnrolled, Context.NowSchoolTime);
            return sys.Count > 0 ? sys[0] : ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
        }

        public void PrepareToDeleteStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            throw new NotImplementedException();
        }

        public IList<SchoolYear> GetSortedYears()
        {
            return SchoolYearStorage.GetAll();
        }

        public IList<SchoolYear> GetSchoolYearsByAcadYear(int year, bool activeOnly = true, bool withDateRange = true)
        {
            var res = SchoolYearStorage.GetAll().Where(x => x.AcadYear == year);
            if (activeOnly)
                res = res.Where(x => x.IsActive);
            if (withDateRange)
                res = res.Where(x => x.StartDate.HasValue && x.EndDate.HasValue).ToList();
            return res.ToList();
        }

        public IList<StudentSchoolYear> GetStudentAssignments(int personId)
        {
            return StudentSchoolYearStorage.GetAll(personId);
        }

        public IList<StudentSchoolYear> GetStudentAssignments(int? schoolYearId, StudentEnrollmentStatusEnum? enrollmentStatus)
        {
            return StudentSchoolYearStorage.GetList(schoolYearId, enrollmentStatus);
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            return StudentSchoolYearStorage.GetAll();
        }

        public SchoolYear GetByDate(DateTime date)
        {
            return SchoolYearStorage.GetByDate(date);
        }

        public bool IsStudentEnrolled(int studentId, int schoolYearId)
        {
            return StudentSchoolYearStorage.GetData().Any(x => x.Value.StudentRef == studentId
                                                               && x.Value.SchoolYearRef == schoolYearId &&
                                                               x.Value.IsEnrolled);
        }

        public int GetStudentGradeLevel(int studentId)
        {
            return StudentSchoolYearStorage.GetAll(studentId).Select(x => x.GradeLevelRef).First();
        }

        public bool GradeLevelExists(int gradeLevelId, int studentId)
        {
            return StudentSchoolYearStorage.Exists(new[] {gradeLevelId}, studentId);
        }
    }
}
