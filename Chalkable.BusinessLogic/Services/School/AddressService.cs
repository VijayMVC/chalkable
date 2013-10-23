﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        Address Add(Guid personId, string value, string note, AddressType type);
        Address Edit(Guid id, string value, string note, AddressType type);
        void Delete(Guid id);
        IList<Address> GetAddress();
        IList<Address> GetAddress(Guid personId);
    }
    public class AddressService : SchoolServiceBase, IAddressService
    {
        public AddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public Address Add(Guid personId, string value, string note, AddressType type)
        {
            if(!BaseSecurity.IsAdminOrTeacher(Context))//TODO:can teacher do this?
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
                da.Insert(address);
                uow.Commit();
                return address;
            }
        }

        public Address Edit(Guid id, string value, string note, AddressType type)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetById(id);
                if (!AddressSecurity.CanModify(address, Context))
                    throw new ChalkableSecurityException();
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
            if(!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAll();
            }
        }

        public IList<Address> GetAddress(Guid personId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            using (var uow = Read())
            {
                var da = new AddressDataAccess(uow);
                return da.GetAll(new AndQueryCondition{{Address.PERSON_REF_FIELD, personId}});
            }
        }
    }
}
