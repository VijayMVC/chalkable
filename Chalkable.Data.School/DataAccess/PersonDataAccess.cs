using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonDataAccess : DataAccessBase
    {
        public PersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void Create(Person person)
        {
            SimpleInsert(person);
        }

        public void Update(Person person)
        {
            SimpleUpdate(person);
        }

        public void Delete(Person person)
        {
            SimpleDelete(person);
        }

        public Person GetById(Guid id)
        {
            var conds = new Dictionary<string, object> { { "id", id } };
            return SelectOne<Person>(conds);
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
                glIds = query.GradeLevelIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(",");
            parameters.Add("@gradeLevelIds", glIds);

            parameters.Add("@sortType", (int)query.SortType);
            

            var result = new PersonQueryResult();
            result.Query = query;
            using (var reader = ExecuteStoredProcedureReader("spGetPersons", parameters))
            {
                reader.Read();
                result.SourceCount = SqlTools.ReadInt32(reader, "AllCount");
                reader.NextResult();
                result.Persons = new List<Person>();
                while (reader.Read())
                {
                    var p = reader.Read<Person>();
                    p.StudentInfo = reader.Read<StudentInfo>();    
                    result.Persons.Add(p);
                }
            }
            return result;
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
        public Guid? SchoolId { get; set; }

        public string StartFrom { get; set; }
        public string Filter { get; set; }
        public IEnumerable<int> GradeLevelIds { get; set; }
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
