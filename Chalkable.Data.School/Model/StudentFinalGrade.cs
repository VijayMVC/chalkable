using System;

namespace Chalkable.Data.School.Model
{
    public class StudentFinalGrade
    {
        public Guid Id { get; set; }
        public Guid FinalGradeRef { get; set; }
        public Guid ClassPersonRef { get; set; }
        public int? TeacherGrade { get; set; }
        public int? AdminGrade { get; set; }
        public string Comment { get; set; }
        public int? GradeByAnnouncement { get; set; }
        public int? GradeByParticipation { get; set; }
        public int? GradeByAttendance { get; set; }
        public int? GradeByDiscipline { get; set; }
    }

    public class StudentFinalGradeDetails : StudentFinalGrade
    {
        public Person Student { get; set; }
        public FinalGrade FinalGrade { get; set; }
    }
}
