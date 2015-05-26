using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GroupDataAccess : DataAccessBase<Group, int>
    {
        public GroupDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GroupDetails> GetGroupsDetails(int ownerId)
        {
            var groupType = typeof (Group);
            var studentType = typeof (Student);
            var conds = new AndQueryCondition { { Group.OWNER_REF_FIELD, ownerId } };
            var groupQuery = Orm.SimpleSelect(groupType.Name, conds);
            
            var studentGroupsQuery = new DbQuery();
            var types = new List<Type> {groupType, studentType};
            studentGroupsQuery.Sql
                .AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), groupType.Name)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, typeof (StudentGroup).Name, StudentGroup.GROUP_REF_FIELD, groupType.Name, Group.ID_FIELD)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, studentType.Name, Student.ID_FIELD, typeof (StudentGroup).Name, StudentGroup.STUDENT_REF_FIELD);
            conds.BuildSqlWhere(studentGroupsQuery, groupType.Name);

            var dbQuery = new DbQuery(new List<DbQuery> {groupQuery, studentGroupsQuery});
            return Read(dbQuery, ReadGetGroupDetailsResult);
        }

        private IList<GroupDetails> ReadGetGroupDetailsResult(DbDataReader reader)
        {
            var groups = reader.ReadList<Group>();
            reader.NextResult();
            var studentGroups = reader.ReadList<StudentGroupComplex>(true);
            return groups.Select(chlkGroup => new GroupDetails
                {
                    Id = chlkGroup.Id, 
                    Name = chlkGroup.Name, 
                    OwnerRef = chlkGroup.OwnerRef, 
                    Students = studentGroups.Where(x => x.Group.Id == chlkGroup.Id).Select(x => x.Student).ToList()
                }).ToList();
        } 
    }
}
