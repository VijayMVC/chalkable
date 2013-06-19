using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolYearService
    {
        SchoolYear Add(string name,string description, DateTime startDate, DateTime endDate);
        SchoolYear Edit(Guid id, string name, string description, DateTime startDate, DateTime endDate);
        SchoolYear GetSchoolYearById(Guid id);
        PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue);
        void Delete(Guid schoolYearId);
        SchoolYear GetCurrentSchoolYear();
        IList<SchoolYear> GetSortedYears();
    }

    public class SchoolYearService : SchoolServiceBase, ISchoolYearService
    {
        public SchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs test 
        public SchoolYear Add(string name, string description, DateTime startDate, DateTime endDate)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow);
                if(IsOverlaped(startDate, endDate))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_OVERLAPPING_DATA);
                if (da.Exists(name))
                    throw new ChalkableException(ChlkResources.ERR_SCHOOL_YEAR_ALREADY_EXISTS);
                
                var schoolYear = new SchoolYear
                    {
                        Id = Guid.NewGuid(),
                        Description = description,
                        Name = name,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                
                da.Create(schoolYear);
                uow.Commit();
                return schoolYear;
            }
        }
        
        private bool IsOverlaped(DateTime startDate, DateTime endDate, SchoolYearDataAccess dataAccess = null, SchoolYear schoolYear = null)
        {
            return startDate >= endDate || (schoolYear != null &&  dataAccess != null 
                        && (dataAccess.IsOverlaped(startDate, endDate, schoolYear)));
        }

        public SchoolYear Edit(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            if(BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow);
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

        public SchoolYear GetSchoolYearById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                return da.GetById(id);
            }
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                return da.GetSchoolYears(start, count);
            }
        }

        public void Delete(Guid schoolYearId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new SchoolYearDataAccess(uow);
                da.Delete(schoolYearId);
                uow.Commit();
            }
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            var nowDate = Context.NowSchoolTime;
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                return da.GetByDate(nowDate);
            }
        }
        public IList<SchoolYear> GetSortedYears()
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                return da.GetSchoolYears();
            }
        }
    }
}
