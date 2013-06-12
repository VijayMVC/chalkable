using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AddressDataAccess : DataAccessBase
    {
        public AddressDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Address address)
        {
            SimpleInsert(address);
        }

        public void Update(Address address)
        {
            SimpleUpdate(address);
        }
        
        public void Delete(Guid id)
        {
            SimpleDelete<Address>(id);
        } 

        public IList<Address> GetAddresses()
        {
            return SelectMany<Address>();
        } 
        
        public Address GetAddressById(Guid id)
        {
            var conds = new Dictionary<string, object>{{"@id", id}};
            return SelectOne<Address>(conds);
        }
    }
}
