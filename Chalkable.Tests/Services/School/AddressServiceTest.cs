using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AddressServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void TestAddress()
        {
            var addressValue = "USA New York";
            //security check
            AssertForDeny(sl=>sl.AddressSerivce.Add(SchoolTestContext.AdminGrade.Id, addressValue, "test", AddressType.Home), 
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Add(SchoolTestContext.AdminView.Id, addressValue, "test", AddressType.Home),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Add(SchoolTestContext.FirstTeacher.Id, addressValue, "test", AddressType.Home),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent 
                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Add(SchoolTestContext.FirstStudent.Id, addressValue, "test", AddressType.Home),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher 
                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var address = SchoolTestContext.AdminGradeSl.AddressSerivce.Add(SchoolTestContext.AdminGrade.Id, addressValue, "test", AddressType.Home);
            Assert.AreEqual(address.Value, addressValue);
            Assert.AreEqual(address.Note, "test");
            Assert.AreEqual(address.Type, AddressType.Home);
            Assert.AreEqual(address.PersonRef, SchoolTestContext.AdminGrade.Id);

            var addresses = SchoolTestContext.AdminGradeSl.AddressSerivce.GetAddress();
            Assert.AreEqual(addresses.Count, 1);
            AssertAreEqual(address, addresses[0]);

            var address2 = SchoolTestContext.AdminGradeSl.AddressSerivce.Add(SchoolTestContext.AdminView.Id, addressValue, "test", AddressType.Home);
            var address3 = SchoolTestContext.AdminGradeSl.AddressSerivce.Add(SchoolTestContext.FirstTeacher.Id, addressValue, "test", AddressType.Home);
            var address4 = SchoolTestContext.AdminGradeSl.AddressSerivce.Add(SchoolTestContext.FirstStudent.Id, addressValue, "test", AddressType.Home);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.AddressSerivce.GetAddress().Count, 4);

            //edit security check 
            AssertForDeny(sl => sl.AddressSerivce.Edit(address.Id, addressValue, "test", AddressType.Work),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Edit(address2.Id, addressValue, "test", AddressType.Work),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Edit(address3.Id, addressValue, "test", AddressType.Work),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Edit(address4.Id, addressValue, "test", AddressType.Work),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            addressValue += " test";
            address = SchoolTestContext.AdminGradeSl.AddressSerivce.Edit(address.Id, addressValue, "test2", AddressType.Work);
            Assert.AreEqual(address.Value, addressValue);
            Assert.AreEqual(address.Note, "test2");
            Assert.AreEqual(address.Type, AddressType.Work);

            //delete security check 
            AssertForDeny(sl => sl.AddressSerivce.Delete(address.Id),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Delete(address2.Id),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Delete(address3.Id),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
                                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AddressSerivce.Delete(address4.Id),
                SchoolTestContext, SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher
                                 | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            SchoolTestContext.AdminGradeSl.AddressSerivce.Delete(address.Id);
            SchoolTestContext.AdminViewSl.AddressSerivce.Delete(address2.Id);
            SchoolTestContext.FirstTeacherSl.AddressSerivce.Delete(address3.Id);
            SchoolTestContext.FirstStudentSl.AddressSerivce.Delete(address4.Id);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.AddressSerivce.GetAddress().Count, 0);

        }
    }
}
