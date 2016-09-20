using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
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

        private const string SP_GET_GROUP_EXPLORER_DATA =  "spGetGroupExplorerData";
        private const string GROUP_ID_PARAM = "groupId";
        private const string OWNER_ID_PARAM = "ownerId";
        private const string CURRENT_DATE_PARAM = "currentDate";
        
        public GroupExplorer GetGroupExplorerData(int groupId, int ownerId, DateTime currentDate)
        {
            var parameters = new Dictionary<string, object>
                {
                    {GROUP_ID_PARAM, groupId},
                    {OWNER_ID_PARAM, ownerId},
                    {CURRENT_DATE_PARAM, currentDate}
                };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_GROUP_EXPLORER_DATA, parameters))
            {
                var res = new GroupExplorer();
                reader.Read();
                res.Group = reader.Read<Group>();
                reader.NextResult();
                res.Schools = reader.ReadList<SchoolForGroup>();
                reader.NextResult();
                res.GradeLevels = reader.ReadList<GradeLevel>();
                reader.NextResult();
                res.GroupMembers = reader.ReadList<GroupMember>().ToList();
                return res;
            }
        }

        private const string SP_SEARCH_STUDENTS_FOR_GROUP = "spSearchStudentsForGroup";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string GRADE_LEVEL_ID_PARAM = "gradeLevelId";
        private const string CLASSES_IDS_PARAM = "classesIds";
        private const string COURSES_IDS_PARAM = "coursesIds";

        public IList<StudentForGroup> GetStudentForGroup(int groupId, int schoolYearId, int gradeLevelId, IList<int> classesIds, IList<int> coursesIds)
        {
            var parametes = new Dictionary<string, object>
                {
                    {GROUP_ID_PARAM, groupId},
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {GRADE_LEVEL_ID_PARAM, gradeLevelId},
                    {CLASSES_IDS_PARAM, classesIds ?? new List<int>()},
                    {COURSES_IDS_PARAM, coursesIds ?? new List<int>()}
                };
           return ExecuteStoredProcedureList<StudentForGroup>(SP_SEARCH_STUDENTS_FOR_GROUP, parametes);
        }

        public IList<int> GetStudentsByGroups(IList<int> groupIds)
        {
            if (groupIds == null || groupIds.Count == 0) return new List<int>();

            var sql = new StringBuilder();
            sql.AppendFormat(Orm.SELECT_FORMAT, $" distinct {StudentGroup.STUDENT_REF_FIELD}", nameof(StudentGroup));
            sql.Append($" Where {StudentGroup.GROUP_REF_FIELD} In (Select * From @groupIds)");

            var param = new Dictionary<string, object>
            {
                ["groupIds"] = groupIds
            };

            using (var reader = ExecuteReaderParametrized(sql.ToString(), param))
            {
                var res = new List<int>();
                while (reader.Read())
                    res.Add(SqlTools.ReadInt32(reader, StudentGroup.STUDENT_REF_FIELD));
                
                return res;
            }
        } 
    }


    public class StudentGroupDataAccess : DataAccessBase<StudentGroup, int>
    {
        public StudentGroupDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string SP_ASSIGN_ALL_TO_GROUP = "spAssigAllToGroup";
        private const string NOW_PARAM = "now";
        private const string GROUP_ID_PARAM = "groupId";

        public void AssignAllStudentsToGroup(int groupId, DateTime now)
        {
            var parameter = new Dictionary<string, object>
                {
                    {GROUP_ID_PARAM, groupId},
                    {NOW_PARAM, now}
                };
            ExecuteStoredProcedure(SP_ASSIGN_ALL_TO_GROUP, parameter);
        }

        public void UnassignAllStudentsFromGroup(int groupId)
        {
            SimpleDelete<StudentGroup>(new AndQueryCondition {{StudentGroup.GROUP_REF_FIELD, groupId}});
        } 

        public void AssignStudentsBySchoolYear(int groupId, int schoolYearId, int? gradeLevelId)
        {
            var query = new DbQuery();
            query.Sql.AppendFormat("insert into [{0}] ", typeof (StudentGroup).Name)
                 .AppendFormat(@"select @groupId, StudentRef from [{0}] ", typeof (StudentSchoolYear).Name);
            var conds = BuildCondsForStudentSchoolYearSelect(schoolYearId, gradeLevelId);
           conds.BuildSqlWhere(query, typeof(StudentSchoolYear).Name);
            query.Sql.AppendFormat(" and StudentRef not in (select StudentRef from StudentGroup where GroupRef =@groupId)");
            query.Parameters.Add("groupId", groupId);
            ExecuteNonQueryParametrized(query.Sql.ToString(), query.Parameters);
        }

        public void UnassignStudentsBySchoolYear(int groupId, int schoolYearId, int? gradeLevelId)
        {
            var query = Orm.SimpleDelete<StudentGroup>(new AndQueryCondition {{StudentGroup.GROUP_REF_FIELD, groupId}});
            var ssyQuery = new DbQuery();
            ssyQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, StudentSchoolYear.STUDENT_FIELD_REF_FIELD, typeof (StudentSchoolYear).Name);

            BuildCondsForStudentSchoolYearSelect(schoolYearId, gradeLevelId).BuildSqlWhere(ssyQuery, typeof (StudentSchoolYear).Name);

            query.Sql.AppendFormat(" and [{0}] in ({1})", StudentGroup.STUDENT_REF_FIELD, ssyQuery.Sql);
            query.AddParameters(ssyQuery.Parameters);
            ExecuteNonQueryParametrized(query.Sql.ToString(), query.Parameters);
        }

        private QueryCondition BuildCondsForStudentSchoolYearSelect(int schoolYearId, int? gradeLevelId)
        {
            var conds = new AndQueryCondition
                {
                    {StudentSchoolYear.SCHOOL_YEAR_REF_FIELD, schoolYearId},
                    {StudentSchoolYear.ENROLLMENT_STATUS_FIELD, 0}
                };
            if (gradeLevelId.HasValue)
                conds.Add(StudentSchoolYear.GRADE_LEVEL_REF_FIELD, gradeLevelId);
            return conds;
        }
    }
}
