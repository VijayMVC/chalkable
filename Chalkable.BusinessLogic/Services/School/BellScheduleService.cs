using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IBellScheduleService
    {
        void Add(IList<BellSchedule> bellSchedules);
        void Edit(IList<BellSchedule> bellSchedules);
        void Delete(IList<BellSchedule> bellSchedules);
        IList<BellSchedule> GetAll();
    }

    public class BellScheduleService : SchoolServiceBase, IBellScheduleService
    {
        public BellScheduleService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<BellSchedule> bellSchedules)
        {
            DoUpdate(u=>new BellScheduleDataAccess(u, null).Insert(bellSchedules));
        }

        public void Edit(IList<BellSchedule> bellSchedules)
        {
            DoUpdate(u => new BellScheduleDataAccess(u, null).Update(bellSchedules));
        }

        public void Delete(IList<BellSchedule> bellSchedules)
        {
            DoUpdate(u => new BellScheduleDataAccess(u, null).Delete(bellSchedules));
        }

        public IList<BellSchedule> GetAll()
        {
            return DoRead(u => new BellScheduleDataAccess(u, null).GetAll());
        }
    }
}