using System.Collections.Generic;
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
            return Exists(Orm.SimpleSelect<SchoolPerson>(GetSchoolPersonsCondition(personId, roleId, schoolId)));
        }

        private QueryCondition GetSchoolPersonsCondition(int? personId, int? roleId, int? schoolId)
        {
            return new AndQueryCondition
                {
                    {SchoolPerson.PERSON_REF_FIELD, personId},
                    {SchoolPerson.ROLE_REF, roleId},
                    {SchoolPerson.SCHOOL_REF_FIELD, schoolId}
                };
        } 

        public IList<SchoolPerson> GetSchoolPersons(int? personId, int? roleId, int? schoolId)
        {
            return SelectMany<SchoolPerson>(GetSchoolPersonsCondition(personId, roleId, schoolId));
        }
        public SchoolPerson GetSchoolPerson(int personId, int? schoolId, int? roleId)
        {
            return SelectOne<SchoolPerson>(GetSchoolPersonsCondition(personId, roleId, schoolId));
        }
    }
}