﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonDataAccess : DataAccessBase<Person>
    {
        public PersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string FILTER_FORMAT = "%{0}%";
        
       
        public PersonQueryResult GetPersons(PersonQuery query)
        {
            
            var parameters = new Dictionary<string, object>();
            parameters.Add("@personId", query.PersonId);
            parameters.Add("@callerId", query.CallerId);
            parameters.Add("@roleId", query.RoleId);
            parameters.Add("@start", query.Start);
            parameters.Add("@count", query.Count);
            parameters.Add("@startFrom", query.StartFrom);

            parameters.Add("@teacherId", query.TeacherId);
            parameters.Add("@classId", query.ClassId);
            parameters.Add("@callerRoleId", query.CallerRoleId);
            

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

        public PersonDetails GetPersonDetails(Guid personId, Guid callerId, int callerRoleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"callerId", callerId},
                    {"callerRoleId", callerRoleId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonDetails", parameters))
            {
                if (reader.Read())
                {
                    return ReadPersonDetailsData(reader);
                }
                return null;
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
            res.Addresses = reader.ReadList<Address>();
            reader.NextResult();
            res.Phones = reader.ReadList<Phone>();
            return res;
        }

        public static PersonDetails ReadPersonData(DbDataReader reader)
        {
            if (reader != null)
            {
                var res = reader.Read<PersonDetails>();
                if (res.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                {
                    res.StudentInfo = reader.Read<StudentInfo>();
                    res.StudentInfo.GradeLevel = reader.Read<GradeLevel>(true);
                    res.StudentInfo.GradeLevelRef = res.StudentInfo.GradeLevel.Id;
                }
                return res;
            }
            return null;
        }

        public void AddStudent(Guid id, Guid gradeLevelId)
        {
            SimpleInsert(new StudentInfo{GradeLevelRef = gradeLevelId, Id = id});
        }

    }

    public class PersonQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? RoleId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CallerId { get; set; }
        public int CallerRoleId { get; set; }
        
        public string StartFrom { get; set; }
        public string Filter { get; set; }
        public IEnumerable<Guid> GradeLevelIds { get; set; }
        public SortTypeEnum SortType { get; set; }

        

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
