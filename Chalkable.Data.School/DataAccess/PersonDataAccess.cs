﻿using System;
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
            
            var parameters = new Dictionary<string, object>();
            parameters.Add("@personId", query.PersonId);
            parameters.Add("@callerId", query.CallerId);
            parameters.Add("@markingPeriodId", query.MarkingPeriodId);
            parameters.Add("@schoolYearId", query.SchoolYearId);
            parameters.Add("@isEnrolled", query.IsEnrolled);

            //string roleIdsS = "";
            //if (query.RoleIds != null && query.RoleIds.Count > 0)
            //{
            //    roleIdsS = query.RoleIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            //}
            //parameters.Add("@roleIds", roleIdsS);
            parameters.Add("@roleId", query.RoleId);
            parameters.Add("@start", query.Start);
            parameters.Add("@count", query.Count);
            parameters.Add("@startFrom", query.StartFrom);

            parameters.Add("@teacherId", query.TeacherId);
            parameters.Add("@classId", query.ClassId);
            parameters.Add("@callerRoleId", query.CallerRoleId);
            parameters.Add("@schoolId", schoolId);
            
            string filter1 = null;
            string filter2 = null;
            string filter3 = null;
            if (!string.IsNullOrEmpty(query.Filter))
            {
                string[] sl = query.Filter.Trim().Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sl.Length > 0)
                    filter1 = string.Format(FILTER_FORMAT, sl[0]);
                if (sl.Length > 1)
                    filter2 = string.Format(FILTER_FORMAT, sl[1]);
                if (sl.Length > 2)
                    filter3 = string.Format(FILTER_FORMAT, sl[2]);
            }
            parameters.Add("@filter1", filter1);
            parameters.Add("@filter2", filter2);
            parameters.Add("@filter3", filter3);

            string glIds = null;
            if (query.GradeLevelIds != null)
                glIds = query.GradeLevelIds.Select(x => x.ToString()).JoinString(",");
            parameters.Add("@gradeLevelIds", glIds);
            parameters.Add("@sortType", (int)query.SortType);
            
            using (var reader = ExecuteStoredProcedureReader("spGetPersons", parameters))
            {
                var result = ReadPersonQueryResult(reader);
                result.Query = query;
                return result;
            }
        }

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

        public IList<int> RoleIds { get; set; } 

        public string StartFrom { get; set; }
        public string Filter { get; set; }
        public IEnumerable<int> GradeLevelIds { get; set; }
        public SortTypeEnum SortType { get; set; }

        public int? MarkingPeriodId { get; set; }
        public int? SchoolYearId { get; set; }

        public bool? IsEnrolled { get; set; }

        public PersonQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            SortType = SortTypeEnum.ByLastName;
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
