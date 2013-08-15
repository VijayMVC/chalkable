using System.Collections.Generic;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonAppsViewData : ShortPersonViewData
    {
        public IList<InstalledApplicationViewData> InstalledApplication { get; set; }
        public decimal Balance { get; set; }
        public decimal? Reserve { get; set; }

        private PersonAppsViewData(Person person) : base(person) { }

        public static PersonAppsViewData Create( Person person, decimal? reserve, decimal balance, IList<Application> applications, IList<ApplicationInstall> appsInstalls)
        {
            return new PersonAppsViewData(person)
                {
                    Balance = balance,
                    InstalledApplication = InstalledApplicationViewData.Create(appsInstalls, person, applications),
                    Reserve = reserve
                };
        }
    }
}