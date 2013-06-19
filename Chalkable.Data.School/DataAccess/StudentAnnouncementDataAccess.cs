using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementDataAccess : DataAccessBase
    {
        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string ANNOUNCEMENT_ID_FIELD = "announcementRef";
        private const string DROP_PARAM = "drop";

        public void Update(Guid announcementId, bool drop)
        {
            var conds = new Dictionary<string, object> {{ANNOUNCEMENT_ID_FIELD, announcementId}};
            var updateParams = new Dictionary<string, object> {{DROP_PARAM, drop}};
            SimpleUpdate<StudentAnnouncement>(updateParams, conds);
        }
    }
}
