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
        SchoolYear Add(int id, string name,string description, DateTime startDate, DateTime endDate);
        SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate);
        SchoolYear GetSchoolYearById(int id);
        PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue);
        void Delete(int schoolYearId);
        SchoolYear GetCurrentSchoolYear();
        IList<SchoolYear> GetSortedYears();
    }

    public class SchoolYearService : SchoolServiceBase, ISchoolYearService
    {
        public SchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test 
        public SchoolYear Add(int id, string name, string description, DateTime startDate, DateTime endDate)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                if(IsOverlaped(startDate, endDate, da))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_OVERLAPPING_DATA);
                if (da.Exists(name))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_ALREADY_EXISTS);
                
                var schoolYear = new SchoolYear
                    {
                        Id = id,
                        Description = description,
                        Name = name,
                        StartDate = startDate,
                        EndDate = endDate,
                    };
                
                da.Insert(schoolYear);
                uow.Commit();
                return schoolYear;
            }
        }
        
        private bool IsOverlaped(DateTime startDate, DateTime endDate, SchoolYearDataAccess dataAccess, SchoolYear schoolYear = null)
        {
            var id = schoolYear != null ? schoolYear.Id : (int?) null;
            return startDate >= endDate || (dataAccess.IsOverlaped(startDate, endDate, id));
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

        public void Delete(int schoolYearId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                da.Delete(schoolYearId);
                uow.Commit();
            }
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
    }
}
