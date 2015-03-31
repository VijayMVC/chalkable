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
    public class DemoAttendanceReasonStorage : BaseDemoIntStorage<AttendanceReason>
    {
        public DemoAttendanceReasonStorage()
            : base(x => x.Id)
        {
        }

    }

    public class DemoAttendanceLevelReasonStorage : BaseDemoIntStorage<AttendanceLevelReason>
    {
        public DemoAttendanceLevelReasonStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<AttendanceLevelReason> GetForAttendanceReason(int attendanceReasonId)
        {
            return data.Where(x => x.Value.AttendanceReasonRef == attendanceReasonId).Select(x => x.Value).ToList();
        }
    }

    public class DemoAttendanceReasonService : DemoSchoolServiceBase, IAttendanceReasonService
    {
        private DemoAttendanceLevelReasonStorage AttendanceLevelReasonStorage { get; set; }
        private DemoAttendanceReasonStorage AttendanceReasonStorage { get; set; }

        public DemoAttendanceReasonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AttendanceLevelReasonStorage = new DemoAttendanceLevelReasonStorage();
            AttendanceReasonStorage = new DemoAttendanceReasonStorage();
        }

        public void Add(IList<AttendanceReason> reasons)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            AttendanceReasonStorage.Add(reasons);
        }

        public void Edit(IList<AttendanceReason> reasons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            AttendanceReasonStorage.Update(reasons);
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            AttendanceReasonStorage.Delete(ids);
        }

        public IList<AttendanceReason> List(bool onlyWithLevel = true)
        {
            var res = AttendanceReasonStorage.GetAll();
            if (onlyWithLevel)
                res = res.Where(x => x.AttendanceLevelReasons.Count > 0).ToList();
            return res;
        }

        public AttendanceReason Get(int id)
        {
            return AttendanceReasonStorage.GetById(id);
        }

        public void AddAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            AttendanceLevelReasonStorage.Add(attendanceLevelReasons);
        }

        public void EditAttendanceLevelReasons(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            AttendanceLevelReasonStorage.Update(attendanceLevelReasons);
        }

        public void DeleteAttendanceLevelReasons(IList<int> ids)
        {
              if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            AttendanceLevelReasonStorage.Delete(ids);
        }

        public IList<AttendanceReason> GetAll()
        {
            return AttendanceReasonStorage.GetAll();
            
        }
    }
}
