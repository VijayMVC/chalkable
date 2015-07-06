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
            var res = new AndQueryCondition();
            if(personId.HasValue)
                res.Add(SchoolPerson.PERSON_REF_FIELD, personId);
            if (roleId.HasValue)
                res.Add(SchoolPerson.ROLE_REF, roleId);
            if (schoolId.HasValue)
                res.Add(SchoolPerson.SCHOOL_REF_FIELD, schoolId);
            return res;
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