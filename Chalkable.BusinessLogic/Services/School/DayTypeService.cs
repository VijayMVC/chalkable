using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDayTypeService
    {
        IList<DayType> GetSections(int schoolYearId);
        DayType GetSectionById(int id);
        
        void Add(IList<DayType> dayTypes); 
        void Edit(IList<DayType> dayTypes);
        void Delete(IList<int> ids);
    }

    public class DayTypeService : SchoolServiceBase, IDayTypeService
    {
        public DayTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public IList<DayType> GetSections(int schoolYearId)
        {
            using (var uow = Read())
            {
                return new DayTypeDataAccess(uow).GetDateTypes(schoolYearId);
            }
        }

        //TODO : filter by school 
        public DayType GetSectionById(int id)
        {
            using (var uow = Read())
            {
                return new DayTypeDataAccess(uow).GetById(id);
            }
        }

        public void Add(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u=>new DayTypeDataAccess(u).Insert(dayTypes));
        }

        public void Edit(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DayTypeDataAccess(u).Update(dayTypes));
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DayTypeDataAccess(u).Delete(ids));
        }
    }
}
