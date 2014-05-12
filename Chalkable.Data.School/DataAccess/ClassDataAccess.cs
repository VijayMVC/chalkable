using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDataAccess : BaseSchoolDataAccess<Class>
    {
        public ClassDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
        {
        }

        private const string GET_CLASSES_PROC = "spGetClasses";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string CALLER_ID_PARAM = "callerId";
        private const string CALLER_ROLE_ID_PARAM = "callerRoleId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string CLASS_ID_PARAM = "classId";
        private const string PERSON_ID_PARAM = "personId";
        private const string START_PARAM = "start";
        private const string COUNT_PARAM = "count";
        private const string FILTER1_PARAM = "filter1";
        private const string FILTER2_PARAM = "filter2";
        private const string FILTER3_PARAM = "filter3";
        private const string SCHOOL_ID = "schoolId";

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
                    {COUNT_PARAM, query.Count},
                    {CALLER_ROLE_ID_PARAM, query.CallerRoleId},
                    {SCHOOL_ID, schoolId}
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
                var classes = new List<ClassDetails>();
                while (reader.Read())
                {
                    var c = reader.Read<ClassDetails>(true);
                    if (c.TeacherRef.HasValue)
                        c.Teacher = reader.Read<Person>(true);
                    c.GradeLevel = reader.Read<GradeLevel>(true);
                    classes.Add(c);
                }
                reader.NextResult();
                var markingPeriodClasses = reader.ReadList<MarkingPeriodClass>();
                foreach (var classComplex in classes)
                {
                    classComplex.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == classComplex.Id).ToList();
                }
                return new ClassQueryResult { Classes = classes, Query = query, SourceCount = sourceCount };
            }
        }


        public void Delete(IList<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                var sqlFormat = "delete from [{0}] where [{0}].[{1}] in ({2}) ";
                var sqlBuilder = new StringBuilder();
                var idsString = ids.Select(x => x.ToString()).JoinString(",");
                sqlBuilder.AppendFormat(sqlFormat, "ClassPerson", ClassPerson.CLASS_REF_FIELD, idsString)
                          .AppendFormat(sqlFormat, "MarkingPeriodClass", MarkingPeriodClass.CLASS_REF_FIELD, idsString)
                          .AppendFormat(sqlFormat, "Class", Class.ID_FIELD, idsString);
                ExecuteNonQueryParametrized(sqlBuilder.ToString(), new Dictionary<string, object>());
            }
        }

        public new void Delete(int id)
        {
            Delete(new List<int> {id});
        }
    }

    public class ClassQuery
    {
        public int CallerId { get; set; }
        public int CallerRoleId { get; set; }
        public int? SchoolYearId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public int? ClassId { get; set; }
        public int? PersonId { get; set; }
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
        public IList<ClassDetails> Classes { get; set; }
        public int SourceCount { get; set; }
    }
}
