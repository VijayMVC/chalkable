using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

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

        protected DisciplineView(ClassDisciplineDetails discipline, IList<ClassComplex> classes, bool canEdit)
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

        public static IList<DisciplineView> Create(IList<ClassDisciplineDetails> disciplines, IList<ClassComplex> classes,
                                            bool canEdit = false)
        {
            return disciplines.Select(x => new DisciplineView(x, classes, canEdit)).ToList();
        }
    }
}