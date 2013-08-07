﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class CategoryServiceTest : MasterServiceTestBase
    {
        [Test]
        public void AddEditGetTest()
        {
            var category = SysAdminMasterLocator.CategoryService.Add("test", "test_descriptioin");
            Assert.AreEqual(category.Name, "test");
            Assert.AreEqual(category.Description, "test_descriptioin");
            AssertAreEqual(category, SysAdminMasterLocator.CategoryService.GetById(category.Id));
            var categories = SysAdminMasterLocator.CategoryService.ListCategories();
            Assert.AreEqual(categories.Count, 1);
            AssertAreEqual(category, categories[0]);

            category = SysAdminMasterLocator.CategoryService.Edit(category.Id, "test2", "test_description2");
            Assert.AreEqual(category.Name, "test2");
            Assert.AreEqual(category.Description, "test_description2");
            AssertAreEqual(category, SysAdminMasterLocator.CategoryService.GetById(category.Id));

            SysAdminMasterLocator.CategoryService.Delete(category.Id);
            AssertAreEqual(SysAdminMasterLocator.CategoryService.ListCategories().Count, 0);
            
        }
    }
}
