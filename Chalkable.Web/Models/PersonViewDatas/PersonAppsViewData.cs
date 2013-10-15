using System.Collections.Generic;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonAppsViewData : ShortPersonViewData
    {
        public AppsBudgetViewData AppsBudget { get; set; }
        private PersonAppsViewData(Person person) : base(person) { }
        
        public static PersonAppsViewData Create( Person person, decimal? reserve, decimal balance, IList<Application> applications, IList<ApplicationInstall> appsInstalls)
        {
            var installedAppViewData = InstalledApplicationViewData.Create(appsInstalls, person, applications);
            return new PersonAppsViewData(person){ AppsBudget = AppsBudgetViewData.Create(balance, reserve, installedAppViewData) };
        }
    }
}