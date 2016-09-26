using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassAlphaGradesViewData : ClassViewData
    {
        public IList<AlphaGradeViewData> AlphaGrades { get; set; }
        public IList<AlphaGradeViewData> AlphaGradesForStandards { get; set; }
        
        protected ClassAlphaGradesViewData(ClassDetails classComplex) : base(classComplex)
        {
        }

        public static ClassAlphaGradesViewData Create(ClassDetails classDetails, IList<AlphaGrade> alphaGrades, IList<AlphaGrade> alphaGradesForStandards)
        {
            return new ClassAlphaGradesViewData(classDetails)
            {
                AlphaGrades = AlphaGradeViewData.Create(alphaGrades),
                AlphaGradesForStandards = AlphaGradeViewData.Create(alphaGradesForStandards)
            };
        }
        
    }
}