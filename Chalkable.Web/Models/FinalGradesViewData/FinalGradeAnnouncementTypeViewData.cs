using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class FinalGradeAnnouncementTypeViewData
    {
        public Guid Id { get; set; }
        public string TypeName { get; set; }
        public int Value { get; set; }
        public bool DropLowest { get; set; }
        public int GradingStyle { get; set; }

        protected FinalGradeAnnouncementTypeViewData(FinalGradeAnnouncementType finalGradeAnnType)
        {
            Id = finalGradeAnnType.Id;
            DropLowest = finalGradeAnnType.DropLowest;
            GradingStyle = (int) finalGradeAnnType.GradingStyle;
            TypeName = finalGradeAnnType.AnnouncementType.Name;
            Value = finalGradeAnnType.PercentValue;
        }

        public static FinalGradeAnnouncementTypeViewData Create(FinalGradeAnnouncementType finalGradeAnnType)
        {
            return new FinalGradeAnnouncementTypeViewData(finalGradeAnnType);
        }
        public static IList<FinalGradeAnnouncementTypeViewData> Create(IList<FinalGradeAnnouncementType> finalGradeAnnTypes)
        {
            return finalGradeAnnTypes.Select(Create).ToList();
        }
    }
}