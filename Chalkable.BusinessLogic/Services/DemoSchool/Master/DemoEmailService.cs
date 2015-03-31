using System;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoEmailService : DemoMasterServiceBase, IEmailService
    {
        public DemoEmailService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void SendResettedPasswordToPerson(User person, string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public void SendResettedPasswordToDeveloper(Developer developer, string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public void SendChangedEmailToPerson(Person person, string newEmail)
        {
            throw new NotImplementedException();
        }

        public void SendInviteToPerson(Person person, string confirmationKey, string message, string messageTemplate)
        {
            
        }

        public void SendNotificationToPerson(Person person, string message)
        {
            
        }

        public void SendMailToFriend(string fromMail, string toMail, string message, string subject = null)
        {
            
        }

        public void SendApplicationEmailToDeveloper(Application application)
        {
        }

        public void SendApplicationEmailToSysadmin(Application application)
        {
            throw new NotImplementedException();
        }

        public void SendChangedEmailToPerson(Person person, string oldEmail, string newEmail)
        {
            throw new NotImplementedException();
        }
 
    }
}
