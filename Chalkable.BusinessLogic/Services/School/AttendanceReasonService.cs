using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceReasonService
    {
        void Add(IList<AttendanceReason> reasons);
        void Edit(IList<AttendanceReason> reasons);
        void Delete(IList<int> ids);
        IList<AttendanceReason> List(bool onlyWithLevel = true);
        AttendanceReason Get(int id);
        void AddAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons);
        void EditAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons);
        void DeleteAttendanceLevelReasons(IList<int> ids);
        IList<AttendanceReason> GetAll();
    }

    public class AttendanceReasonService : SchoolServiceBase, IAttendanceReasonService
    {
        public AttendanceReasonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public void Add(IList<AttendanceReason> reasons)
        {
            if(!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AttendanceReasonDataAccess(uow);
                da.Insert(reasons);
                uow.Commit();
            }
        }

        public IList<AttendanceReason> List(bool onlyWithLevel = true)
        {
            using (var uow = Read())
            {
                var da = new AttendanceReasonDataAccess(uow);
                var res = da.GetAll();
                if (onlyWithLevel)
                    res = res.Where(x => x.AttendanceLevelReasons.Count > 0).ToList();
                return res;
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


        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {   
                new AttendanceReasonDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }


        public void Edit(IList<AttendanceReason> reasons)
        {
            if(!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AttendanceReasonDataAccess(uow).Update(reasons);
                uow.Commit();
            }
        }


        public void EditAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AttendanceLevelReasonDataAccess(uow).Update(attendanceLevelReasons);
                uow.Commit();
            }
        }


        public void DeleteAttendanceLevelReasons(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AttendanceLevelReasonDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }
    }
}
