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

        private const string SP_GET_GROUP_EXPLORER_DATA =  "spGetGroupExplorerData";
        private const string GROUP_ID_PARAM = "groupId";
        private const string OWNER_ID_PARAM = "ownerId";
        private const string CURRENT_DATE_PARAM = "currentDate";
        
        public GroupExplorer GetGroupExplorerData(int groupId, int ownerId, DateTime currentDate)
        {
            var parameters = new Dictionary<string, object>
                {
                    {GROUP_ID_PARAM, groupId},
                    {OWNER_ID_PARAM, OWNER_ID_PARAM},
                    {CURRENT_DATE_PARAM, currentDate}
                };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_GROUP_EXPLORER_DATA, parameters))
            {
                var res = new GroupExplorer();
                reader.Read();
                res.Group = reader.Read<Group>();
                reader.NextResult();
                res.Schools = reader.ReadList<Model.School>();
                reader.NextResult();
                res.GradeLevels = reader.ReadList<GradeLevel>();
                reader.NextResult();
                res.GroupMembers = reader.ReadList<GroupMember>();
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
                    {CLASSES_IDS_PARAM, classesIds},
                    {COURSES_IDS_PARAM, coursesIds}
                };
            return ExecuteStoredProcedureList<StudentForGroup>(SP_SEARCH_STUDENTS_FOR_GROUP, parametes);
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


    public class StudentGroupDataAccess: DataAccessBase<StudentGroup, int>
    {
        public StudentGroupDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void ReCreateStudentGroups(int groupId, IList<int> studentIds)
        { 
            var deleteQuery = Orm.SimpleDelete<StudentGroup>(new AndQueryCondition {{StudentGroup.GROUP_REF_FIELD, groupId}});
            var studentGroups = studentIds.Select(x => new StudentGroup {StudentRef = x, GroupRef = groupId}).ToList();
            var insertQuery = Orm.SimpleInsert<IList<StudentGroup>>(studentGroups);
            var dbQuery = new DbQuery(new List<DbQuery> {deleteQuery, insertQuery});
            ExecuteNonQueryParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters);
        }
    }
}
