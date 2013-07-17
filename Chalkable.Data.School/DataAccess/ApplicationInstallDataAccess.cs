using Chalkable.Data.Common;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Data.School.DataAccess
{
    public class ApplicationInstallDataAccess : DataAccessBase<ApplicationInstall>
    {
        public ApplicationInstallDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}