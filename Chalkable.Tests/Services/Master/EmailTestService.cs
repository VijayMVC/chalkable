using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Tests.Services.Master
{
    public class EmailTestService : MasterServiceBase, IEmailService
    {

        private IDictionary<EmailResultTypeEnum, EmailResultTestModel> emailResult = new Dictionary<EmailResultTypeEnum, EmailResultTestModel>();

        public EmailTestService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private Pair<string, string> PrepareEmailToSysAdmin()
        {
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var sysadminMail = ServiceLocator.UserService.GetSysAdmin();
            return new Pair<string, string>(sysEMail.Email, sysadminMail.Login);
        }
        private void PreperinResult(EmailResultTypeEnum type, EmailResultTestModel result)
        {
            if (!emailResult.ContainsKey(type))
                emailResult.Add(type, result);
            emailResult[type] = result;
        }

        public EmailResultTestModel GetEmailResult(EmailResultTypeEnum typeResult)
        {
            if (!emailResult.ContainsKey(typeResult))
                throw new ChalkableException();
            return emailResult[typeResult];
        }

        public void SendInviteToPerson(Person person, string confirmationKey, string message, string messageTemplate)
        {
            return;
        }

        public void SendChangedEmailToPerson(Person person, string newEmail)
        {
            return;
        }

        public void SendResettedPasswordToPerson(Person person, string confirmationKey)
        {
            return;
        }

        public void SendNotificationToPerson(Person person, string message)
        {
            return;
        }

        public void SendMailToFriend(string fromMail, string toMail, string message, string subject = null)
        {
            return;
        }

        public void SendApplicationEmailToDeveloper(Application application)
        {
            return;
        }

        public void SendApplicationEmailToSysadmin(Application application)
        {
            return;
        }
    }


    public class EmailResultTestModel
    {
        public const string DEFUAL_MESSAGE = "new mail was send";

        public string FromPerson { get; set; }
        public string ToPerson { get; set; }
        public string Message { get; set; }

        //public IDictionary<ImportTypeEnum, CsvContainer> ImportContainers { get; set; }
        public EmailResultTestModel()
        {
            Message = DEFUAL_MESSAGE;
        }
    }

    public enum EmailResultTypeEnum
    {
        SetupDataUploaded = 1,
        RegistrationToSysAdmins,
        SchoolRegistrationBegin,
        SchoolSetupFirst,
        SchoolSetupSecond,
        InviteToPerson,
        NotificationToPerson,
        ActionLink,
        ResettedPasswordToPerson,
        PurchaseOrderConfirmationToPerson,
        MailToFrined,
        ApplicationEmailToSysAdmin,
        ApplicationEmailToDeveloper
    }
}
