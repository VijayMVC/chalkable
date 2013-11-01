using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        Address Add(AddressInfo address);
        Address Edit(AddressInfo address);
        void Delete(int id);
        IList<Address> GetAddress();
        IList<Address> GetAddress(int personId);
    }
    public class AddressService : SchoolServiceBase, IAddressService
    {
        public AddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public Address Add(AddressInfo addressInfo)
        {
            if(!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = EditAddress(addressInfo, null);
                da.Insert(address);
                uow.Commit();
                return address;
            }
        }

        public Address Edit(AddressInfo addressInfo)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetById(addressInfo.Id);
                if (!AddressSecurity.CanModify(address, Context))
                    throw new ChalkableSecurityException();
                address = EditAddress(addressInfo, address);
                da.Update(address);
                uow.Commit();
                return address;
            }
        }

        private Address EditAddress(AddressInfo addressInfo, Address address)
        {
            if(address == null)
                address = new Address();
            address.Id = addressInfo.Id;
            address.AddressLine1 = addressInfo.AddressLine1;
            address.AddressLine2 = addressInfo.AddressLine2;
            address.AddressNumber = addressInfo.AddressNumber;
            address.StreetNumber = addressInfo.StreetNumber;
            address.City = addressInfo.City;
            address.State = addressInfo.State;
            address.Country = addressInfo.Country;
            address.CountyId = addressInfo.CountyId;
            address.Latitude = addressInfo.Latitude;
            address.Longitude = addressInfo.Longitude;
            address.PostalCode = addressInfo.PostalCode;
            return address;
        }

        public void Delete(int id)
        {
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetById(id);
                if (!AddressSecurity.CanModify(address, Context))
                    throw new ChalkableSecurityException();
                da.Delete(id);
                uow.Commit();
            }
        }
        
        public IList<Address> GetAddress()
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAll();
            }
        }

        public IList<Address> GetAddress(int personId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAddress(personId);
            }
        }
    }
}
