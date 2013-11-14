using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AddressServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteAddressTest()
        {
            var addressInfo = new AddressInfo
                {
                    AddressLine1 = "first address line test",
                    AddressLine2 = "secondAddressLine",
                    Country = "USA",
                    AddressNumber = "1",
                    City = "New York",
                    CountyId = 1,
                    Id = 1,
                    Latitude = 20,
                    Longitude = 20,
                    PostalCode = "1010",
                    State = "test1",
                    StreetNumber = "1"
                };
            //security check
            AssertForDeny(sl => sl.AddressService.Add(addressInfo), FirstSchoolContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | 
                SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var addressService = DistrictTestContext.DistrictLocatorFirstSchool.AddressService;
            var address = addressService.Add(addressInfo);
            AssertAddressEqual(addressInfo, address);
            var addresses = FirstSchoolContext.AdminGradeSl.AddressService.GetAddress();
            Assert.AreEqual(addresses.Count, 1);
            AssertAreEqual(address, addresses[0]);

            addressInfo.Id = 2;
            addressInfo.Longitude = 21;
            addressInfo.Latitude = 21;
            addressInfo.StreetNumber = "2";
            var address2 = addressService.Add(addressInfo);
            
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AddressService.GetAddress().Count, 2);

            //edit security check 
            AssertForDeny(sl => sl.AddressService.Edit(addressInfo),
                FirstSchoolContext, SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent 
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);


            addressInfo = AddressInfo.Create(address);
            addressInfo.City = "test city 4";
            addressInfo.StreetNumber = "street4";
            addressInfo.AddressLine1 = "AddressLine1_4";
            addressInfo.AddressLine2 = "AddressLine2_4";
            addressInfo.AddressNumber = "4";
            addressInfo.Country = "Canada";
            addressInfo.CountyId = 3;

            address = addressService.Edit(addressInfo);
            AssertAddressEqual(addressInfo, address);

            //delete security check 
            AssertForDeny(sl => sl.AddressService.Delete(address.Id),
                FirstSchoolContext, SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            addressService.Delete(address.Id);
            addressService.Delete(address2.Id);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AddressService.GetAddress().Count, 0);
            
        }

        private static void AssertAddressEqual(AddressInfo addressInfo, Address address)
        {
            Assert.AreEqual(addressInfo.Id, address.Id);
            Assert.AreEqual(addressInfo.Latitude, address.Latitude);
            Assert.AreEqual(addressInfo.Longitude, address.Longitude);
            Assert.AreEqual(addressInfo.PostalCode, address.PostalCode);
            Assert.AreEqual(addressInfo.State, address.State);
            Assert.AreEqual(addressInfo.StreetNumber, address.StreetNumber);
            Assert.AreEqual(addressInfo.AddressLine1, address.AddressLine1);
            Assert.AreEqual(addressInfo.AddressLine2, address.AddressLine2);
            Assert.AreEqual(addressInfo.AddressNumber, address.AddressNumber);
        }


    }
}
