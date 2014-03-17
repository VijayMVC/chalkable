using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        void Add(IList<AddressInfo> addressInfos);
        Address Add(AddressInfo addressInfo);
        Address Edit(AddressInfo address);
        IList<Address> Edit(IList<AddressInfo> addresses);
        void Delete(int id);
        void Delete(IList<int> ids);
        IList<Address> GetAddress();
        IList<Address> GetAddress(int personId);
    }
    public class AddressService : SchoolServiceBase, IAddressService
    {
        public AddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Add(IList<AddressInfo> addressInfos)
        {
            if(!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var addresses = addressInfos.Select(EditAddress).ToList();
                da.Insert(addresses);
                uow.Commit();
            }
        }

        public Address Add(AddressInfo addressInfo)
        {
            var a = EditAddress(addressInfo);
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                da.Insert(a);
                uow.Commit();
            }
            return a;
        }

        public Address Edit(AddressInfo addressInfo)
        {
            return Edit(new List<AddressInfo> {addressInfo}).First();
        }
        public IList<Address> Edit(IList<AddressInfo> addresses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var res = addresses.Select(EditAddress).ToList();
                da.Update(res);
                uow.Commit();
                return res;
            }
        }

        private static Address EditAddress(AddressInfo addressInfo)
        {
            return new Address
                {
                    Id = addressInfo.Id,
                    AddressLine1 = addressInfo.AddressLine1,
                    AddressLine2 = addressInfo.AddressLine2,
                    AddressNumber = addressInfo.AddressNumber,
                    StreetNumber = addressInfo.StreetNumber,
                    City = addressInfo.City,
                    State = addressInfo.State,
                    Country = addressInfo.Country,
                    CountyId = addressInfo.CountyId,
                    Latitude = addressInfo.Latitude,
                    Longitude = addressInfo.Longitude,
                    PostalCode = addressInfo.PostalCode
                };
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


        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AddressDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }


       
    }
}
