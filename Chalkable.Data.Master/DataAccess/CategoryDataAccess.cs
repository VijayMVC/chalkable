using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class CategoryDataAccess : DataAccessBase<Category, Guid>
    {
        public CategoryDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
