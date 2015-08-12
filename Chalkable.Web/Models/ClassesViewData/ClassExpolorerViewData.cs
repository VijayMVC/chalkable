using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassExpolorerViewData : ClassViewData
    {
        public IList<ClassStandardViewData> Standards { get; set; } 

        protected ClassExpolorerViewData(ClassDetails classComplex) : base(classComplex)
        {
        }

        public static ClassExpolorerViewData Create(ClassDetails classDetails, IList<GradingStandardInfo> gradingStandards)
        {
            var res = new ClassExpolorerViewData(classDetails)
                {
                    Standards = new List<ClassStandardViewData>()
                };
            var standardsDic = gradingStandards.GroupBy(x => x.Standard.Id).ToDictionary(x => x.Key, x => x);
            IList<ClassStandardViewData> graded = new List<ClassStandardViewData>();
            IList<ClassStandardViewData> notGraded =new List<ClassStandardViewData>();
            foreach (var item in standardsDic)
            {
                var standard = item.Value.First().Standard;
                var classStandardView = ClassStandardViewData.Create(standard, item.Value.ToList());
                if(classStandardView.NumericGrade.HasValue)
                    graded.Add(classStandardView);
                else notGraded.Add(classStandardView);
            }
            graded = graded.OrderBy(x => x.NumericGrade).ThenBy(x => x.Name).ToList();
            res.Standards = graded.Concat(notGraded.OrderBy(x => x.Name)).ToList();
            return res;
        }
    }

    public class ClassStandardViewData : StandardViewData
    {
        public decimal? NumericGrade { get; set; }

        protected ClassStandardViewData(Standard standard) : base(standard) {}

        public static ClassStandardViewData Create(Standard standard, IList<GradingStandardInfo> gradingStandards)
        {
            var res = new ClassStandardViewData(standard);
            var gStandards = gradingStandards.Where(x => x.Standard.Id == standard.Id).ToList();
            if (gStandards.Count > 0)
                res.NumericGrade = gStandards.Average(x => x.NumericGrade);
            return res;
        }
    }
}