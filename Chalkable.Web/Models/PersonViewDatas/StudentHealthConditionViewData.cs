using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentHealthConditionViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Treatment { get; set; }
        public string MedicationType { get; set; }
        public bool IsAlert { get; set; }

        public static StudentHealthConditionViewData Create(StudentHealthCondition condition)
        {
            return new StudentHealthConditionViewData
                {
                    Id = condition.Id,
                    Name = condition.Name,
                    Description = condition.Description,
                    IsAlert = condition.IsAlert,
                    MedicationType = condition.MedicationType,
                    Treatment = condition.Treatment
                };
        }

        public static IList<StudentHealthConditionViewData> Create(IList<StudentHealthCondition> conditions)
        {
            return conditions.Select(Create).ToList();
        }
    }
}