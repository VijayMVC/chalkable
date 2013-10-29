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
//            AssertForDeny(sl => sl.AddressService.Add(SchoolTestContext.AdminGrade.Id, addressValue, "test", AddressType.Home), 
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(SchoolTestContext.AdminView.Id, addressValue, "test", AddressType.Home),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(SchoolTestContext.FirstTeacher.Id, addressValue, "test", AddressType.Home),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent 
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Add(SchoolTestContext.FirstStudent.Id, addressValue, "test", AddressType.Home),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher 
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            var address = SchoolTestContext.AdminGradeSl.AddressService.Add(SchoolTestContext.AdminGrade.Id, addressValue, "test", AddressType.Home);
//            Assert.AreEqual(address.Value, addressValue);
//            Assert.AreEqual(address.Note, "test");
//            Assert.AreEqual(address.Type, AddressType.Home);
//            Assert.AreEqual(address.PersonRef, SchoolTestContext.AdminGrade.Id);

//            var addresses = SchoolTestContext.AdminGradeSl.AddressService.GetAddress();
//            Assert.AreEqual(addresses.Count, 1);
//            AssertAreEqual(address, addresses[0]);

//            var address2 = SchoolTestContext.AdminGradeSl.AddressService.Add(SchoolTestContext.AdminView.Id, addressValue, "test", AddressType.Home);
//            var address3 = SchoolTestContext.AdminGradeSl.AddressService.Add(SchoolTestContext.FirstTeacher.Id, addressValue, "test", AddressType.Home);
//            var address4 = SchoolTestContext.AdminGradeSl.AddressService.Add(SchoolTestContext.FirstStudent.Id, addressValue, "test", AddressType.Home);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.AddressService.GetAddress().Count, 4);

//            //edit security check 
//            AssertForDeny(sl => sl.AddressService.Edit(address.Id, addressValue, "test", AddressType.Work),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address2.Id, addressValue, "test", AddressType.Work),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address3.Id, addressValue, "test", AddressType.Work),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Edit(address4.Id, addressValue, "test", AddressType.Work),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            addressValue += " test";
//            address = SchoolTestContext.AdminGradeSl.AddressService.Edit(address.Id, addressValue, "test2", AddressType.Work);
//            Assert.AreEqual(address.Value, addressValue);
//            Assert.AreEqual(address.Note, "test2");
//            Assert.AreEqual(address.Type, AddressType.Work);

//            //delete security check 
//            AssertForDeny(sl => sl.AddressService.Delete(address.Id),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address2.Id),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address3.Id),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
//                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AddressService.Delete(address4.Id),
//                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
//                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            SchoolTestContext.AdminGradeSl.AddressService.Delete(address.Id);
//            SchoolTestContext.AdminViewSl.AddressService.Delete(address2.Id);
//            SchoolTestContext.FirstTeacherSl.AddressService.Delete(address3.Id);
//            SchoolTestContext.FirstStudentSl.AddressService.Delete(address4.Id);
//            Assert.AreEqual(SchoolTestContext.AdminGradeSl.AddressService.GetAddress().Count, 0);

//        }
//    }
//}
