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

        public void Delete(Address address)
        {
            SimpleDelete(address);
        }

        public IList<Address> GetAddresses()
        {
            var sql = "select * from Address";
            var conds = new Dictionary<string, object>();
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return reader.ReadList<Address>();
            }
        } 

        public Address GetAddressById(Guid id)
        {
            var sql = "select * from Address";
            var conds = new Dictionary<string, object>{{"@id", id}};
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                var res = reader.ReadOrNull<Address>();
                return res;
            }
        }
    }
}
