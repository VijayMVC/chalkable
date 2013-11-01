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
//            AssertForDeny(sl => sl.PhoneService.Add(FirstSchoolContext.FirstStudent.Id, phoneNumber, PhoneType.Home, true), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondStudent);
//            AssertForDeny(sl => sl.PhoneService.Add(FirstSchoolContext.FirstTeacher.Id, phoneNumber, PhoneType.Home, true), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondStudent);
//            AssertForDeny(sl => sl.PhoneService.Add(FirstSchoolContext.AdminGrade.Id, phoneNumber,  PhoneType.Home, true), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondStudent);

//            var phone = FirstSchoolContext.AdminGradeSl.PhoneService.Add(FirstSchoolContext.AdminGrade.Id, phoneNumber, PhoneType.Home, true);
//            Assert.IsTrue(phone.IsPRIMARY);
//            Assert.AreEqual(phone.PersonRef, FirstSchoolContext.AdminGrade.Id);
//            Assert.AreEqual(phone.Value, phoneNumber);
//            Assert.AreEqual(phone.Type,  PhoneType.Home);
//            Assert.AreEqual(phone.DigitOnlyValue, digitValue);
            
//            var phones = FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones();
//            Assert.AreEqual(phones.Count, 1);
//            AssertAreEqual(phone, phones[0]);
//            var persons = FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue);
//            Assert.AreEqual(persons.Count, 1);
//            AssertAreEqual(FirstSchoolContext.AdminGrade, persons[0]);
//            var phone2 = FirstSchoolContext.AdminGradeSl.PhoneService.Add(FirstSchoolContext.FirstTeacher.Id, phoneNumber, PhoneType.Home, true);
//            var phone3 = FirstSchoolContext.AdminGradeSl.PhoneService.Add(FirstSchoolContext.FirstStudent.Id, phoneNumber + "7", PhoneType.Home, true);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 2);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue + "7").Count, 1);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

//            phoneNumber +=  "8";
//            digitValue += "8";
//            //edit phone security check
//            AssertForDeny(sl => sl.PhoneService.Edit(phone.Id, phoneNumber, PhoneType.Work, false), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Edit(phone2.Id, phoneNumber, PhoneType.Work, false), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Edit(phone3.Id, phoneNumber, PhoneType.Work, false), FirstSchoolContext
//                            , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.Checkin);

//            phone = FirstSchoolContext.AdminGradeSl.PhoneService.Edit(phone.Id, phoneNumber, PhoneType.Work, false);
//            Assert.IsFalse(phone.IsPRIMARY);
//            Assert.AreEqual(phone.PersonRef, FirstSchoolContext.AdminGrade.Id);
//            Assert.AreEqual(phone.Value, phoneNumber);
//            Assert.AreEqual(phone.DigitOnlyValue, digitValue);
//            Assert.AreEqual(phone.Type, PhoneType.Work);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 3);

//            //delete phone security check
//            AssertForDeny(sl => sl.PhoneService.Delete(phone.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Delete(phone2.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.PhoneService.Delete(phone3.Id), FirstSchoolContext
//                            , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.Checkin);

//            FirstSchoolContext.AdminGradeSl.PhoneService.Delete(phone.Id);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetUsersByPhone(digitValue).Count, 0);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 2);
//            FirstSchoolContext.FirstTeacherSl.PhoneService.Delete(phone2.Id);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 1);
//            FirstSchoolContext.FirstStudentSl.PhoneService.Delete(phone3.Id);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PhoneService.GetPhones().Count, 0);
            
//        }
//    }
//}
