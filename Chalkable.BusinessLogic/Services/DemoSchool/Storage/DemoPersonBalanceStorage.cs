using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPersonBalance
    {
        public decimal Balance { get; set; }
        public int PersonId { get; set; }
    }
    public class DemoPersonBalanceStorage : BaseDemoIntStorage<DemoPersonBalance>
    {
        public DemoPersonBalanceStorage(DemoStorage storage)
            : base(storage, x => x.PersonId, false)
        {
        }

        public new void Add(DemoPersonBalance balance)
        {
            data.Add(balance.PersonId, balance);
        }

        public void UpdatePersonBalance(int personId, decimal balance)
        {
            if (data.ContainsKey(personId))
            {
                data[personId].Balance = balance;
            }
        }

    }
}
