using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        void Add(IList<Address> addresses);
        void Edit(IList<Address> addresses);
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

        public void Add(IList<Address> addresses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AddressDataAccess(u).Insert(addresses));
        }

        public void Edit(IList<Address> addresses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new AddressDataAccess(u).Update(addresses));
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
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AddressDataAccess(u).Delete(ids));
        }
    }
}
