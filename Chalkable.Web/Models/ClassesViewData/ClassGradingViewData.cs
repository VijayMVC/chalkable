using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassGradingViewData : ClassViewData
    {
        public IList<GradingClassSummaryViewData> GradingPerMp { get; set; }

        protected ClassGradingViewData(ClassDetails classComplex) : base(classComplex) { }

        public static ClassGradingViewData Create(ClassDetails classComplex, IList<GradingClassSummaryViewData> gradingPerMp)
        {
            return new ClassGradingViewData(classComplex) { GradingPerMp = gradingPerMp };
        }
    }
}