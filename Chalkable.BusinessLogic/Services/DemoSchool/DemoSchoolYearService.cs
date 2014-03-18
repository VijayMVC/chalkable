using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoSchoolYearService : DemoSchoolService, ISchoolYearService
    {
        public DemoSchoolYearService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }
        
        private bool IsOverlaped(DateTime startDate, DateTime endDate, SchoolYearDataAccess dataAccess, SchoolYear schoolYear = null)
        {
            return false;//TODO: isn't supported in INOW
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue)
        {
            var schoolYears = Storage.SchoolYearStorage.GetAll();
            return new PaginatedList<SchoolYear>(schoolYears, start/count, count, schoolYears.Count);
        }

        public void Delete(IList<int> schoolYearIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.SchoolYearStorage.Delete(schoolYearIds);
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            return Storage.SchoolYearStorage.GetCurrentSchoolYear();
        }

        public SchoolYear Add(int id, int schoolId, string name, string description, DateTime? startDate, DateTime? endDate)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var schoolYear = new SchoolYear
            {
                Id = id,
                Description = description,
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                SchoolRef = schoolId
            };
            Storage.SchoolYearStorage.Add(schoolYear);
            return schoolYear;
        }

        public IList<SchoolYear> AddSchoolYears(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.SchoolYearStorage.Add(schoolYears);
            return schoolYears;
        }

        private bool IsOverlaped(DateTime startDate, DateTime endDate,  SchoolYear schoolYear = null)
        {
            //var id = schoolYear != null ? schoolYear.Id : (int?) null;
            //return startDate >= endDate || (dataAccess.IsOverlaped(startDate, endDate, id));
            return false;//TODO: isn't supported in INOW
        }

        public SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var schoolYear = Storage.SchoolYearStorage.GetById(id);
            if (IsOverlaped(startDate, endDate, schoolYear))
                throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_OVERLAPPING_DATA);
            if (schoolYear.Name != name && Storage.SchoolYearStorage.Exists(name))
                throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_ALREADY_EXISTS);
            return Storage.SchoolYearStorage.Edit(id, name, description, startDate, endDate);
        }

        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.SchoolYearStorage.Update(schoolYears);
        }

        public SchoolYear GetSchoolYearById(int id)
        {
            return Storage.SchoolYearStorage.GetById(id);
        }

        public void AssignStudent(int schoolYearId, int personId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.StudentSchoolYearStorage.Add(new StudentSchoolYear
            {
                GradeLevelRef = gradeLevelId,
                SchoolYearRef = schoolYearId,
                StudentRef = personId
            });
        }

        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.StudentSchoolYearStorage.Add(studentAssignments);
        }

        public void Delete(int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.SchoolYearStorage.Delete(schoolYearId);
        }

        public IList<SchoolYear> GetSortedYears()
        {
            return Storage.SchoolYearStorage.GetAll();
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.StudentSchoolYearStorage.GetAll();
        }
    }
}
