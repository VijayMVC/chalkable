﻿using System;
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
        public IList<StudentGradingSummaryViewData> Well { get; set; }
        public IList<StudentGradingSummaryViewData> Trouble { get; set; }

        public IList<StudentGradingSummaryViewData> AllStudents { get; set; }

        public static IList<GradingTeacherClassSummaryViewData> Create(IList<StudentGradeAvgPerClass> studentClassStats, IList<ClassDetails> classes)
        {
            return classes.Select(x => Create(x, studentClassStats)).ToList();
        }
        public static GradingTeacherClassSummaryViewData Create(ClassDetails classDetails, IList<StudentGradeAvgPerClass> studentClassStats)
        {
            var res = new GradingTeacherClassSummaryViewData {Class = ClassViewData.Create(classDetails)};
            studentClassStats = studentClassStats.Where(x => x.ClassRef == classDetails.Id).OrderByDescending(x => x.Avg).ToList();     
            var well = new List<StudentGradeAvgPerClass>();
            var trouble = new List<StudentGradeAvgPerClass>();
            foreach (var studentStats in studentClassStats)
            {
                if(studentStats.Avg <= BAD_GRADE)
                    trouble.Add(studentStats);
                else well.Add(studentStats);
            }
            well.Reverse();
            res.AllStudents = StudentGradingSummaryViewData.Create(studentClassStats);
            res.Trouble = StudentGradingSummaryViewData.Create(well.Take(DEFAUL_STUDENTS_COUNT));
            res.Well = StudentGradingSummaryViewData.Create(trouble.Take(DEFAUL_STUDENTS_COUNT));
            return res;
        }
    }

    public class StudentGradingSummaryViewData : ShortPersonViewData
    {
        public int? Avg { get; set; }
        protected StudentGradingSummaryViewData(Person person) : base(person)
        {
        }

        public static StudentGradingSummaryViewData Create(StudentGradeAvg studentGradeAvg)
        {
            return new StudentGradingSummaryViewData(studentGradeAvg.Student) { Avg = studentGradeAvg.Avg };
        }
        public static IList<StudentGradingSummaryViewData> Create(IEnumerable<StudentGradeAvgPerClass> studentGradeAvgs)
        {
            return studentGradeAvgs.Select(Create).ToList();
        }
    }
}