using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementQnADataAccess : DataAccessBase<AnnouncementQnA>
    {
        public AnnouncementQnADataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        private const string GET_ANNOUNCEMENT_QNA_PROCEDURE = "spGetAnnouncementsQnA";
        private const string CALLER_ID_PARAM = "callerId";
        private const string ASKER_ID_PARAM = "askerId";
        private const string ANSWERER_ID_PARAM = "answererId";
        private const string ANNOUNCEMENT_ID_PARAM = "announcementId";
        private const string ANNOUNCEMENT_QNA_ID_PARAM = "announcementQnAId";
        
        public AnnouncementQnAQueryResult GetAnnouncementQnA(AnnouncementQnAQuery query)
        {
            var parameter = new Dictionary<string, object>
                {
                    {ANNOUNCEMENT_QNA_ID_PARAM, query.Id},
                    {ANNOUNCEMENT_ID_PARAM, query.AnnouncementId},
                    {ASKER_ID_PARAM, query.AskerId},
                    {ANSWERER_ID_PARAM, query.AnswererId},
                    {CALLER_ID_PARAM, query.CallerId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ANNOUNCEMENT_QNA_PROCEDURE, parameter))
            {
                var res = ReadAnnouncementQnAComplexes(reader);
                return new AnnouncementQnAQueryResult 
                    {
                        AnnouncementQnAs = res,
                        Query = query
                    };
            }
        }

        public static IList<AnnouncementQnAComplex> ReadAnnouncementQnAComplexes(DbDataReader reader)
        {
            var res = new List<AnnouncementQnAComplex>();
            while (reader.Read())
            {
                res.Add(ReadAnnouncementQnAComplex(reader));
            }
            return res;
        }
        public static AnnouncementQnAComplex ReadAnnouncementQnAComplex(DbDataReader reader)
        {
            var annQnA = reader.Read<AnnouncementQnAComplex>();
            annQnA.Asker = ReadAnnouncementQnAPerson(reader, "Asker");
            annQnA.Answerer = ReadAnnouncementQnAPerson(reader, "Answerer");
            return annQnA;
        }
        private static Person ReadAnnouncementQnAPerson(DbDataReader reader, string prefix)
        {
            var template = prefix + "{0}";
            return new Person
                {
                    Id = SqlTools.ReadGuid(reader, string.Format(template, Person.ID_FIELD)),
                    FirstName = SqlTools.ReadStringNull(reader, string.Format(template, Person.FIRST_NAME_FIELD)),
                    LastName = SqlTools.ReadStringNull(reader, string.Format(template, Person.LAST_NAME_FIELD)),
                    Gender = SqlTools.ReadStringNull(reader, string.Format(template, Person.GENDER_FIELD)),
                    RoleRef = SqlTools.ReadInt32(reader, string.Format(template, Person.ROLE_REF_FIELD))
                };
        } 
    }

    public class AnnouncementQnAQuery
    {
        public Guid? Id { get; set; }
        public Guid? AskerId { get; set; }
        public Guid? AnswererId { get; set; }
        public Guid? AnnouncementId { get; set; }
        public Guid CallerId { get; set; }
    }

    public class AnnouncementQnAQueryResult
    {
        public AnnouncementQnAQuery Query { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; }
    }
}
