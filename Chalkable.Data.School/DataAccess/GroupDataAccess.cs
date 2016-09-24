using System;
using System.Collections.Generic;
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

        public override void Delete(int key)
        {
            var dbQuery = new DbQuery(new List<DbQuery>
                {
                    Orm.SimpleDelete<AnnouncementGroup>(new AndQueryCondition{{AnnouncementGroup.GROUP_REF_FIELD, key}}),
                    Orm.SimpleDelete<StudentGroup>(new AndQueryCondition{{StudentGroup.GROUP_REF_FIELD, key}}),
                    Orm.SimpleDelete<Group>(new AndQueryCondition{{Group.ID_FIELD, key}})
                });
            ExecuteNonQueryParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters);
        }


        public IList<Group> GetAll(QueryCondition conditions = null, string filter = null)
        {
            var query = new DbQuery();
            var groupTName = typeof (Group).Name;
            var querySet = string.Format(" [{0}].*, (select count(*) from [{3}] where [{3}].[{4}] = [{1}]) as {2}"
                                        , groupTName, Group.ID_FIELD, Group.STUDENT_COUNT_FIELD
                                        , typeof (StudentGroup).Name, StudentGroup.GROUP_REF_FIELD);
            query.Sql.AppendFormat(Orm.SELECT_FORMAT, querySet, groupTName);
            if(conditions != null)
                conditions.BuildSqlWhere(query, groupTName);

            if (!string.IsNullOrEmpty(filter))
            {
                query.Sql.AppendFormat(" AND (");
                string[] sl = filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sl.Length; i++)
                {
                    query.Parameters.Add("@filter" + i, string.Format(FILTER_FORMAT, sl[i]));
                    query.Sql.AppendFormat($" [{typeof(Group).Name}].[{nameof(Group.Name)}] like @filter" + i + " OR");
                }
                query.Sql.Remove(query.Sql.Length - 2, 2);
                query.Sql.AppendFormat(")");
            }

            return ReadMany<Group>(query);
        }

        public GroupInfo GetGroutInfo(int groupId)
        {
            var query = $@"
                select * from [{nameof(Group)}] where [{nameof(Group)}].[{nameof(Group.Id)}] = @groupId
                select * from [{nameof(Student)}] 
                where [{nameof(Student)}].[{nameof(Student.Id)}] 
                    in (select [{nameof(StudentGroup)}].[{nameof(StudentGroup.StudentRef)}]
                            from [{nameof(StudentGroup)}] 
                            where [{nameof(StudentGroup)}].[{nameof(StudentGroup.GroupRef)}] = @groupId)";

            
            using (var reader = ExecuteReaderParametrized(query, new Dictionary<string, object> { { "@groupId", groupId }}))
            {
                reader.Read();
                var groupInfo = reader.Read<GroupInfo>();
                reader.NextResult();
                groupInfo.Students = reader.ReadList<Student>();
                return groupInfo;
            }
        }
    }
}
