using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class PhoneServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddEditDeletePhoneTest()
        {
            var phoneNumber = "huna+3809363236";
            var digitValue = "+3809363236";

            //add phone security check
            var phoneId = 1;
            AssertForDeny(sl => sl.PhoneService.Add(digitValue, FirstSchoolContext.FirstStudent.Id, phoneNumber, PhoneType.Home, true), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent
                | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondStudent);
            
            var phoneService = DistrictTestContext.DistrictLocatorFirstSchool.PhoneService;
            var phone = phoneService.Add(digitValue, FirstSchoolContext.AdminGrade.Id, phoneNumber, PhoneType.Home, true);
            Assert.IsTrue(phone.IsPrimary);
            Assert.AreEqual(phone.PersonRef, FirstSchoolContext.AdminGrade.Id);
            Assert.AreEqual(phone.Value, phoneNumber);
            Assert.AreEqual(phone.Type, PhoneType.Home);
            Assert.AreEqual(phone.DigitOnlyValue, digitValue);

            var phones = FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones();
            Assert.AreEqual(phones.Count, 1);
            AssertAreEqual(phone, phones[0]);
            var persons = FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue);
            Assert.AreEqual(persons.Count, 1);
            //AssertAreEqual(FirstSchoolContext.AdminGrade, persons[0]);
            Assert.AreEqual(FirstSchoolContext.AdminGrade.Id, persons[0].Id);
            var phone2 = phoneService.Add(digitValue, FirstSchoolContext.FirstTeacher.Id, phoneNumber, PhoneType.Home, true);
            var phone3 = phoneService.Add(digitValue + "7", FirstSchoolContext.FirstStudent.Id, phoneNumber + "7", PhoneType.Home, true);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 2);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue + "7").Count, 1);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

            phoneNumber += "8";
            digitValue += "8";
            //edit phone security check
            AssertForDeny(sl => sl.PhoneService.Edit(phone.DigitOnlyValue, phone.PersonRef, phoneNumber, PhoneType.Work, false), FirstSchoolContext
                , SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent
                | SchoolContextRoles.FirstTeacher |  SchoolContextRoles.Checkin);
            
            phone = phoneService.Edit(phone.DigitOnlyValue, phone.PersonRef, phoneNumber, PhoneType.Work, false);
            Assert.IsFalse(phone.IsPrimary);
            Assert.AreEqual(phone.PersonRef, FirstSchoolContext.AdminGrade.Id);
            Assert.AreEqual(phone.Value, phoneNumber);
            Assert.AreEqual(phone.DigitOnlyValue, digitValue);
            Assert.AreEqual(phone.Type, PhoneType.Work);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 1);
            //Assert.AreEqual(SecondSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 0);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

            //delete phone security check
            AssertForDeny(sl => sl.PhoneService.Delete(phone.DigitOnlyValue, phone.PersonRef), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
            
            phoneService.Delete(phone.DigitOnlyValue, phone.PersonRef);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 0);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 2);
            phoneService.Delete(phone2.DigitOnlyValue, phone2.PersonRef);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 1);
            phoneService.Delete(phone3.DigitOnlyValue, phone3.PersonRef);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 0);

        }
    }
}
