using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class DepartmentServiceTest : MasterServiceTestBase
    {
        [Test]
        public void AddDeleteDepartmentTest()
        {

            //TODO: security check 

            var icon1 = PictureServiceTest.LoadImage(DefaulImage1Path);
            var department = SysAdminMasterLocator.ChalkableDepartmentService.Add("department1", "department1KeyWord", icon1);
            Assert.AreEqual(department.Name, "department1");
            Assert.AreEqual(department.Keywords, "department1KeyWord");
            Assert.AreEqual(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null), icon1);
            AssertAreEqual(department, SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(department.Id));
            Assert.AreEqual(SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Count, 1);
        
            var icon2 = PictureServiceTest.LoadImage(DefaulImage2Path);
            department = SysAdminMasterLocator.ChalkableDepartmentService.Edit(department.Id, "department2", "department2KeyWord", icon2);
            Assert.AreEqual(department.Name, "department2");
            Assert.AreEqual(department.Keywords, "department2KeyWord");
            Assert.AreEqual(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null), icon2);
            AssertAreEqual(department, SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(department.Id));
            
            SysAdminMasterLocator.ChalkableDepartmentService.Delete(department.Id);
            Assert.AreEqual(SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Count, 0);
            Assert.IsNull(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null));
        }
    }
}
