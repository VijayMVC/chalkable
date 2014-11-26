using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
        
        public void Delete(IList<int> ids)
        {
            if (ids.Count == 0)
                return;
            var res = new StringBuilder();
            var idsS = ids.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(",");
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
            var connectionString = Settings.GetSchoolConnectionString(districtServerUrl, districtId);
            using (var uow = new UnitOfWork(connectionString, false))
            {
                var student = new StudentDataAccess(uow)
                    .GetAll(new AndQueryCondition {{Student.USER_ID_FIELD, userId}}).FirstOrDefault();
                if (student != null)
                {
                    roleId = CoreRoles.STUDENT_ROLE.Id;
                    return student.Id;
                }
                var staff = new StaffDataAccess(uow)
                    .GetAll(new AndQueryCondition {{Staff.USER_ID_FIELD, userId}}).FirstOrDefault();
                if (staff != null)
                {
                    roleId = CoreRoles.TEACHER_ROLE.Id;
                    return staff.Id;
                }
                var person = new PersonDataAccess(uow, null)
                    .GetAll(new AndQueryCondition {{Person.USER_ID_FIELD, userId}}).FirstOrDefault();
                if (person != null)
                {
                    roleId = CoreRoles.PARENT_ROLE.Id;
                    return person.Id;
                }
                throw new ChalkableException("User is not identified");
            }
        }

        public PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count)
        {
            var ps = new Dictionary<string, object>
            {
                {"@start", start},
                {"@count", count},
                {"@filter", "%" + filter + "%"},
                {"@orderByFirstName", orderByFirstName}
            };
            return ExecuteStoredProcedurePaginated<Person>("spSearchPersons", ps, start, count);
        }
    }

}
