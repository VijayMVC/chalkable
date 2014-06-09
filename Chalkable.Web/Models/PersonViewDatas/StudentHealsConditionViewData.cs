using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentHealsConditionViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Treatment { get; set; }
        public string MedicationType { get; set; }
        public bool IsAlert { get; set; }

        public static StudentHealsConditionViewData Create(StudentHealsCondition condition)
        {
            return new StudentHealsConditionViewData
                {
                    Id = condition.Id,
                    Name = condition.Name,
                    Description = condition.Description,
                    IsAlert = condition.IsAlert,
                    MedicationType = condition.MedicationType,
                    Treatment = condition.Treatment
                };
        }

        public static IList<StudentHealsConditionViewData> Create(IList<StudentHealsCondition> conditions)
        {
            return conditions.Select(Create).ToList();
        }
    }
}