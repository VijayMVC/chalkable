using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentParentDataAccess : DataAccessBase<StudentParent>
    {
        public StudentParentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(StudentParentQuery query)
        {
            SimpleDelete<StudentParent>(BuildConditions(query));    
        }



        public IList<StudentParentDetails> GetParents(Guid studentId, Guid callerId, int callerRoleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"studentId", studentId},
                    {"callerId", callerId},
                    {"callerRoleId", callerRoleId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetParents", parameters))
            {
                var res = reader.ReadList<StudentParentDetails>();
                if (res != null)
                {
                    var parents = new List<PersonDetails>();
                    for (int i = 0; i < res.Count; i++)
                    {
                        reader.NextResult();
                        reader.Read();
                        parents.Add(PersonDataAccess.ReadPersonDetailsData(reader));
                    }
                    if (parents.Count > 0)
                    {
                        foreach (var studentParent in res)
                        {
                            studentParent.Parent = parents.FirstOrDefault(x => x.Id == studentParent.ParentRef);
                        }
                    }
                }
                return res;
            }
        }


        private QueryCondition BuildConditions(StudentParentQuery query)
        {
            var res = new AndQueryCondition();
            if(query.Id.HasValue)
                res.Add("Id", query.Id);
            if(query.ParentId.HasValue)
                res.Add("ParentRef", query.ParentId);
            if(query.StudentId.HasValue)
                res.Add("StudentRef", query.StudentId);
            return res;
        } 
    }

    public class StudentParentQuery
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? StudentId { get; set; }
    }
}
