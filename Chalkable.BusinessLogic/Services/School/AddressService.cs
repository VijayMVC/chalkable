using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressSerivce
    {
        Address Add(Guid personId, string value, string note, AddressType type);
        Address Edit(Guid id, string value, string note, AddressType type);
        void Delete(Guid id);
        IList<Address> GetAddress();

    }
    public class AddressService : SchoolServiceBase, IAddressSerivce
    {
        public AddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        //TODO: security
        public Address Add(Guid personId, string value, string note, AddressType type)
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
                        PersonRef = personId,
                        Type = type,
                        Value = value
                    };
                da.Create(address);
                uow.Commit();
                return address;
            }
        }

        public Address Edit(Guid id, string value, string note, AddressType type)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetAddressById(id);
                address.Value = value;
                address.Note = note;
                address.Type = type;
                da.Update(address);
                uow.Commit();
                return address;
            }
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
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
            if(!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAddresses();
            }
        }
    }
}
