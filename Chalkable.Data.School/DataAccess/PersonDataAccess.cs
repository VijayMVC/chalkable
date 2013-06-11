using System;
using System.Collections.Generic;
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

        //public IList<Person> GetPersons()
        //{
        //    var conds = new Dictionary<string, object>();
        //    var sql = "select * from [Person]";
        //    using (var reader = ExecuteReaderParametrized(sql, conds))
        //    {
        //        var res = reader.ReadList<Person>();
        //        return res;
        //    }
        //}

        public Person GetById(Guid id)
        {
            var conds = new Dictionary<string, object>() { { "@id", id } };
            var sql = "select * from [Person] where Id = @id";
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                var res = reader.ReadOrNull<Person>();
                return res;
            }
        }
    }
}
