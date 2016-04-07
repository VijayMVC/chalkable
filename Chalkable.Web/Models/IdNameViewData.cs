using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class IdNameViewData<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; }
        protected IdNameViewData(TId id, string name)
        {
            Id = id;
            Name = name;
        }
        public static IdNameViewData<int> Create(int id, string name)
        {
            return new IdNameViewData<int>(id, name);
        }
    }

    public class GradeLevelViewData : IdNameViewData<int>
    {
        public int Number { get; set; }
        protected GradeLevelViewData(GradeLevel gradeLevel): base(gradeLevel.Id, gradeLevel.Name)
        {
            Number = gradeLevel.Number;
        }
        public static GradeLevelViewData Create(GradeLevel gradeLevel)
        {
            return new GradeLevelViewData(gradeLevel);
        }
        public static IList<GradeLevelViewData> Create(IList<GradeLevel> gradeLevels)
        {
            return gradeLevels.Select(Create).ToList();
        } 
    }
}