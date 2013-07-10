using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class FinalGradeAnnouncementTypeDataAccess : DataAccessBase<FinalGradeAnnouncementType>
    {
        public FinalGradeAnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
