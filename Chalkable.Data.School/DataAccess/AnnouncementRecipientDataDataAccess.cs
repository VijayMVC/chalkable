﻿using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataDataAccess : DataAccessBase<AnnouncementRecipientData, int>
    {
        public AnnouncementRecipientDataDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void UpdateAnnouncementRecipientData(int? announcementId, int announcementType, int?schoolYearId, int? personId, int? roleId, 
            bool complete, int? classId, DateTime? fromDate, DateTime? toDate)
        {
            var param = new Dictionary<string, object>
            {
                [nameof(announcementId)] = announcementId,
                [nameof(complete)] = complete,
                [nameof(personId)] = personId,
                [nameof(roleId)] = roleId,
                [nameof(schoolYearId)] = schoolYearId,
                [nameof(classId)] = classId,
                [nameof(announcementType)] = announcementType,
                [nameof(fromDate)] = fromDate,
                [nameof(toDate)] = toDate
            };

            ExecuteStoredProcedure("spUpdateAnnouncementRecipientData", param);
        }
    }
}
