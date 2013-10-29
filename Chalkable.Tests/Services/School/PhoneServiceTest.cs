//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class PhoneServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddEditDeletePhoneTest()
//        {
//            var phoneNumber = "huna+3809363236";
//            var digitValue = "+3809363236";
            
//            //add phone security check
//            AssertForDeny(sl => sl.PhoneService.Add(SchoolTestContext.FirstStudent.Id, phoneNumber, PhoneType.Home, true), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondStudent);
//            AssertForDeny(sl => sl.PhoneService.Add(SchoolTestContext.FirstTeacher.Id, phoneNumber, PhoneType.Home, true), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondStudent);
//            AssertForDeny(sl => sl.PhoneService.Add(SchoolTestContext.AdminGrade.Id, phoneNumber,  PhoneType.Home, true), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondStudent);

//            var phone = SchoolTestContext.AdminGradeSl.PhoneService.Add(SchoolTestContext.AdminGrade.Id, phoneNumber, PhoneType.Home, true);
//            Assert.IsTrue(phone.IsPRIMARY);
//            Assert.AreEqual(phone.PersonRef, SchoolTestContext.AdminGrade.Id);
//            Assert.AreEqual(phone.Value, phoneNumber);
//            Assert.AreEqual(phone.Type,  PhoneType.Home);
//            Assert.AreEqual(phone.DigitOnlyValue, digitValue);
            
//            var phones = SchoolTestContext.AdminGradeSl.PhoneService.GetPhones();
//            Assert.AreEqual(phones.Count, 1);
//            AssertAreEqual(phone, phones[0]);
//            var persons = SchoolTestContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue);
//            Assert.AreEqual(persons.Count, 1);
//            AssertAreEqual(SchoolTestContext.AdminGrade, persons[0]);
//            var phone2 = SchoolTestContext.AdminGradeSl.PhoneService.Add(SchoolTestContext.FirstTeacher.Id, phoneNumber, PhoneType.Home, true);
//            var phone3 = SchoolTestContext.AdminGradeSl.PhoneService.Add(SchoolTestContext.FirstStudent.Id, phoneNumber + "7", PhoneType.Home, true);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 2);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue + "7").Count, 1);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

//            phoneNumber +=  "8";
//            digitValue += "8";
//            //edit phone security check
//            AssertForDeny(sl => sl.PhoneService.Edit(phone.Id, phoneNumber, PhoneType.Work, false), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Edit(phone2.Id, phoneNumber, PhoneType.Work, false), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Edit(phone3.Id, phoneNumber, PhoneType.Work, false), SchoolTestContext
//                            , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.Checkin);

//            phone = SchoolTestContext.AdminGradeSl.PhoneService.Edit(phone.Id, phoneNumber, PhoneType.Work, false);
//            Assert.IsFalse(phone.IsPRIMARY);
//            Assert.AreEqual(phone.PersonRef, SchoolTestContext.AdminGrade.Id);
//            Assert.AreEqual(phone.Value, phoneNumber);
//            Assert.AreEqual(phone.DigitOnlyValue, digitValue);
//            Assert.AreEqual(phone.Type, PhoneType.Work);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 1);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

//            //delete phone security check
//            AssertForDeny(sl => sl.PhoneService.Delete(phone.Id), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Delete(phone2.Id), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Delete(phone3.Id), SchoolTestContext
//                            , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.Checkin);

//            SchoolTestContext.AdminGradeSl.PhoneService.Delete(phone.Id);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 0);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetPhones().Count, 2);
//            SchoolTestContext.FirstTeacherSl.PhoneService.Delete(phone2.Id);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetPhones().Count, 1);
//            SchoolTestContext.FirstStudentSl.PhoneService.Delete(phone3.Id);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.PhoneService.GetPhones().Count, 0);
            
//        }
//    }
//}
