using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class DisciplineView
    {
        public Guid StudentId { get; set; }
        public PeriodViewData Period { get; set; }
        public string ClassName { get; set; }
        public Guid TeacherId { get; set; }
        public IList<DisciplineTypeViewData> DisciplineTypes { get; set; }
        public string Description { get; set; }
        public Guid ClassPersonId { get; set; }
        public Guid ClassPeriodId { get; set; }
        public string Summary { get; set; }
        public bool Editable { get; set; }

        protected DisciplineView(ClassDisciplineDetails discipline, Guid currentPersonId,  bool canEdit)
        {
            StudentId = discipline.Student.Id;
            Period = PeriodViewData.Create(discipline.ClassPeriod.Period);
            DisciplineTypes = DisciplineTypeViewData.Create(discipline.DisciplineTypes.Select(x => x.DisciplineType).ToList());
            ClassName = discipline.Class.Name;
            TeacherId = discipline.Class.TeacherRef;
            ClassPeriodId = discipline.ClassPeriodRef;
            ClassPersonId = discipline.ClassPersonRef;
            Editable = canEdit || currentPersonId == TeacherId;
        }

        public static IList<DisciplineView> Create(IList<ClassDisciplineDetails> disciplines, Guid currentPersonId,
                                            bool canEdit = false)
        {
            return disciplines.Select(x => new DisciplineView(x, currentPersonId, canEdit)).ToList();
        }
    }

    public class StudentDisciplineSummaryViewData 
    {
        public PersonViewData Student { get; set; }
        public int Total { get; set; }
        public int DisciplineRecordsNumber { get; set; }
        public string Summary { get; set; }

        private StudentDisciplineSummaryViewData(Person student)
        {
            Student = PersonViewData.Create(student);
        }

        public static IList<StudentDisciplineSummaryViewData> Create(IList<ClassDisciplineDetails> disciplines)
        {
            IDictionary<Guid, List<ClassDisciplineTypeDetails>> stDiscTypeDic = new Dictionary<Guid, List<ClassDisciplineTypeDetails>>(); 
            IDictionary<Guid, Person> stDic = new Dictionary<Guid, Person>();
            foreach (var discipline in disciplines)
            {
                if (!stDiscTypeDic.ContainsKey(discipline.Student.Id))
                {
                    stDic.Add(discipline.Student.Id, discipline.Student);
                    stDiscTypeDic.Add(discipline.Student.Id, new List<ClassDisciplineTypeDetails>());
                }
                stDiscTypeDic[discipline.Student.Id].AddRange(discipline.DisciplineTypes);

            }
            return stDiscTypeDic.Select(x => new StudentDisciplineSummaryViewData(stDic[x.Key])
                {
                    Summary = BuildSummary(x.Value),
                    Total = x.Value.Sum(y => y.DisciplineType.Score),
                    DisciplineRecordsNumber = x.Value.Count
                }).ToList();
        }

        private static string BuildSummary(IEnumerable<ClassDisciplineTypeDetails> disciplineTypes)
        {
            var dic = disciplineTypes.GroupBy(x => x.DisciplineType.Name).ToDictionary(x => x.Key, x => x.Count());
            return dic.Select(x => string.Format("{0} {1}", x.Key, x.Value)).JoinString(",");
        }
    }


    public class DisciplineInputModel
    {
        public Guid ClassPersonId { get; set; }
        public Guid ClassPeriodId { get; set; }
        public DateTime Date { get; set; }
        public GuidList DisciplineTypeIds { get; set; }
        public string Description { get; set; }
    }

    public class DisciplineListInputModel
    {
        public IList<DisciplineInputModel> Disciplines { get; set; } 
    }
}