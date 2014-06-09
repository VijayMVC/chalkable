using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class FinalGradeInfo
    {
        public IList<FinalStudentAttendance> Attendances { get; set; }
        public IList<FinalStudentDiscipline> Disciplines { get; set; }
        public ChalkableGradeBook GradeBook { get; set; }
        public IList<ChalkableAverage> Averages { get; set; }
    }

    public class FinalStudentAttendance
    {
        public int StudentId { get; set; }
        public int Tardies { get; set; }
        public decimal Absenses { get; set; }
        public decimal Presents { get; set; }

        public static IList<FinalStudentAttendance> Create(IList<StudentTotalSectionAttendance> sectionAttendances)
        {
            return sectionAttendances.Select(x => new FinalStudentAttendance
                {
                    StudentId = x.StudentId,
                    Absenses = x.Absences,
                    Presents = x.DaysPresent,
                    Tardies = x.Tardies
                }).ToList();
        } 
    }

    public class FinalStudentDiscipline
    {
        public int Occurrences { get; set; }
        public Data.School.Model.Infraction Infraction { get; set; }
        public int StudentId { get; set; }

        public static IList<FinalStudentDiscipline> Create(IList<StudentDisciplineSummary> disciplines
            ,  IList<Data.School.Model.Infraction> infractions)
        {
            var res = new List<FinalStudentDiscipline>();
            foreach (var discipline in disciplines)
            {
                var infraction = infractions.FirstOrDefault(x => x.Id == discipline.InfractionId);
                if(infraction == null) continue;
                res.Add(new FinalStudentDiscipline
                {
                    Occurrences = discipline.Occurrences,
                    Infraction = infraction,
                    StudentId = discipline.StudentId
                });
            }
            return res;
        }
    }
}
