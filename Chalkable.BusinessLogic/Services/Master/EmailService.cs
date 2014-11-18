using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IEmailService
    {
        void SendResettedPasswordToPerson(User person, string confirmationKey);
        void SendResettedPasswordToDeveloper(Developer developer, string confirmationKey);
        void SendMailToFriend(string fromMail, string toMail, string message, string subject = null);
        void SendApplicationEmailToDeveloper(Application application);
        void SendApplicationEmailToSysadmin(Application application);
        void SendChangedEmailToPerson(Person person, string oldEmail, string newEmail);
    }

    public class EmailService : MasterServiceBase, IEmailService
    {
        public EmailService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }
        private const string confirmUrlFormat = "{0}/User/Confirm?key={1}";
        public void SendResettedPasswordToPerson(User user, string confirmationKey)
        {
            var bodyTemplate = PreferenceService.Get(Preference.RESETTED_PASSWORD_EMAIL_BODY).Value;
            var url = string.Format(confirmUrlFormat, PreferenceService.Get(Preference.APPLICATION_URL).Value, confirmationKey);
            var body = string.Format(bodyTemplate, user.FullName, url);
            SendResettedPassword(user.Login, body);
            user.LoginInfo.LastPasswordReset = DateTime.UtcNow;
        }
        
        public void SendResettedPasswordToDeveloper(Developer developer, string confirmationKey)
        {
            var bodyTemplate = PreferenceService.Get(Preference.RESETTED_PASSWORD_EMAIL_BODY).Value;
            var url = string.Format(confirmUrlFormat, PreferenceService.Get(Preference.APPLICATION_URL).Value, confirmationKey);
            var body = string.Format(bodyTemplate, developer.DisplayName, "", url);
            SendResettedPassword(developer.Email, body);
        }

        private void SendResettedPassword(string email, string body)
        {
            var fromEmail = PreferenceService.GetTyped<EmailInfo>(Preference.FORGOTPASSWORD_SYSTEM_EMAIL);
            var mail = PrepareMailToSysadmins(fromEmail);
            if (EmailTools.IsValidEmailAddress(email))
                mail.To.Add(email);
            else
            {
                Trace.TraceWarning(ChlkResources.ERR_EMAIL_INVALID, email);
                return;
            }
            mail.Subject = ChlkResources.EMAIL_CHALKABLE_PASSWORD_RESET;
            mail.Body = body;
            SendMail(mail, fromEmail);
        }

        public void SendChangedEmailToPerson(Person person, string oldEmail, string newEmail)
        {
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var personMail = oldEmail;
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

        private const string developerConfirmUrlFormat = "{0}/Developer/GoLive?key={1}&applicationId={2}";
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
            var developer = application.Developer;
            var sysEMail = PreferenceService.GetTyped<EmailInfo>(Preference.SYSTEM_EMAIL);
            var mail = PrepareMailToSysadmins(new EmailInfo { Email = developer.Email });
            switch (application.State)
            {
                case ApplicationStateEnum.SubmitForApprove:
                    mail.Subject = PreferenceService.Get(Preference.APPLICATION_SUBMITTED_EMAIL_SUBJECT).Value;
                    mail.Body = string.Format(PreferenceService.Get(Preference.APPLICATION_SUBMITTED_EMAIL_BODY).Value, application.Name);
                    break;
                case ApplicationStateEnum.Live:
                    mail.Subject = PreferenceService.Get(Preference.APPLICATION_GO_LIVE_EMAIL_SUBJECT).Value;
                    mail.Body = string.Format(PreferenceService.Get(Preference.APPLICATION_GO_LIVE_EMAIL_BODY).Value,
                                              application.Name, developer.DisplayName);
                    break;
                default: throw new ChalkableException(ChlkResources.EMAIL_INCORRECT_APP_STATE);
            }
            SendMail(mail, sysEMail);
        }


        private MailMessage PrepareMailToSysadmins(EmailInfo from)
        {
            var sysAdminEmail = ServiceLocator.UserService.GetSysAdmin().Login;
            var mail = PrepareDefaultMail(from);
            if (EmailTools.IsValidEmailAddress(sysAdminEmail))
                mail.To.Add(sysAdminEmail);
            else
                Trace.TraceWarning(ChlkResources.ERR_EMAIL_INVALID, sysAdminEmail);
            return mail;
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
            var client = new SmtpClient
                {
                    Credentials = new System.Net.NetworkCredential(emailInfo.UserName, emailInfo.Password),
                    Port = Int32.Parse(PreferenceService.Get(Preference.SERVER_PORT).Value),
                    Host = PreferenceService.Get(Preference.SERVER_NAME).Value,
                    EnableSsl = true
                };
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ChlkResources.ERR_EMAIL_WASNT_SEND);
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }
        }

    }
}
