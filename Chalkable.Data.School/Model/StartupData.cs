using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.Data.School.Model
{
    public class StartupData
    {
        public IList<AlphaGrade> AlphaGrades { get; set; }        
        public IList<AlternateScore> AlternateScores { get; set; } 
        public IList<MarkingPeriod> MarkingPeriods { get; set; } 
        public GradingPeriod GradingPeriod { get; set; }
        public IList<GradingPeriod> GradingPeriods { get; set; } 
        public PersonDetails Person { get; set; }
        public IList<ClassDetails> Classes { get; set; }
        public SchoolOption SchoolOption { get; set; }
        public IList<GradingComment> GradingComments { get; set; } 
        public IList<AttendanceReason> AttendanceReasons { get; set; } 
        public int UnshownNotificationsCount { get; set; }
        public IDictionary<int, IList<AlphaGrade>> AlphaGradesForClasses { get; set; }
        public IDictionary<int, IList<AlphaGrade>> AlphaGradesForClassStandards { get; set; }
        public IList<AlphaGradeDataAccess.SchoolAlphaGrade> AlphaGradesForSchoolStandards { get; set; } 
        public IList<Room> Rooms { get; set; } 
    }
}