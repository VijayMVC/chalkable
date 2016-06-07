﻿using System.Collections.Generic;
using System.Data.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class SupplementalAnnouncementRecipientDataAccess : DataAccessBase<SupplementalAnnouncementRecipient, int>
    {
        public SupplementalAnnouncementRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void UpdateRecipients(IList<SupplementalAnnouncementRecipient> entities)
        {
            Delete(entities);
            Insert(entities);
        }

        private SupplementalAnnouncementRecipient ReadSupplementalAnnouncementRecipient(DbDataReader reader)
        {
            var announcement = reader.Read<SupplementalAnnouncementRecipient>();
            announcement.Recipient = reader.Read<Person>();

            return announcement;
        }

        public void DeleteByAnnouncementId(int id)
        {
            SimpleDelete<SupplementalAnnouncementRecipient>(new AndQueryCondition
            {
                {SupplementalAnnouncementRecipient.SUPPLEMENTAL_ANNONCEMENT_REF_FIELD, id }
            });
        }

        public IList<SupplementalAnnouncementRecipient> GetRecipientsByAnnouncementIds(IList<int> ids)
        {
            var query = $@"Select * 
                           From {nameof(SupplementalAnnouncementRecipient)} join {nameof(Person)}
                                on {SupplementalAnnouncementRecipient.STUDENT_REF_FIELD} = {Person.ID_FIELD}
                           Where {SupplementalAnnouncementRecipient.SUPPLEMENTAL_ANNONCEMENT_REF_FIELD} in(Select * From @ids)";
            var @params = new Dictionary<string, object>
            {
                ["ids"] = ids
            };

            using (var reader = ExecuteReaderParametrized(query, @params))
            {
                var res = new List<SupplementalAnnouncementRecipient>();
                while (reader.Read())
                    res.Add(ReadSupplementalAnnouncementRecipient(reader));

                return res;
            }
        }

        public IList<SupplementalAnnouncementRecipient> GetRecipientsByAnnouncementId(int id)
        {
            return GetRecipientsByAnnouncementIds(new List<int> {id});
        } 
    }
}
