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
//    public class AddressServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddDeleteAddressTest()
//        {
//            var addressValue = "USA New York";
//            //security check
//            AssertForDeny(sl => sl.AddressService.Add(FirstSchoolContext.AdminGrade.Id, addressValue, "test", AddressType.Home), 
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(FirstSchoolContext.AdminView.Id, addressValue, "test", AddressType.Home),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(FirstSchoolContext.FirstTeacher.Id, addressValue, "test", AddressType.Home),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent 
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(FirstSchoolContext.FirstStudent.Id, addressValue, "test", AddressType.Home),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher 
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            var address = FirstSchoolContext.AdminGradeSl.AddressService.Add(FirstSchoolContext.AdminGrade.Id, addressValue, "test", AddressType.Home);
//            Assert.AreEqual(address.Value, addressValue);
//            Assert.AreEqual(address.Note, "test");
//            Assert.AreEqual(address.Type, AddressType.Home);
//            Assert.AreEqual(address.PersonRef, FirstSchoolContext.AdminGrade.Id);

//            var addresses = FirstSchoolContext.AdminGradeSl.AddressService.GetAddress();
//            Assert.AreEqual(addresses.Count, 1);
//            AssertAreEqual(address, addresses[0]);

//            var address2 = FirstSchoolContext.AdminGradeSl.AddressService.Add(FirstSchoolContext.AdminView.Id, addressValue, "test", AddressType.Home);
//            var address3 = FirstSchoolContext.AdminGradeSl.AddressService.Add(FirstSchoolContext.FirstTeacher.Id, addressValue, "test", AddressType.Home);
//            var address4 = FirstSchoolContext.AdminGradeSl.AddressService.Add(FirstSchoolContext.FirstStudent.Id, addressValue, "test", AddressType.Home);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AddressService.GetAddress().Count, 4);

//            //edit security check 
//            AssertForDeny(sl => sl.AddressService.Edit(address.Id, addressValue, "test", AddressType.Work),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address2.Id, addressValue, "test", AddressType.Work),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address3.Id, addressValue, "test", AddressType.Work),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address4.Id, addressValue, "test", AddressType.Work),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            addressValue += " test";
//            address = FirstSchoolContext.AdminGradeSl.AddressService.Edit(address.Id, addressValue, "test2", AddressType.Work);
//            Assert.AreEqual(address.Value, addressValue);
//            Assert.AreEqual(address.Note, "test2");
//            Assert.AreEqual(address.Type, AddressType.Work);

//            //delete security check 
//            AssertForDeny(sl => sl.AddressService.Delete(address.Id),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address2.Id),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address3.Id),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address4.Id),
//                FirstSchoolContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            FirstSchoolContext.AdminGradeSl.AddressService.Delete(address.Id);
//            FirstSchoolContext.AdminViewSl.AddressService.Delete(address2.Id);
//            FirstSchoolContext.FirstTeacherSl.AddressService.Delete(address3.Id);
//            FirstSchoolContext.FirstStudentSl.AddressService.Delete(address4.Id);
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AddressService.GetAddress().Count, 0);

//        }
//    }
//}
