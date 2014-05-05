using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;


namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class DisciplineView
    {
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public int? ClassId { get; set; }
        public ShortPersonViewData Student { get; set; }
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public IList<DisciplineTypeViewData> DisciplineTypes { get; set; }
        public string Description { get; set; }
        public bool Editable { get; set; }

        protected DisciplineView(ClassDisciplineDetails discipline, int currentPersonId, bool canEdit)
        {
            Id = discipline.Id;
            StudentId = discipline.StudentId;
            Student = ShortPersonViewData.Create(discipline.Student);
            DisciplineTypes = DisciplineTypeViewData.Create(discipline.Infractions.ToList());
            ClassName = discipline.Class.Name;
            TeacherId = discipline.Class.TeacherRef;
            Editable = canEdit || currentPersonId == TeacherId;
            Description = discipline.Description;
            ClassId = discipline.ClassId;

        }

        public static DisciplineView Create(ClassDisciplineDetails discipline, int currentPersonId, bool canEdit = false)
        {
            return  new DisciplineView(discipline, currentPersonId, canEdit);
        }

        public static IList<DisciplineView> Create(IList<ClassDisciplineDetails> disciplines, int currentPersonId, bool canEdit = false)
        {
            return disciplines.Select(x => new DisciplineView(x, currentPersonId, canEdit)).ToList();
        }
    }
}