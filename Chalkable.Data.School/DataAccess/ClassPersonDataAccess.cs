using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassPersonDataAccess : DataAccessBase<ClassPerson, int>
    {
        public ClassPersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public QueryCondition BuildConditioins(ClassPersonQuery query)
        {
            var conds = new AndQueryCondition();
            if(query.ClassId.HasValue)
                conds.Add(ClassPerson.CLASS_REF_FIELD, query.ClassId);
            if(query.PersonId.HasValue)
                conds.Add(ClassPerson.PERSON_REF_FIELD, query.PersonId);
            if(query.MarkingPeriodId.HasValue)
                conds.Add(ClassPerson.MARKING_PERIOD_REF, query.MarkingPeriodId);
            if (query.IsEnrolled.HasValue)
                conds.Add(ClassPerson.IS_ENROLLED_FIELD, query.IsEnrolled);
            return conds;
        }

        public bool Exists(ClassPersonQuery query)
        {
            return Exists<ClassPerson>(BuildConditioins(query));
        }

        public IList<ClassPerson> GetClassPersons(ClassPersonQuery query)
        {
            return SelectMany<ClassPerson>(BuildConditioins(query));
        }

        public IList<ClassPerson> GetClassPersons(int? classId)
        {
            return GetClassPersons(new ClassPersonQuery {ClassId = classId});
        } 
    }

    public class ClassPersonQuery
    {
        public int? ClassId { get; set; }
        public int? PersonId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public bool? IsEnrolled { get; set; }
    }
}
