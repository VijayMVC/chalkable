using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        void Add(IList<Address> addressInfos);
        Address Add(Address addressInfo);
        Address Edit(Address address);
        IList<Address> Edit(IList<Address> addresses);
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

        public void Add(IList<Address> addressInfos)
        {
            if(!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                da.Insert(addressInfos);
                uow.Commit();
            }
        }

        public Address Add(Address addressInfo)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                da.Insert(addressInfo);
                uow.Commit();
            }
            return addressInfo;
        }

        public Address Edit(Address addressInfo)
        {
            return Edit(new List<Address> {addressInfo}).First();
        }

        public IList<Address> Edit(IList<Address> addresses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var res = addresses;
                da.Update(res);
                uow.Commit();
                return res;
            }
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
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
