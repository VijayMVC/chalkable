using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class GradingTeacherClassSummaryViewData
    {
        private const int DEFAUL_STUDENTS_COUNT = 5;
        private const int BAD_GRADE = 65;

        public ClassViewData Class { get; set; }
        public IList<ShortStudentGradingViewData> Well { get; set; }
        public IList<ShortStudentGradingViewData> Trouble { get; set; }

        public IList<ShortStudentGradingViewData> AllStudents { get; set; }

        public static IList<GradingTeacherClassSummaryViewData> Create(IList<ShortClassGradesSummary> classGradesSummaries)
        {
            return classGradesSummaries.Select(Create).ToList();
        }
        public static GradingTeacherClassSummaryViewData Create(ShortClassGradesSummary classGradesSummary)
        {
            var res = new GradingTeacherClassSummaryViewData {Class = ClassViewData.Create(classGradesSummary.Class)};
            var studentsGrades = classGradesSummary.Students.Where(x=>x.Avg.HasValue).OrderBy(x => x.Avg)
                                                 .ThenBy(x => x.Student.LastName)
                                                 .ThenBy(x => x.Student.FirstName).ToList();
            var well = new List<ShortStudentClassGradesSummary>();
            var trouble = new List<ShortStudentClassGradesSummary>();
            foreach (var studentGrades in studentsGrades)
            {
                if (studentGrades.Avg <= BAD_GRADE)
                    trouble.Add(studentGrades);
                else well.Add(studentGrades);
            }
            int skip = well.Count - DEFAUL_STUDENTS_COUNT > 0 ? well.Count - DEFAUL_STUDENTS_COUNT : 0;
            res.AllStudents = ShortStudentGradingViewData.Create(studentsGrades);
            res.Trouble = ShortStudentGradingViewData.Create(trouble.Take(DEFAUL_STUDENTS_COUNT).ToList());
            res.Well = ShortStudentGradingViewData.Create(well.Skip(skip).Take(DEFAUL_STUDENTS_COUNT).ToList());
            return res;
        }
    }

    public class ShortStudentGradingViewData : StudentViewData
    {
        public decimal? Avg { get; set; }
        public bool Exempt { get; set; }
        protected ShortStudentGradingViewData(StudentDetails person) : base(person)
        {
        }

        public static ShortStudentGradingViewData Create(ShortStudentClassGradesSummary studentClassGrades)
        {
            return new ShortStudentGradingViewData(studentClassGrades.Student)
                {
                    Avg = studentClassGrades.Avg,
                    Exempt = studentClassGrades.Exempt
                };
        }

        public static IList<ShortStudentGradingViewData> Create(IList<ShortStudentClassGradesSummary> studentsClassGrades)
        {
            return studentsClassGrades.Select(Create).ToList();
        }
    }
}