using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoEmailService : DemoMasterServiceBase, IEmailService
    {
        public DemoEmailService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }
        private const string confirmUrlFormat = "{0}/Home/Confirm?key={1}";
        public void SendResettedPasswordToPerson(Person person, string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public void SendResettedPasswordToDeveloper(Developer developer, string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public void SendChangedEmailToPerson(Person person, string newEmail)
        {
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var personMail = person.Email;
            var mail = PrepareDefaultMail(sysEMail);
            if (EmailTools.IsValidEmailAddress(personMail))
                mail.To.Add(personMail);
            else
            {
                Trace.TraceWarning(ChlkResources.ERR_EMAIL_INVALID, personMail);
                return;
            }
            var bodyTemplate = PreferenceService.Get(Preference.EMAIL_CHANGE_EMAIL_BODY).Value;
            mail.Body = string.Format(bodyTemplate, person.FirstName, newEmail);
            SendMail(mail, sysEMail);
        }

        public void SendInviteToPerson(Person person, string confirmationKey, string message, string messageTemplate)
        {
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var personMail = person.Email;
            var mail = PrepareDefaultMail(sysEMail);
            var user = ServiceLocator.UserService.GetByLogin(person.Email);
            var schoolName = user.SchoolUsers.First().School.Name;
            if (EmailTools.IsValidEmailAddress(personMail))
                mail.To.Add(personMail);
            else
            {
                Trace.TraceWarning(ChlkResources.ERR_EMAIL_INVALID, personMail);
                return;
            }
            mail.Subject = ChlkResources.EMAIL_CHALKABLE_WELCOME;
            string url = string.Format(confirmUrlFormat, PreferenceService.Get(Preference.APPLICATION_URL).Value, confirmationKey);
            mail.Body = string.Format(messageTemplate, person.FirstName, message ?? string.Empty, url, schoolName);
            SendMail(mail, sysEMail);
        }

        public void SendNotificationToPerson(Person person, string message)
        {
            if (person.Active)
            {
                var personMail = person.Email;
                var fromEmail = PreferenceService.GetTyped<EmailInfo>(Preference.NOTIFICATION_SYSTEM_EMAIL);
                var mail = PrepareDefaultMail(fromEmail);
                if (EmailTools.IsValidEmailAddress(personMail))
                    mail.To.Add(personMail);
                else
                {
                    Trace.TraceWarning(ChlkResources.ERR_EMAIL_INVALID, personMail);
                    return;
                }
                mail.Subject = ChlkResources.EMAIL_CHALKABLE_NOTIFICATIONS;
                mail.Body = message;
                SendMail(mail, fromEmail);
            }
            else
                Trace.TraceWarning(ChlkResources.ERR_EMAIL_NOTIFICATION_USER_IS_NOT_ACTIVE, person.Id);
        }

        public void SendMailToFriend(string fromMail, string toMail, string message, string subject = null)
        {
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var emailfrom = new EmailInfo {Email = fromMail};
            var mailMessage = PrepareDefaultMail(emailfrom);
            mailMessage.To.Add(new MailAddress(toMail));
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            SendMail(mailMessage, sysEMail);
        }

        private const string developerConfirmUrlFormat = "{0}/Developer/Confirm?key={1}&applicationId={2}";
        public void SendApplicationEmailToDeveloper(Application application)
        {

            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var mail = PrepareDefaultMail(sysEMail);
            var developer = application.Developer;
            switch (application.State)
            {
                case ApplicationStateEnum.Approved:
                    mail.Subject = PreferenceService.Get(Preference.APPLICATION_APPROVED_EMAIL_SUBJECT).Value;
                    var confirmationKey = developer.User.ConfirmationKey;
                    var url = string.Format(developerConfirmUrlFormat, PreferenceService.Get(Preference.APPLICATION_URL).Value
                        , confirmationKey, application.Id);
                    mail.Body = string.Format(PreferenceService.Get(Preference.APPLICATION_APPROVED_EMAIL_BODY).Value,
                        application.Name, developer.Name, url);
                    break;
                case ApplicationStateEnum.Rejected:
                    mail.Subject = PreferenceService.Get(Preference.APPLICATION_REJECTED_EMAIL_SUBJECT).Value;
                    mail.Body = string.Format(PreferenceService.Get(Preference.APPLICATION_REJECTED_EMAIL_BODY).Value,
                        application.Name, developer.DisplayName);
                    break;
                default: throw new ChalkableException(ChlkResources.EMAIL_INCORRECT_APP_STATE);
            }
            if (EmailTools.IsValidEmailAddress(developer.Email))
                mail.To.Add(developer.Email);
            SendMail(mail, sysEMail);
        }

        public void SendApplicationEmailToSysadmin(Application application)
        {
            throw new NotImplementedException();
        }
       
        private MailMessage PrepareDefaultMail(EmailInfo emailInfo)
        {
            var mailAdd = new MailAddress(emailInfo.Email, emailInfo.Email, Encoding.UTF8);
            var mail = new MailMessage
            {
                From = mailAdd,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            return mail;
        }

        private void SendMail(MailMessage mail, EmailInfo emailInfo)
        {
            //todo save mail somewhere
        }

 
    }
}
