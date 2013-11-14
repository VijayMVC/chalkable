using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassPersonDataAccess : BaseSchoolDataAccess<ClassPerson>
    {
        public ClassPersonDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void Delete(ClassPersonQuery query)
        {
            SimpleDelete<ClassPerson>(BuildConditioins(query));
        }

        public QueryCondition BuildConditioins(ClassPersonQuery query)
        {
            var conds = new AndQueryCondition();
            if(query.ClassId.HasValue)
                conds.Add(ClassPerson.CLASS_REF_FIELD, query.ClassId);
            if(query.PersonId.HasValue)
                conds.Add(ClassPerson.PERSON_REF_FIELD, query.PersonId);
            return conds;// FilterBySchool(conds);
        }

        public bool Exists(ClassPersonQuery query)
        {
            return Exists<ClassPerson>(BuildConditioins(query));
        }

        public ClassPerson GetClassPerson(ClassPersonQuery query)
        {
            return SelectOne<ClassPerson>(BuildConditioins(query));
        }

        public ClassPerson GetClassPersonOrNull(ClassPersonQuery query)
        {
            return SelectOneOrNull<ClassPerson>(BuildConditioins(query));
        }

        public IList<ClassPerson> GetClassPersons(ClassPersonQuery query)
        {
            return SelectMany<ClassPerson>(BuildConditioins(query));
        }
    }

    public class ClassPersonQuery
    {
        public int? ClassId { get; set; }
        public int? PersonId { get; set; }
    }
}
