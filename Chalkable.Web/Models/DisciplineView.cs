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

        protected DisciplineView(ClassDisciplineDetails discipline, IList<ClassDetails> classes, bool canEdit)
        {
            StudentId = discipline.Student.Id;
            Period = PeriodViewData.Create(discipline.ClassPeriod.Period);
            DisciplineTypes = DisciplineTypeViewData.Create(discipline.DisciplineTypes.Select(x => x.DisciplineType).ToList());
            ClassName = discipline.Class.Name;
            TeacherId = discipline.Class.TeacherRef;
            ClassPeriodId = discipline.ClassPeriodRef;
            ClassPersonId = discipline.ClassPersonRef;
            Editable = canEdit || (classes != null && classes.Any(t => t.Name == ClassName));
        }

        public static IList<DisciplineView> Create(IList<ClassDisciplineDetails> disciplines, IList<ClassDetails> classes,
                                            bool canEdit = false)
        {
            return disciplines.Select(x => new DisciplineView(x, classes, canEdit)).ToList();
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
            ISet<Guid> studentIds = new HashSet<Guid>();
            var res = new List<StudentDisciplineSummaryViewData>();
            StudentDisciplineSummaryViewData disciplineView = null;
            var disciplineTypes = new List<ClassDisciplineTypeDetails>();
            foreach (var discipline in disciplines)
            {
                if (!studentIds.Contains(discipline.Student.Id))
                {
                    studentIds.Add(discipline.Id);
                    if (disciplineView != null)
                    {
                        disciplineView.Summary = BuildSummary(disciplineTypes);
                        disciplineView.Total = disciplineTypes.Sum(x => x.DisciplineType.Score);
                        disciplineView.DisciplineRecordsNumber = disciplineTypes.Count;
                        res.Add(disciplineView);
                    }
                    disciplineView = new StudentDisciplineSummaryViewData(discipline.Student);
                }
                disciplineTypes.AddRange(discipline.DisciplineTypes);           
            }
            return res;
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
        public GuidList DiscplineTypeIds { get; set; }
        public string Description { get; set; }
    }

    public class DisciplineListInputModel
    {
        public IList<DisciplineInputModel> Disciplines { get; set; } 
    }
}