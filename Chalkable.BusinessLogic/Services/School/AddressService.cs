using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAddressService
    {
        void Add(IList<Address> addresses);
        void Edit(IList<Address> addresses);
        void Delete(IList<Address> addresses);
        IList<Address> GetAddress();
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
            DoUpdate(u => new DataAccessBase<Address>(u).Insert(addresses));
        }

        public void Edit(IList<Address> addresses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Address>(u).Update(addresses));
        }

        public IList<Address> GetAddress()
        {
            if (!BaseSecurity.IsDistrictOrTeacher(Context))
                throw new ChalkableSecurityException();
            return DoRead(u => new DataAccessBase<Address>(u).GetAll());
        }

        public void Delete(IList<Address> addresses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Address>(u).Delete(addresses));
        }
    }
}
