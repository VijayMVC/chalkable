using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string FILTER1_PARAM = "filter1";
        private const string FILTER2_PARAM = "filter2";
        private const string FILTER3_PARAM = "filter3";
        
        public ClassQueryResult GetClassesComplex(ClassQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, query.SchoolYearId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {PERSON_ID_PARAM, query.PersonId},
                    {CLASS_ID_PARAM, query.ClassId},
                    {CALLER_ID_PARAM, query.CallerId},
                    {START_PARAM, query.Start},
                    {COUNT_PARAM, query.Count}
                };

            string filter1 = null;
            string filter2 = null;
            string filter3 = null;
            if (!string.IsNullOrEmpty(query.Filter))
            {
                string[] sl = query.Filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sl.Length > 0)
                    filter1 = string.Format(FILTER_FORMAT, sl[0]);
                if (sl.Length > 1)
                    filter2 = string.Format(FILTER_FORMAT, sl[1]);
                if (sl.Length > 2)
                    filter3 = string.Format(FILTER_FORMAT, sl[2]);
            }
            parameters.Add(FILTER1_PARAM, filter1);
            parameters.Add(FILTER2_PARAM, filter2);
            parameters.Add(FILTER3_PARAM, filter3);


            using (var reader = ExecuteStoredProcedureReader(GET_CLASSES_PROC, parameters))
            {
                var sourceCount = reader.Read() ? SqlTools.ReadInt32(reader, "SourceCount") : 0;
                reader.NextResult();
                var classes = reader.ReadList<ClassComplex>(true);
                reader.NextResult();
                var markingPeriodClasses = reader.ReadList<MarkingPeriodClass>();
                foreach (var classComplex in classes)
                {
                    classComplex.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == classComplex.Id).ToList();
                }
                return new ClassQueryResult { Classes = classes, Query = query, SourceCount = sourceCount };
            }
        }
    }

    public class ClassQuery
    {
        public Guid CallerId { get; set; }
        public Guid? SchoolYearId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? PersonId { get; set; }
        public string Filter { get; set; }
        
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
        public int SourceCount { get; set; }
    }
}
