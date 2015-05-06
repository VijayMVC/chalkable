using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class ShortStudentClassAttendanceSummary
    {
        public int? Tardies { get; set; }
        public decimal? Absences { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public decimal? Presents { get; set; }

        public decimal PosibleAttendanceCount
        {
            get 
            { 
                decimal sum = 0;
                if (Tardies.HasValue)
                    sum += Tardies.Value;
                if (Absences.HasValue)
                    sum += Absences.Value;
                if (Presents.HasValue)
                    sum += Presents.Value;
                return sum;
            }
        }

        protected ShortStudentClassAttendanceSummary(StudentSectionAbsenceSummary studentSectionAbsence)
        {
            Tardies = studentSectionAbsence.Tardies;
            Absences = studentSectionAbsence.Absences;
            StudentId = studentSectionAbsence.StudentId;
            ClassId = studentSectionAbsence.SectionId;
            Presents = studentSectionAbsence.Presents;
        }

        public static IList<ShortStudentClassAttendanceSummary> Create(IList<StudentSectionAbsenceSummary> studentSectionAttendances)
        {
            return studentSectionAttendances.Select(x => new ShortStudentClassAttendanceSummary(x)).ToList();
        }
    }

    public class StudentClassAttendanceSummary : ShortStudentClassAttendanceSummary
    {       
        public Class Class { get; set; }
        protected StudentClassAttendanceSummary(StudentSectionAbsenceSummary studentSectionAbsence, Class cClass)
            : base(studentSectionAbsence)
        {
            Class = cClass;
        }

        public static IList<StudentClassAttendanceSummary> Create(IList<StudentSectionAbsenceSummary> studentSectionAttendances, IList<ClassDetails> classes)
        {
            return studentSectionAttendances.Select(x =>
                {
                    var cClass = classes.FirstOrDefault(c => c.Id == x.SectionId);
                    return cClass == null ? null : new StudentClassAttendanceSummary(x, cClass);
                })
                .Where(x=> x != null).ToList();
        }
    }
}
