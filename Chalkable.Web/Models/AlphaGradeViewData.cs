using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Web.Models
{
    public class AlphaGradeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static AlphaGradeViewData Create(AlphaGrade alphaGrade)
        {
            return new AlphaGradeViewData
                {
                    Id = alphaGrade.Id,
                    Name = alphaGrade.Name,
                    Description = alphaGrade.Description,
                };
        }
        public static IList<AlphaGradeViewData> Create(IList<AlphaGrade> alphaGrades)
        {
            return alphaGrades.Select(Create).ToList();
        } 
    }
}