using System;
using System.Collections.Generic;
using System.Linq;
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
                    {CLASSES_IDS_PARAM, classesIds != null ? classesIds.Select(x=>x.ToString()).JoinString(",") : null},
                    {COURSES_IDS_PARAM, coursesIds != null ? coursesIds.Select(x=>x.ToString()).JoinString(",") : null}
                };
           return ExecuteStoredProcedureList<StudentForGroup>(SP_SEARCH_STUDENTS_FOR_GROUP, parametes);
        }
        
        private const string SP_ASSIGN_ALL_TO_GROUP = "spAssigAllToGroup";
        private const string NOW_PARAM = "now";

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
    }
}
