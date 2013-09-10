using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class FinalGradeAnnTypeGradeStatsViewData : FinalGradeAnnouncementTypeViewData
    {
        IList<GradingStatsByDateViewData> GradePerDate { get; set; }

        protected FinalGradeAnnTypeGradeStatsViewData(FinalGradeAnnouncementType finalGradeAnnType)
            : base(finalGradeAnnType)
        {
        }

        public static IList<FinalGradeAnnTypeGradeStatsViewData> Create(IList<AnnTypeGradeStats> annTypesGradeStats, IList<FinalGradeAnnouncementType> fgAnnTypes)
        {
            var res = new List<FinalGradeAnnTypeGradeStatsViewData>();
            foreach (var annTypeGradeStats in annTypesGradeStats)
            {
                var fgAnnType = fgAnnTypes.FirstOrDefault(x => x.AnnouncementTypeRef == annTypeGradeStats.AnnouncementTypeId);
                res.Add(new FinalGradeAnnTypeGradeStatsViewData(fgAnnType)
                {
                    GradePerDate = GradingStatsByDateViewData.Create(annTypeGradeStats.GradeAvgPerDates)
                });
            }
            return res;
        }

    }
}