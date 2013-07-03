using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class DepartmentServiceTest : MasterServiceTestBase
    {
        [Test]
        public void AddDeleteDepartmentTest()
        {

            //TODO: security test 

            var icon1 = PictureServiceTest.LoadImage(DefaulImage1Path);
            IList<string> keywords1 = new List<string>{"dep1", "dep2"};
            var department = SysAdminMasterLocator.ChalkableDepartmentService.Add("department1", keywords1, icon1);
            Assert.AreEqual(department.Name, "department1");
            Assert.AreEqual(department.Keywords, keywords1.JoinString(","));
            Assert.AreEqual(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null), icon1);
            AssertAreEqual(department, SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(department.Id));
            Assert.AreEqual(SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Count, 1);
        
            var icon2 = PictureServiceTest.LoadImage(DefaulImage2Path);
            IList<string> keywords2 = new List<string> { "dep3", "dep4" };
            department = SysAdminMasterLocator.ChalkableDepartmentService.Edit(department.Id, "department2", keywords2, icon2);
            Assert.AreEqual(department.Name, "department2");
            Assert.AreEqual(department.Keywords, keywords2.JoinString(","));
            Assert.AreEqual(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null), icon2);
            AssertAreEqual(department, SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(department.Id));
            
            SysAdminMasterLocator.ChalkableDepartmentService.Delete(department.Id);
            Assert.AreEqual(SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Count, 0);
            Assert.IsNull(SysAdminMasterLocator.DepartmentIconService.GetPicture(department.Id, null, null));
        }
    }
}
