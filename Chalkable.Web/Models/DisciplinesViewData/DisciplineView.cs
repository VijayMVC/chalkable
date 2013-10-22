using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;


namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class DisciplineView
    {
        public ShortPersonViewData Student { get; set; }
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
            Student = ShortPersonViewData.Create(discipline.Student);
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
}