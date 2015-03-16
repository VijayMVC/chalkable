using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoAttendanceReasonService : DemoSchoolServiceBase, IAttendanceReasonService
    {
        public DemoAttendanceReasonService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }


        public void Add(IList<AttendanceReason> reasons)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.AttendanceReasonStorage.Add(reasons);
        }

        public void Edit(IList<AttendanceReason> reasons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.AttendanceReasonStorage.Update(reasons);
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.AttendanceReasonStorage.Delete(ids);
        }

        public IList<AttendanceReason> List(bool onlyWithLevel = true)
        {
            var res = Storage.AttendanceReasonStorage.GetAll();
            if (onlyWithLevel)
                res = res.Where(x => x.AttendanceLevelReasons.Count > 0).ToList();
            return res;
        }

        public AttendanceReason Get(int id)
        {
            return Storage.AttendanceReasonStorage.GetById(id);
        }

        public void AddAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            Storage.AttendanceLevelReasonStorage.Add(attendanceLevelReasons);
        }

        public void EditAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.AttendanceLevelReasonStorage.Update(attendanceLevelReasons);
        }

        public void DeleteAttendanceLevelReasons(IList<int> ids)
        {
              if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.AttendanceLevelReasonStorage.Delete(ids);
        }

        public IList<AttendanceReason> GetAll()
        {
            return Storage.AttendanceReasonStorage.GetAll();
            
        }
    }
}
