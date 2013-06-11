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
    }
}
