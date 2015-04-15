using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoSchoolYearService : DemoSchoolServiceBase, ISchoolYearService
    {
        public DemoSchoolYearService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
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

        public IList<SchoolYear> Add(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.SchoolYearStorage.Add(schoolYears);
            return schoolYears;
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
        
        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.StudentSchoolYearStorage.Add(studentAssignments);
        }

        public void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears)
        {
            throw new NotImplementedException();
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            throw new NotImplementedException();
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
