using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonDataAccess : BaseSchoolDataAccess<Person>
    {
        public PersonDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }
        
        private const string FILTER_FORMAT = "%{0}%";

        public void Delete(IList<int> ids)
        {
            if (ids.Count == 0)
                return;
            var res = new StringBuilder();
            var idsS = ids.Select(x => x.ToString()).JoinString(",");
            var sqlFormat = " delete from [{0}] where [{0}].[{1}] in ({2})";
            res.AppendFormat(sqlFormat, typeof(SchoolPerson).Name, SchoolPerson.PERSON_REF_FIELD, idsS);
            res.AppendFormat(sqlFormat, typeof(StudentSchoolYear).Name, StudentSchoolYear.STUDENT_FIELD_REF_FIELD, idsS);
            res.AppendFormat(sqlFormat, typeof(Person).Name, Person.ID_FIELD, idsS);
            ExecuteNonQueryParametrized(res.ToString(), new Dictionary<string, object>());
        }

        protected override QueryCondition FilterBySchool(QueryCondition queryCondition)
        {
            return queryCondition;
        }

        public Person GetPerson(PersonQuery query)
        {
            query.Count = 1;
            return GetPersons(query).Persons.First();
        }

        public bool Exists(string email, int? currentPersonId)
        {
            var conds = new AndQueryCondition {{Person.EMAIL_FIELD, email}};
            if(currentPersonId.HasValue)
                conds.Add(Person.ID_FIELD, currentPersonId, ConditionRelation.NotEqual);
            return Exists<Person>(conds);
        }

        public PersonQueryResult GetPersons(PersonQuery query)
        {
            var dbQuery = new DbQuery();
            var conditions = new AndQueryCondition();
            if(query.PersonId.HasValue)
                conditions.Add(Person.ID_FIELD, query.PersonId);
            if(query.RoleId.HasValue)
                conditions.Add(Person.ROLE_REF_FIELD, query.RoleId);
            if(!string.IsNullOrEmpty(query.StartFrom))
                conditions.Add(Person.LAST_NAME_FIELD, query.StartFrom, ConditionRelation.GreaterEqual);
            dbQuery.Sql.AppendFormat("select * from vwPerson ");

            if(query.CallerRoleId != CoreRoles.SUPER_ADMIN_ROLE.Id)
                conditions.Add(Person.SCHOOL_REF_FIELD, schoolId);
            conditions.BuildSqlWhere(dbQuery, "vwPerson");

            if (query.CallerRoleId == CoreRoles.DEVELOPER_ROLE.Id)
            {
                var orConds = new OrQueryCondition
                    {
                        {Person.ID_FIELD, query.CallerId}, 
                        {Person.ROLE_REF_FIELD, CoreRoles.STUDENT_ROLE.Id}
                    };
                dbQuery.Sql.Append(" and ");
                orConds.BuildSqlWhere(dbQuery, "vwPerson", false);
            }

            if (query.CallerRoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                var callerIdPrName = "callerId";
                dbQuery.Parameters.Add(callerIdPrName, query.CallerId);
                dbQuery.Sql.AppendFormat(" and ([{0}].[{1}] = @{2} ", "vwPerson", Person.ID_FIELD, callerIdPrName);
                var rolesIds = new List<int>
                    {
                        CoreRoles.ADMIN_EDIT_ROLE.Id,
                        CoreRoles.ADMIN_GRADE_ROLE.Id,
                        CoreRoles.ADMIN_VIEW_ROLE.Id
                    };
                if (query.OnlyMyTeachers)
                {
                    var innerSql = string.Format(@"select * from ClassTeacher 
                                                   join ClassPerson on ClassPerson.[{0}] = ClassTeacher.[{1}]
                                                   where ClassPerson.[{2}] = @{3} and ClassTeacher.[{4}] = vwPerson.[{5}]"
                        , ClassPerson.CLASS_REF_FIELD, ClassTeacher.CLASS_REF_FIELD, ClassPerson.PERSON_REF_FIELD
                        , callerIdPrName, ClassTeacher.PERSON_REF_FIELD, Person.ID_FIELD);
                    dbQuery.Sql.Append("or (")
                           .AppendFormat("[{0}].[{1}] = {2} ", "vwPerson", Person.ROLE_REF_FIELD, CoreRoles.TEACHER_ROLE.Id)
                           .AppendFormat(" and exists({0}))", innerSql);
                }
                else rolesIds.Add(CoreRoles.TEACHER_ROLE.Id);
                dbQuery.Sql.AppendFormat(" or ([{0}].[{1}] in ({2}))", "vwPerson", Person.ROLE_REF_FIELD
                    , rolesIds.Select(x=> x.ToString()).JoinString(","));


                var gls = new List<int>();
                if(query.CallerGradeLevelId.HasValue)
                    gls.Add(query.CallerGradeLevelId.Value);
                var stSchoolYearDbQuery = BuildStudentSchoolYearQuery(query.SchoolYearId, query.IsEnrolled, gls);
                dbQuery.Sql.Append("or (")
                       .AppendFormat("[{0}].[{1}] = {2} ", "vwPerson", Person.ROLE_REF_FIELD, CoreRoles.STUDENT_ROLE.Id)
                       .AppendFormat(" and exists({0}))", stSchoolYearDbQuery.Sql);
                dbQuery.AddParameters(stSchoolYearDbQuery.Parameters);
                dbQuery.Sql.Append(")");
            }

            dbQuery = BuildFilterConds(dbQuery, query);
            
            if (query.TeacherId.HasValue)
            {
                var cpDbQuery = new DbQuery();
                cpDbQuery.Sql.AppendFormat(@"select ClassPerson.[{0}] from ClassPerson
                                             join ClassTeacher on ClassPerson.[{1}] = ClassTeacher.[{2}]"
                                           , ClassPerson.PERSON_REF_FIELD, ClassPerson.CLASS_REF_FIELD, ClassTeacher.CLASS_REF_FIELD);

                new AndQueryCondition {{ClassTeacher.PERSON_REF_FIELD, "teacherId", query.TeacherId, ConditionRelation.Equal}}
                    .BuildSqlWhere(cpDbQuery, "ClassTeacher");
                cpDbQuery = BuildClassPersonConds(cpDbQuery, query, false);
                dbQuery.AddParameters(cpDbQuery.Parameters);
                dbQuery.Sql.AppendFormat(" and vwPerson.[{0}] in ({1})", Person.ID_FIELD, cpDbQuery.Sql);
            } 
            else if (query.ClassId.HasValue && (query.RoleId == CoreRoles.STUDENT_ROLE.Id || query.RoleId == CoreRoles.TEACHER_ROLE.Id))
            {
                var cpDbQuery = new DbQuery();
                if (query.RoleId == CoreRoles.STUDENT_ROLE.Id)
                {
                    cpDbQuery.Sql.AppendFormat("select ClassPerson.[{0}] from ClassPerson", ClassPerson.PERSON_REF_FIELD);
                    cpDbQuery = BuildClassPersonConds(cpDbQuery, query);          
                }
                if (query.RoleId == CoreRoles.TEACHER_ROLE.Id)
                {
                    cpDbQuery.Sql.AppendFormat("select ClassTeacher.[{0}] from ClassTeacher ",ClassTeacher.PERSON_REF_FIELD);
                    new AndQueryCondition { { ClassTeacher.CLASS_REF_FIELD, query.ClassId } }.BuildSqlWhere(cpDbQuery, "ClassTeacher");                
                }
                dbQuery.AddParameters(cpDbQuery.Parameters);
                dbQuery.Sql.AppendFormat(" and vwPerson.[{0}] in ({1})", Person.ID_FIELD, cpDbQuery.Sql);
            }

            var stSyDbQuery = BuildStudentSchoolYearQuery(query.SchoolYearId, query.IsEnrolled, query.GradeLevelIds);
            dbQuery.Sql.AppendFormat("and (([{0}].[{1}] = {2} and  exists({3}))", "vwPerson"
                                     , Person.ROLE_REF_FIELD, CoreRoles.STUDENT_ROLE.Id, stSyDbQuery.Sql);
            dbQuery.AddParameters(stSyDbQuery.Parameters);
            dbQuery.Sql.AppendFormat("or ([{0}].[{1}] = {2}", "vwPerson", Person.ROLE_REF_FIELD, CoreRoles.TEACHER_ROLE.Id);
            if (query.GradeLevelIds != null && query.GradeLevelIds.Any())
            {
                var sql = string.Format(@"select * from Class   
                                          join ClassTeacher on ClassTeacher.ClassRef = Class.Id
                                          where ClassTeacher.PersonRef = vwPerson.Id and Class.GradeLevelRef in ({0})"
                                        , query.GradeLevelIds.Select(x => x.ToString()).JoinString(","));
                dbQuery.Sql.AppendFormat(" and exists({0})", sql);
            }
            dbQuery.Sql.Append("))");
            var orderBy = query.SortType == SortTypeEnum.ByFirstName ? Person.FIRST_NAME_FIELD : Person.LAST_NAME_FIELD;
            var res = PaginatedSelect<Person>(dbQuery, orderBy, query.Start, query.Count);
            return new PersonQueryResult
                {
                    Persons = res,
                    Query = query,
                    SourceCount = res.TotalCount
                };
        }

        private DbQuery BuildStudentSchoolYearQuery(int? schoolYearId, bool? isEnrolled, IEnumerable<int> gradeLevelIds)
        {
            var dbQuery = new DbQuery();
            var tableName = "StudentSchoolYear";
            dbQuery.Sql.AppendFormat(@"select * from [{0}] where [{0}].[{1}] = vwPerson.[{2}] "
                                 , tableName, StudentSchoolYear.STUDENT_FIELD_REF_FIELD, Person.ID_FIELD);
            if (gradeLevelIds != null && gradeLevelIds.Any())
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in ({2})", tableName
                    , StudentSchoolYear.GRADE_LEVEL_REF_FIELD
                    , gradeLevelIds.Select(x=>x.ToString()).JoinString(","));

            var stConds = new AndQueryCondition(); 
            if (schoolYearId.HasValue)
                stConds.Add(StudentSchoolYear.SCHOOL_YEAR_REF_FIELD, schoolYearId.Value);
            if (isEnrolled.HasValue)
            {
                var enrollmentStatus = isEnrolled.Value
                                           ? StudentEnrollmentStatusEnum.CurrentlyEnrolled
                                           : StudentEnrollmentStatusEnum.PreviouslyEnrolled;
                stConds.Add(StudentSchoolYear.ENROLLMENT_STATUS_FIELD, enrollmentStatus);

            }
            if (stConds.Count > 0)
            {
                dbQuery.Sql.Append(" and ");
                stConds.BuildSqlWhere(dbQuery, tableName, false);
            }
            return dbQuery;
        }

        private DbQuery BuildFilterConds(DbQuery dbQuery, PersonQuery query)
        {
            if (!string.IsNullOrEmpty(query.Filter))
            {
                var words = query.Filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var wordsPrNames = new List<string>();
                for (int i = 0; i < words.Length; i++)
                {
                    var wordPrName = string.Format("filter{0}", i + 1);
                    dbQuery.Parameters.Add(wordPrName, string.Format(FILTER_FORMAT, words[i]));
                    wordsPrNames.Add(wordPrName);
                }
                dbQuery.Sql.AppendFormat(" and ({0})", wordsPrNames.Select(x => string.Format("[{0}] like @{1} or [{2}] like @{1}"
                    , Person.FIRST_NAME_FIELD, x, Person.LAST_NAME_FIELD)).JoinString(" or "));
            }
            return dbQuery;
        }

        private DbQuery BuildClassPersonConds(DbQuery dbQuery, PersonQuery query, bool first = true)
        {
            var cpConds = new AndQueryCondition();
            if (query.ClassId.HasValue)
                cpConds.Add(ClassPerson.CLASS_REF_FIELD, query.ClassId);
            if (query.MarkingPeriodId.HasValue)
                cpConds.Add(ClassPerson.MARKING_PERIOD_REF, query.MarkingPeriodId);
            if (query.IsEnrolled.HasValue)
                cpConds.Add(ClassPerson.IS_ENROLLED_FIELD, query.IsEnrolled.Value);
            if (cpConds.Count > 0)
            {
                if (!first) dbQuery.Sql.Append(" and ");
                cpConds.BuildSqlWhere(dbQuery, "ClassPerson", first);
            }
            return dbQuery;
        }

        //public PersonQueryResult GetPersons(PersonQuery query)
        //{
            
        //    var parameters = new Dictionary<string, object>();
        //    parameters.Add("@personId", query.PersonId);
        //    parameters.Add("@callerId", query.CallerId);
        //    parameters.Add("@markingPeriodId", query.MarkingPeriodId);
        //    parameters.Add("@schoolYearId", query.SchoolYearId);
        //    parameters.Add("@isEnrolled", query.IsEnrolled);

        //    //string roleIdsS = "";
        //    //if (query.RoleIds != null && query.RoleIds.Count > 0)
        //    //{
        //    //    roleIdsS = query.RoleIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
        //    //}
        //    //parameters.Add("@roleIds", roleIdsS);
        //    parameters.Add("@roleId", query.RoleId);
        //    parameters.Add("@start", query.Start);
        //    parameters.Add("@count", query.Count);
        //    parameters.Add("@startFrom", query.StartFrom);

        //    parameters.Add("@teacherId", query.TeacherId);
        //    parameters.Add("@classId", query.ClassId);
        //    parameters.Add("@callerRoleId", query.CallerRoleId);
        //    parameters.Add("@schoolId", schoolId);
        //    parameters.Add("@onlyMyTeachers", query.OnlyMyTeachers);

        //    string filter1 = null;
        //    string filter2 = null;
        //    string filter3 = null;
        //    if (!string.IsNullOrEmpty(query.Filter))
        //    {
        //        string[] sl = query.Filter.Trim().Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (sl.Length > 0)
        //            filter1 = string.Format(FILTER_FORMAT, sl[0]);
        //        if (sl.Length > 1)
        //            filter2 = string.Format(FILTER_FORMAT, sl[1]);
        //        if (sl.Length > 2)
        //            filter3 = string.Format(FILTER_FORMAT, sl[2]);
        //    }
        //    parameters.Add("@filter1", filter1);
        //    parameters.Add("@filter2", filter2);
        //    parameters.Add("@filter3", filter3);

        //    string glIds = null;
        //    if (query.GradeLevelIds != null)
        //        glIds = query.GradeLevelIds.Select(x => x.ToString()).JoinString(",");
        //    parameters.Add("@gradeLevelIds", glIds);
        //    parameters.Add("@sortType", (int)query.SortType);
            
        //    using (var reader = ExecuteStoredProcedureReader("spGetPersons", parameters))
        //    {
        //        var result = ReadPersonQueryResult(reader);
        //        result.Query = query;
        //        return result;
        //    }
        //}

        public PersonDetails GetPersonDetails(int personId, int callerId, int callerRoleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"callerId", callerId},
                    {"callerRoleId", callerRoleId},
                    {"schoolId", schoolId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonDetails", parameters))
            {
                reader.NextResult(); // skip AllCount 
                return reader.Read() ? ReadPersonDetailsData(reader) : null;
            }
        }

        public static PersonQueryResult ReadPersonQueryResult(DbDataReader reader)
        {
            var res = new PersonQueryResult();
            reader.Read();
            res.SourceCount = SqlTools.ReadInt32(reader, "AllCount");
            reader.NextResult();
            res.Persons = new List<Person>();
            while (reader.Read())
            {
                res.Persons.Add(ReadPersonData(reader));
            }
            return res;
        }
        
        public static PersonDetails ReadPersonDetailsData(SqlDataReader reader)
        {
            var res = ReadPersonData(reader);
            reader.NextResult();
            res.Address = reader.ReadOrNull<Address>();
            reader.NextResult();
            res.Phones = reader.ReadList<Phone>();
            reader.NextResult();
            res.StudentSchoolYears = new List<StudentSchoolYear>();
            while (reader.Read())
            {
                var studentSchoolYear = reader.Read<StudentSchoolYear>();
                studentSchoolYear.GradeLevel = reader.Read<GradeLevel>();
                res.StudentSchoolYears.Add(studentSchoolYear);
            }
            return res;
        }
        
        public static PersonDetails ReadPersonData(DbDataReader reader)
        {
            if (reader != null)
            {
                var res = reader.Read<PersonDetails>();
                return res;
            }
            return null;
        }


        public static int GetPersonDataForLogin(string districtServerUrl, Guid districtId, int userId, out int roleId)
        {
            var connectionString = string.Format(Settings.SchoolConnectionStringTemplate, districtServerUrl, districtId);
            using (var uow = new UnitOfWork(connectionString, false))
            {
                //TODO: select person id and determine the role
                throw new NotImplementedException();
            }
        }
    }

    public class PersonQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? RoleId { get; set; }
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public int? PersonId { get; set; }
        public int? CallerId { get; set; }
        public int CallerRoleId { get; set; }
        public int? CallerGradeLevelId { get; set; }

        public IList<int> RoleIds { get; set; } 

        public string StartFrom { get; set; }
        public string Filter { get; set; }
        public IEnumerable<int> GradeLevelIds { get; set; }
        public SortTypeEnum SortType { get; set; }

        public int? MarkingPeriodId { get; set; }
        public int? SchoolYearId { get; set; }

        public bool? IsEnrolled { get; set; }
        public bool OnlyMyTeachers { get; set; }

        public PersonQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            SortType = SortTypeEnum.ByLastName;
            OnlyMyTeachers = false;
        }
    }

    public class PersonQueryResult
    {
        public List<Person> Persons { get; set; }
        public int SourceCount { get; set; }
        public PersonQuery Query { get; set; }
    }

    public enum SortTypeEnum
    {
        ByFirstName = 0,
        ByLastName = 1
    }
}
