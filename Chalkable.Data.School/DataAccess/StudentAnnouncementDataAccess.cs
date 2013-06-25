using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementDataAccess : DataAccessBase<StudentAnnouncement>
    {
        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string DROP_PARAM = "drop";

        public void Update(Guid announcementId, bool drop)
        {
            var conds = new Dictionary<string, object> { { StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, announcementId } };
            var updateParams = new Dictionary<string, object> {{DROP_PARAM, drop}};
            SimpleUpdate<StudentAnnouncement>(updateParams, conds);
        }
    }
}
