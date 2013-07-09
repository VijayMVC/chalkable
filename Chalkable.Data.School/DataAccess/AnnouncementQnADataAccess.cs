using System;
using System.Collections.Generic;
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
                var res = new List<AnnouncementQnAComplex>();
                while (reader.Read())
                {
                    var annQnA = reader.Read<AnnouncementQnAComplex>();
                    annQnA.Asker = new Person
                        {
                            Id = SqlTools.ReadGuid(reader, "AskerId"),
                            FirstName = SqlTools.ReadStringNull(reader, "AskerFirstName"),
                            LastName = SqlTools.ReadStringNull(reader, "AskerLastName"),
                            Gender = SqlTools.ReadStringNull(reader, "AskerGender")
                        };
                    annQnA.Answerer = new Person
                        {
                            Id = SqlTools.ReadGuid(reader, "AnswererId"),
                            FirstName = SqlTools.ReadStringNull(reader, "AnswererFirstName"),
                            LastName = SqlTools.ReadStringNull(reader, "AnswererLastName"),
                            Gender = SqlTools.ReadStringNull(reader, "AnswererGender")
                        };
                    res.Add(annQnA);
                }
                return new AnnouncementQnAQueryResult 
                    {
                        AnnouncementQnAs = res,
                        Query = query
                    };
            }
        } 

        public static AnnouncementQnAComplex ReadAnnouncementQnAComplex(SqlDataReader reader)
        {
            var annQnA = reader.Read<AnnouncementQnAComplex>();
            annQnA.Asker = new Person
            {
                Id = SqlTools.ReadGuid(reader, "AskerId"),
                FirstName = SqlTools.ReadStringNull(reader, "AskerFirstName"),
                LastName = SqlTools.ReadStringNull(reader, "AskerLastName"),
                Gender = SqlTools.ReadStringNull(reader, "AskerGender")
            };
            annQnA.Answerer = new Person
            {
                Id = SqlTools.ReadGuid(reader, "AnswererId"),
                FirstName = SqlTools.ReadStringNull(reader, "AnswererFirstName"),
                LastName = SqlTools.ReadStringNull(reader, "AnswererLastName"),
                Gender = SqlTools.ReadStringNull(reader, "AnswererGender")
            };
            return annQnA;
        }

        public static IList<AnnouncementQnAComplex> ReadAnnouncementQnAComplexes(SqlDataReader reader)
        {
            var res = new List<AnnouncementQnAComplex>();
            while (reader.Read())
            {
                res.Add(ReadAnnouncementQnAComplex(reader));
            }
            return res;
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
