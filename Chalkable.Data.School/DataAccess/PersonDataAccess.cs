using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var sql = @"Insert into [Person] (Id, FirstName, LastName, BirthDate, Gender, Salutation, Active,"
                      + "LastPasswordReset, FirstLoginDate, RoleRef, LastMailNotification, SisId, Email)"  
                      + " values (@id, @firstName, @lastName, @birthDate, @Gender, @salutation, @active," 
                      + " @lastPasswordReset, @firstLoginDate, @roleRef, @lastMailNotification, @sisId, @email)";

            ExecuteNonQueryParametrized(sql, BuildConditions(person));
        }

        private Dictionary<string, object> BuildConditions(Person person)
        {
            return new Dictionary<string, object>
                {
                    {"@id", person.Id},
                    {"@firstName", person.FirstName},
                    {"@lastName", person.LastName},
                    {"@birthDate", person.BirthDate},
                    {"@gender", person.Gender},
                    {"@salutation", person.Salutation},
                    {"@active", person.Active},
                    {"@lastPasswordReset", person.LastPasswordReset},
                    {"@firstLoginDate", person.FirstLoginDate},
                    {"@roleRef", person.RoleRef},
                    {"@lastMailNotification", person.LastMailNotification},
                    {"@sisId", person.SisId},
                    {"@email", person.Email},
                };
        }


        private const string INSERT_TEMPLATE = "insert into {0} ({1}) values({2})";
        private const string UPDATE_TEMPLATE = "update {0} set {1} where {2}";
        //private const string DELETE_TEMPLATE = "delete "

        private string BuildUpdateQuery(string tableName, IDictionary<string, object> conditions, IDictionary<string, object> whereConditions)
        {
            var valuesCommandBuilder = new StringBuilder();
            var whereCommandBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (var condition in conditions)
            {
                if (!isFirst) valuesCommandBuilder.Append(",");
                else isFirst = false;
                valuesCommandBuilder.Append(condition.Key).Append("=").Append("@").Append(condition.Key);
            }
            isFirst = true;
            foreach (var condition in whereConditions)
            {
                if (!isFirst) whereCommandBuilder.Append(" and ");
                else isFirst = false;
                whereCommandBuilder.Append(condition.Key).Append("=").Append("@").Append(condition.Key);
            }
            return string.Format(UPDATE_TEMPLATE, tableName, valuesCommandBuilder, whereConditions);
        }

        public void Update(Person person)
        {
            var conditions = BuildConditions(person);
            var sqlCommand = BuildUpdateQuery("Person", conditions, new Dictionary<string, object>{{"@id", person.Id}});
            ExecuteNonQueryParametrized(sqlCommand, conditions);
        }

        public void Delete()
        {
        }
    }
}
