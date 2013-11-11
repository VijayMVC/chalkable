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
        AttendanceReason Edit(Guid id, AttendanceTypeEnum type, string code, string description);
        void Delete(Guid id);
        IList<AttendanceReason> List();
        AttendanceReason Get(Guid id);
        void AddAttendanceLevelReasons(List<AttendanceLevelReason> absebceLevelReasons);
    }

    public class AttendanceReasonService : SchoolServiceBase, IAttendanceReasonService
    {
        public AttendanceReasonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //public AttendanceReason Add(AttendanceTypeEnum type, string code, string description)
        //{
        //    if(!BaseSecurity.IsAdminEditor(Context))
        //        throw new ChalkableSecurityException();

        //    using (var uow = Update())
        //    {
        //        var res = new AttendanceReason
        //            {
        //                Id = Guid.NewGuid(),
        //                AttendanceType = type,
        //                Code = code,
        //                Description = description
        //            };
        //        new AttendanceReasonDataAccess(uow).Insert(res);
        //        uow.Commit();
        //        return res;
        //    }
        //}

        //public AttendanceReason Edit(Guid id, AttendanceTypeEnum type, string code, string description)
        //{
        //    if (!BaseSecurity.IsAdminEditor(Context))
        //        throw new ChalkableSecurityException();
        //    using (var uow = Update())
        //    {
        //        var da = new AttendanceReasonDataAccess(uow);
        //        var res =  da.GetById(id);
        //        res.Code = code;
        //        res.Description = description;
        //        res.AttendanceType = type;
        //        da.Update(res);
        //        uow.Commit();
        //        return res;
        //    }
        //}

        //public void Delete(Guid id)
        //{
        //    if (!BaseSecurity.IsAdminEditor(Context))
        //        throw new ChalkableSecurityException();
        //    using (var uow = Update())
        //    {
        //        new AttendanceReasonDataAccess(uow).Delete(id);
        //        uow.Commit();
        //    }
        //}

        //public IList<AttendanceReason> List()
        //{
        //    using (var uow = Read())
        //    {
        //      return  new AttendanceReasonDataAccess(uow).GetAll();
        //    }
        //}

        //public AttendanceReason Get(Guid id)
        //{
        //    using (var uow = Read())
        //    {
        //        return new AttendanceReasonDataAccess(uow).GetById(id);
        //    }
        //}

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

        public AttendanceReason Edit(Guid id, AttendanceTypeEnum type, string code, string description)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
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

        public AttendanceReason Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void AddAttendanceLevelReasons(List<AttendanceLevelReason> absebceLevelReasons)
        {
            using (var uow = Update())
            {
                var da = new AttendanceLevelReasonDataAccess(uow);
                da.Insert(absebceLevelReasons);
                uow.Commit();
            }
        }
    }
}
