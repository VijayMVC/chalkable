using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPersonEmailStorage : BaseDemoIntStorage<PersonEmail>
    {
        public DemoPersonEmailStorage()
            : base(null, true)
        {
        }

    }

    public class DemoPersonEmailService : DemoSchoolServiceBase, IPersonEmailService
    {
        private DemoPersonEmailStorage PersonEmailStorage { get; set; }
        public DemoPersonEmailService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PersonEmailStorage = new DemoPersonEmailStorage();
        }

        public static string BuildDemoEmail(int personId, string districtId)
        {
            return "demo-user_" + personId + "_" + districtId + "@chalkable.com";
        }

        public void AddPersonsEmails(IList<PersonEmail> personEmails)
        {
            PersonEmailStorage.Add(personEmails);
        }

        public void UpdatePersonsEmails(IList<PersonEmail> personEmails)
        {
            PersonEmailStorage.Update(personEmails);
        }

        public void DeletePersonsEmails(IList<PersonEmail> personEmails)
        {
            PersonEmailStorage.Delete(personEmails);
        }

        public PersonEmail GetPersonEmail(int personId)
        {
            return PersonEmailStorage.GetAll().FirstOrDefault(x => x.PersonRef == personId);
        }
    }
}
