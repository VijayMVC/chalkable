using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementCommentDataAccess : DataAccessBase<AnnouncementComment, int>
    {
        public AnnouncementCommentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementComment> GetList(int announcementId, int callerId, int roleId)
        {
            var ps = new Dictionary<string, object>
            {
                ["announcementId"] = announcementId,
                ["callerId"] = callerId,
                ["roleId"] = roleId
            };
            return ExecuteStoredProcedureList<AnnouncementComment>("spGetAnnouncementComments", ps);
        } 
    }
}
