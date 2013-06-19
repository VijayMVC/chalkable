using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassPeriodService
    {
        ClassPeriod Add(Guid periodId, Guid classId, Guid roomId);
        void Delete(Guid id);
        IList<ClassPeriod> GetClassPeriods(Guid markingPeriodId, Guid? classId, Guid? roomId, Guid? periodId, Guid? sectionId, Guid? studentId = null, Guid? teacherId = null, Guid? time = null);
        IList<Class> GetAvailableClasses(Guid periodId);
        IList<Room> GetAvailableRooms(Guid periodId);

        ClassPeriod GetClassPeriodForSchoolPersonByDate(Guid personId, DateTime dateTime);
        ClassPeriod GetNearestClassPeriod(Guid? classId, DateTime dateTime);
        IList<ClassPeriod> GetClassPeriods(DateTime date, Guid? classId, Guid? roomId, Guid? studentId, Guid? teacherId, Guid? time = null);
  
    }

    public class ClassPeriodService : SchoolServiceBase, IClassPeriodService
    {
        public ClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public ClassPeriod Add(Guid periodId, Guid classId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<ClassPeriod> GetClassPeriods(Guid markingPeriodId, Guid? classId, Guid? roomId, Guid? periodId, Guid? sectionId,
                                     Guid? studentId = null, Guid? teacherId = null, Guid? time = null)
        {
            throw new NotImplementedException();
        }

        public IList<Class> GetAvailableClasses(Guid periodId)
        {
            throw new NotImplementedException();
        }

        public IList<Room> GetAvailableRooms(Guid periodId)
        {
            throw new NotImplementedException();
        }

        public ClassPeriod GetClassPeriodForSchoolPersonByDate(Guid personId, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public ClassPeriod GetNearestClassPeriod(Guid? classId, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IList<ClassPeriod> GetClassPeriods(DateTime date, Guid? classId, Guid? roomId, Guid? studentId, Guid? teacherId, Guid? time = null)
        {
            throw new NotImplementedException();
        }
    }
}
