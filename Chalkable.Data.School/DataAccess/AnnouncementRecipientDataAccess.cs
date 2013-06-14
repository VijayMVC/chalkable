﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataAccess : DataAccessBase
    {
        public AnnouncementRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(AnnouncementRecipient announcementRecipient)
        {
            SimpleInsert(announcementRecipient);
        }

        public void Create(IList<AnnouncementRecipient> announcementRecipients)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid announcementId)
        {
            var conds = new Dictionary<string, object> {{"announcementRef", announcementId}};
            SimpleDelete<AnnouncementRecipient>(conds);
        }
    }
}
