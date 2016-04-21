using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentAppsViewData
    {
        public ShortPersonViewData Person { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserve { get; set; }
        public int InstalledAppsCount { get; set; }
        public PaginatedList<InstalledApplicationViewData> InstalledApps { get; set; }

        public static StudentAppsViewData Create(StudentDetails student, decimal currentBalance, PaginatedList<InstalledApplicationViewData> apps
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions)
        {
            return new StudentAppsViewData
            {
                Balance = currentBalance,
                Reserve = 0, // todo : gets this from funds 
                InstalledApps = apps,
                InstalledAppsCount = apps.TotalCount,
                Person = StudentProfileViewData.Create(student, customAlerts, healthConditions)
            };
        }
    }
}