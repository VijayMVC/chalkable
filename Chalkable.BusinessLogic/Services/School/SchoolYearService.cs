using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolYearService
    {
        SchoolYear Add(int id, int schoolId, string name, string description, DateTime? startDate, DateTime? endDate);
        IList<SchoolYear> AddSchoolYears(IList<SchoolYear> schoolYears); 
        SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate);
        IList<SchoolYear> Edit(IList<SchoolYear> schoolYears); 
        SchoolYear GetSchoolYearById(int id);
        PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue);
        void AssignStudent(int schoolYearId, int personId, int gradeLevelId);
        void Delete(int schoolYearId);
        void Delete(IList<int> schoolYearIds);
        SchoolYear GetCurrentSchoolYear();
        IList<SchoolYear> GetSortedYears();
        IList<StudentSchoolYear> GetStudentAssignments();
        void AssignStudent(IList<StudentSchoolYear> studentAssignments);
        void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears);
        void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears);

    }

    public class SchoolYearService : SchoolServiceBase, ISchoolYearService
    {
        public SchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test 
        public SchoolYear Add(int id, int schoolId, string name, string description, DateTime? startDate, DateTime? endDate)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                var schoolYear = new SchoolYear
                    {
                        Id = id,
                        Description = description,
                        Name = name,
                        StartDate = startDate,
                        EndDate = endDate,
                        SchoolRef = schoolId
                    };
                
                da.Insert(schoolYear);
                uow.Commit();
                return schoolYear;
            }
        }
        
        private bool IsOverlaped(DateTime startDate, DateTime endDate, SchoolYearDataAccess dataAccess, SchoolYear schoolYear = null)
        {
            //var id = schoolYear != null ? schoolYear.Id : (int?) null;
            //return startDate >= endDate || (dataAccess.IsOverlaped(startDate, endDate, id));
            return false;//TODO: isn't supported in INOW
        }

        public SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                var schoolYear = da.GetById(id);

                if(IsOverlaped(startDate, endDate, da, schoolYear))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_OVERLAPPING_DATA);
                if (schoolYear.Name != name && da.Exists(name))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_ALREADY_EXISTS);

                schoolYear.Name = name;
                schoolYear.Description = description;
                schoolYear.StartDate = startDate;
                schoolYear.EndDate = endDate;
                da.Update(schoolYear);
                uow.Commit();
                return schoolYear;
            }
        }

        public SchoolYear GetSchoolYearById(int id)
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetById(id);
            }
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetPage(start, count);
            }
        }

        public void AssignStudent(int schoolYearId, int personId, int gradeLevelId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new StudentSchoolYearDataAccess(uow);
                da.Insert(new StudentSchoolYear
                    {
                        GradeLevelRef = gradeLevelId,
                        SchoolYearRef = schoolYearId,
                        StudentRef = personId
                    });
                uow.Commit();
            }
        }

        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new StudentSchoolYearDataAccess(uow);
                da.Insert(studentAssignments);
                uow.Commit();
            }
        }

        public void Delete(int schoolYearId)
        {
            Delete(new List<int> {schoolYearId});
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            var nowDate = Context.NowSchoolTime.Date;
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetByDate(nowDate);
            }
        }
        public IList<SchoolYear> GetSortedYears()
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetAll();
            }
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new StudentSchoolYearDataAccess(uow);
                return da.GetAll();
            }
        }


        public IList<SchoolYear> AddSchoolYears(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Insert(schoolYears);
                uow.Commit();
                return schoolYears;
            }
        }

        public void Delete(IList<int> schoolYearIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Delete(schoolYearIds);
                uow.Commit();
            }
        }


        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Update(schoolYears);
                uow.Commit();
                return schoolYears;
            }
        }

        public void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StudentSchoolYearDataAccess(uow).Delete(studentSchoolYears);
                uow.Commit();
            }
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StudentSchoolYearDataAccess(uow).Update(studentSchoolYears);
                uow.Commit();
            }
        }
    }
}
