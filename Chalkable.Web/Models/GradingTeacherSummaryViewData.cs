using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public static IList<GradingTeacherClassSummaryViewData> Create(IList<StudentGradeAvgPerClass> studentClassStats, IList<ClassDetails> classes)
        {
            return classes.Select(x => Create(x, studentClassStats)).ToList();
        }
        public static GradingTeacherClassSummaryViewData Create(ClassDetails classDetails, IList<StudentGradeAvgPerClass> studentClassStats)
        {
            var res = new GradingTeacherClassSummaryViewData {Class = ClassViewData.Create(classDetails)};
            studentClassStats = studentClassStats.Where(x => x.ClassRef == classDetails.Id)
                                                 .OrderBy(x => x.Avg)
                                                 .ThenBy(x => x.Student.LastName)
                                                 .ThenBy(x => x.Student.FirstName).ToList();     
            var well = new List<StudentGradeAvgPerClass>();
            var trouble = new List<StudentGradeAvgPerClass>();
            foreach (var studentStats in studentClassStats)
            {
                if(studentStats.Avg <= BAD_GRADE)
                    trouble.Add(studentStats);
                else well.Add(studentStats);
            }
            int skip = well.Count - DEFAUL_STUDENTS_COUNT > 0 ? well.Count - DEFAUL_STUDENTS_COUNT : 0; 
            res.AllStudents = ShortStudentGradingViewData.Create(studentClassStats);
            res.Trouble = ShortStudentGradingViewData.Create(trouble.Take(DEFAUL_STUDENTS_COUNT));
            res.Well = ShortStudentGradingViewData.Create(well.Skip(skip).Take(DEFAUL_STUDENTS_COUNT));
            return res;
        }
    }

    public class ShortStudentGradingViewData : ShortPersonViewData
    {
        public int? Avg { get; set; }
        protected ShortStudentGradingViewData(Person person) : base(person)
        {
        }

        public static ShortStudentGradingViewData Create(StudentGradeAvg studentGradeAvg)
        {
            return new ShortStudentGradingViewData(studentGradeAvg.Student) { Avg = studentGradeAvg.Avg };
        }
        public static IList<ShortStudentGradingViewData> Create(IEnumerable<StudentGradeAvgPerClass> studentGradeAvgs)
        {
            return studentGradeAvgs.Select(Create).ToList();
        }
    }
}