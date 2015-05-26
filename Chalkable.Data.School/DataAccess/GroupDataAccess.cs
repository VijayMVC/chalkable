using System;
using System.Collections.Generic;
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
            var dbQuery = new DbQuery();
            var groupType = typeof (Group);
            var studentType = typeof (Student);
            var types = new List<Type> {groupType, studentType};
            dbQuery.Sql
                .AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), groupType.Name)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, typeof (StudentGroup).Name, StudentGroup.GROUP_REF_FIELD, groupType.Name, Group.ID_FIELD)
                .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, studentType.Name, Student.ID_FIELD, typeof (StudentGroup).Name, StudentGroup.STUDENT_REF_FIELD);

            new AndQueryCondition {{Group.OWNER_REF_FIELD, ownerId}}.BuildSqlWhere(dbQuery, groupType.Name);

            var studentGroups= ReadMany<StudentGroupComplex>(dbQuery, true).ToList();
            return studentGroups.GroupBy(x => x.Group.Id).Select(x =>
                {
                    var stgroup = x.First();
                    return new GroupDetails
                        {
                            Id = x.Key,
                            Name = stgroup.Group.Name,
                            OwnerRef = stgroup.Group.OwnerRef,
                            Students = x.Select(sg => sg.Student).ToList()
                        };
                }).ToList();
        }
    }
}
