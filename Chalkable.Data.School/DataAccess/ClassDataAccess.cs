using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDataAccess : DataAccessBase<Class>
    {
        public ClassDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string GET_CLASSES_PROC = "spGetClasses";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string CALLER_ID_PARAM = "callerId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string CLASS_ID_PARAM = "classId";
        private const string PERSON_ID_PARAM = "personId";
        private const string START_PARAM = "start";
        private const string COUNT_PARAM = "count";

        public ClassQueryResult GetClassesComplex(ClassQuery query, Guid callerId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, query.SchoolYearId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {PERSON_ID_PARAM, query.PersonId},
                    {CLASS_ID_PARAM, query.ClassId},
                    {CALLER_ID_PARAM, callerId},
                    {START_PARAM, query.Start},
                    {COUNT_PARAM, query.Count}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_CLASSES_PROC, parameters))
            {
                var classes = reader.ReadList<ClassComplex>(true);
                return new ClassQueryResult {Classes = classes, Query = query};
            }
        }
    }

    public class ClassQuery
    {
        public Guid? SchoolYearId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? PersonId { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }

        public ClassQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }

    public class ClassQueryResult
    {
        public ClassQuery Query { get; set; }
        public IList<ClassComplex> Classes { get; set; } 
    }
}
