using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDisciplineDataAccess : DataAccessBase<ClassDiscipline>
    {
        public ClassDisciplineDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string GET_CLASS_DISCIPLINE_PROC = "spGetClassDisciplines";
        private const string STUDENT_ID_PARAM = "studentId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";
        private const string CLASS_ID_PARAM = "classId";
        private const string TEACHER_ID_PARAM = "teacherId";
        private const string TYPE_PARAM = "type";
        private const string FROM_TIME_PARAM = "fromTime";
        private const string TO_TIME_PARAM = "toTime";
        private const string SCHOOLYEAR_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string NEED_ALL_DATA_PARAM = "needAllData";

        public IList<ClassDisciplineDetails> GetClassDisciplinesDetails(ClassDisciplineQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, query.Id},
                    {STUDENT_ID_PARAM, query.PersonId},
                    {TEACHER_ID_PARAM, query.TeacherId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {CLASS_ID_PARAM, query.ClassId},
                    {FROM_DATE_PARAM, query.FromDate},
                    {TO_DATE_PARAM, query.ToDate},
                    {FROM_TIME_PARAM, query.StartTime},
                    {TO_TIME_PARAM, query.EndTime},
                    {TYPE_PARAM, query.Type},
                    {SCHOOLYEAR_PARAM, query.SchoolYearId},
                    {NEED_ALL_DATA_PARAM, query.NeedAllData},
                };
            using (var reader = ExecuteStoredProcedureReader(GET_CLASS_DISCIPLINE_PROC, parameters))
            {
                var res = new List<ClassDisciplineDetails>();
                while (reader.Read())
                {
                    var classDisciplineType = reader.Read<ClassDisciplineTypeDetails>(true);
                    var classDiscipline = reader.Read<ClassDisciplineDetails>(true);
                    classDiscipline.Student = PersonDataAccess.ReadPersonData(reader);
                    var discipline = res.FirstOrDefault(x => x.ClassPeriodRef == classDiscipline.ClassPeriodRef
                                                             && x.ClassPersonRef == classDiscipline.ClassPersonRef
                                                             && x.Date == classDiscipline.Date);
                    if (discipline == null)
                    {
                        classDiscipline.DisciplineTypes = new List<ClassDisciplineTypeDetails> {classDisciplineType};
                        res.Add(classDiscipline);
                        classDisciplineType.ClassDiscipline = classDiscipline;
                    }
                    else
                    {
                        discipline.DisciplineTypes.Add(classDisciplineType);
                        classDisciplineType.ClassDiscipline = discipline;
                    }
                }
                return res;
            }
        } 


        public ClassDiscipline GetClassDiscipline(Guid classPeriodId, Guid classPersonId, DateTime date)
        {
            var conds = new AndQueryCondition()
                {
                    {ClassDiscipline.CLASS_PERIOD_REF_FIELD, classPeriodId},
                    {ClassDiscipline.CLASS_PERSON_REF_FIELD, classPersonId},
                    {ClassDiscipline.DATE_FIELD, date},
                };
           return SelectOneOrNull<ClassDiscipline>(conds);
        }

        public IList<DisciplineTotalPerType> CalcDisciplineTypeTotal(Guid? schoolYearId, Guid? markingPeriodId,
                  Guid? studentId, DateTime? fromDate, DateTime? toDate)
        {
            var parametrs = new Dictionary<string, object>
                {
                    {MARKING_PERIOD_ID_PARAM, markingPeriodId},
                    {SCHOOLYEAR_PARAM, schoolYearId},
                    {FROM_DATE_PARAM, fromDate},
                    {TO_DATE_PARAM, toDate},
                    {STUDENT_ID_PARAM, studentId}
                };
            using (var reader = ExecuteStoredProcedureReader("spCalcDisciplineTypeTotal", parametrs))
            {
                return reader.ReadList<DisciplineTotalPerType>();
            }
        }
    }

    public class ClassDisciplineQuery
    {
        public Guid? PersonId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public Guid? SchoolYearId { get; set; }
        public Guid? Id { get; set; }
        public bool NeedAllData { get; set; }
    }
}
