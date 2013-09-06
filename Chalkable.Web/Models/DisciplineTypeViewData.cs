using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class DisciplineTypeViewData
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int Score { get; set; }

        public static DisciplineTypeViewData Create(DisciplineType disciplineType)
        {
            return new DisciplineTypeViewData
            {
                Id = disciplineType.Id,
                Name = disciplineType.Name,
                Score = disciplineType.Score,
            };
        }
        public static IList<DisciplineTypeViewData> Create(IList<DisciplineType> disciplineTypes)
        {
            return disciplineTypes.Select(Create).ToList();
        }
    }
}