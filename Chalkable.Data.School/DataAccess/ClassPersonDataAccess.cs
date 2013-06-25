using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassPersonDataAccess : DataAccessBase<ClassPerson>
    {
        public ClassPersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(ClassPersonQuery query)
        {
            SimpleDelete<ClassPerson>(BuildConditioins(query));
        }

        public Dictionary<string, object> BuildConditioins(ClassPersonQuery query)
        {
            var conds = new Dictionary<string, object>();
            if(query.Id.HasValue)
                conds.Add("Id", query.Id);
            if(query.ClassId.HasValue)
                conds.Add("ClassRef",query.ClassId);
            if(query.PersonId.HasValue)
                conds.Add("PersonRef", query.PersonId);
            return conds;
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
        public Guid? Id { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? PersonId { get; set; }
    }
}
