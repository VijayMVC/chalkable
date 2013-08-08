﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CourseViewData Course { get; set; }
        public GradeLevelViewData GradeLevel { get; set; }
        public ShortPersonViewData Teacher { get; set; }
        public IList<Guid> MarkingPeriodsId { get; set; }
        

        protected ClassViewData(ClassDetails classComplex)
        {
            Id = classComplex.Id;
            Name = classComplex.Name;
            Description = classComplex.Description;
            Course = CourseViewData.Create(classComplex.Course);
            GradeLevel =  GradeLevelViewData.Create(classComplex.GradeLevel);
            Teacher = ShortPersonViewData.Create(classComplex.Teacher);
            MarkingPeriodsId = classComplex.MarkingPeriodClasses.Select(x => x.MarkingPeriodRef).ToList();
            Teacher.DisplayName = classComplex.Teacher.ShortSalutationName;
        }
        private ClassViewData() {}

        public static ClassViewData Create(ClassDetails classComplex)
        {
           return new ClassViewData(classComplex);
        }
        public static IList<ClassViewData> Create(IList<ClassDetails> classComplexs)
        {
            return classComplexs.Select(Create).ToList();
        } 

        public static ClassViewData Create(Guid id, string name)
        {
            return  new ClassViewData{Id = id, Name = name};
        }
    }
}