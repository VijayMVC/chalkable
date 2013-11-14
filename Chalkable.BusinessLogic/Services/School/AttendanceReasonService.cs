using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceReasonService
    {
        void Add(IList<AttendanceReason> reasons);
        void Delete(int id);
        IList<AttendanceReason> List();
        AttendanceReason Get(int id);
        void AddAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons);
        IList<AttendanceReason> GetAll();
    }

    public class AttendanceReasonService : SchoolServiceBase, IAttendanceReasonService
    {
        public AttendanceReasonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public void Add(IList<AttendanceReason> reasons)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AttendanceReasonDataAccess(uow);
                da.Insert(reasons);
                uow.Commit();
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IList<AttendanceReason> List()
        {
            using (var uow = Read())
            {
                var da = new AttendanceReasonDataAccess(uow);
                return da.GetAll();
            }
        }

        public AttendanceReason Get(int id)
        {
            using (var uow = Read())
            {
                return new AttendanceReasonDataAccess(uow).GetById(id);
            }
        }

        public void AddAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            using (var uow = Update())
            {
                var da = new AttendanceLevelReasonDataAccess(uow);
                da.Insert(attendanceLevelReasons);
                uow.Commit();
            }
        }

        public IList<AttendanceReason> GetAll()
        {
            using (var uow = Read())
            {
                return new AttendanceReasonDataAccess(uow).GetAll();
            }
        }
    }
}
