using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressSerivce
    {
        Address Add(string personId, string value, string note, AddressType type);
        Address Edit(string id, string value, string note, AddressType type);
        void Delete(string id);
    }
    public class AddressService : SchoolServiceBase, IAddressSerivce
    {
        public AddressService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: security
        public Address Add(string personId, string value, string note, AddressType type)
        {
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
            using (var uow = Update())
            {
                var da = new AddressDataAccess(uow);
                var address = da.GetAddressById(Guid.Parse(id));
                da.Delete(address);
                uow.Commit();
            }
        }
    }
}
