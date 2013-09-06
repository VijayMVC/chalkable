using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Model
{
    public partial class StiEntities
    {
        public StiEntities(string connectionString)
                : base(connectionString)
            {
            }
    }
}
