using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonDataAccess : DataAccessBase<Person, int>
    {
        public PersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void UpdateForImport(IList<Person> persons)
        {
            ModifyList(persons, UpdateForImport, BuildQueryForImportUpdate);
        }

        private DbQuery BuildQueryForImportUpdate(IEnumerable<Person> persons)
        {
            IList<DbQuery> queries = new List<DbQuery>();
            var index = 0;
            foreach (var person in persons)
            {
                var p = new Dictionary<string, object>
                    {
                        {Person.FIRST_NAME_FIELD, person.FirstName},
                        {Person.LAST_NAME_FIELD, person.LastName},
                        {Person.GENDER_FIELD, person.Gender},
                        {Person.ADDRESS_REF_FIELD, person.AddressRef},
                        {Person.USER_ID_FIELD, person.UserId},
                        {Person.BIRTH_DATE, person.BirthDate},
                        {Person.PHOTO_MODIFIED_DATE, person.PhotoModifiedDate}
                    };
                queries.Add(Orm.SimpleUpdate(person.GetType(), p, new AndQueryCondition
                                     {
                                         {Person.ID_FIELD, $"{Person.ID_FIELD}_{index}", person.Id, ConditionRelation.Equal}
                                     }, index));
                index++;
            }
            return new DbQuery(queries);
        }

        public PersonDetails GetPersonDetails(int personId, int schoolId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"schoolId", schoolId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonDetails", parameters))
            {
                return ReadPersonDetailsData(reader);
            }
        }

        public static PersonDetails ReadPersonDetailsData(SqlDataReader reader)
        {
            if (!reader.Read())
                return null;
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
            reader.NextResult();
            res.PersonEmails = reader.ReadList<PersonEmail>();
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
//#if DEBUG
//                    if (staff.Id == 8502) // if is regulare teacher (teacher@chalkable.com)
//                        roleId = CoreRoles.DISTRICT_ADMIN_ROLE.Id;
//#endif
                    return staff.Id;
                }
                var person = new PersonDataAccess(uow)
                    .GetAll(new AndQueryCondition {{Person.USER_ID_FIELD, userId}}).FirstOrDefault();
                if (person != null)
                {
                    roleId = CoreRoles.PARENT_ROLE.Id;
                    return person.Id;
                }
                throw new ChalkableException("User is not identified");
            }
        }

        public PaginatedList<Person> SearchPersons(int schoolId, string filter, bool orderByFirstName, int start, int count)
        {
            var filters = (filter != null) ? filter.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries) : null;

            var ps = new Dictionary<string, object>
            {
                {"@schoolId", schoolId},
                {"@start", start},
                {"@count", count},
                {"@filter1", filters!=null && filters.Length>0 ? "%"+filters[0]+"%" : null},
                {"@filter2", filters!=null && filters.Length>1 ? "%"+filters[1]+"%" : null},
                {"@filter3", filters!=null && filters.Length>2 ? "%"+filters[2]+"%" : null},
                {"@orderByFirstName", orderByFirstName}
            };
            return ExecuteStoredProcedurePaginated<Person>("spSearchPersons", ps, start, count);
        }

        public void Delete(IList<int> ids)
        {
            var ps = new Dictionary<string, object>
            {
                {"@personIds", ids}
            };
            ExecuteStoredProcedure("spDeletePersons", ps);
        }
    }
}
