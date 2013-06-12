using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressSerivce
    {
        Address Add(string personId, string value, string note, AddressType type);
        Address Edit(string id, string value, string note, AddressType type);
        void Delete(string id);
        IList<Address> GetAddress();
    }
    public class AddressService : SchoolServiceBase, IAddressSerivce
    {
        public AddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        //TODO: security
        public Address Add(string personId, string value, string note, AddressType type)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Note = note,
                        PersonRef = Guid.Parse(personId),
                        Type = (int) type,
                        Value = value
                    };
                da.Create(address);
                uow.Commit();
                return address;
            }
        }

        public Address Edit(string id, string value, string note, AddressType type)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetAddressById(Guid.Parse(id));
                address.Value = value;
                address.Note = note;
                address.Type = (int) type;
                uow.Commit();
                return address;
            }
        }

        public void Delete(string id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                da.Delete(Guid.Parse(id));
                uow.Commit();
            }
        }
        
        public IList<Address> GetAddress()
        {
            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAddresses();
            }
        }
    }
}
