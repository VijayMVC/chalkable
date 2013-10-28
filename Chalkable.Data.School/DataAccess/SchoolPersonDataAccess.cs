using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolPersonDataAccess : DataAccessBase<SchoolPerson, int>
    {
        public SchoolPersonDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public bool Exists(int? personId, int? roleId, int? schoolId)
        {
            return Exists(Orm.SimpleSelect<SchoolPerson>(new AndQueryCondition
                {
                    {SchoolPerson.PERSON_REF_FIELD, personId},
                    {SchoolPerson.ROLE_REF, roleId},
                    {SchoolPerson.SCHOOL_REF_FIELD, schoolId}
                }));
        }
    }
}