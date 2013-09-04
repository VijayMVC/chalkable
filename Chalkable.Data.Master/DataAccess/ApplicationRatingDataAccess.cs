using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationRatingDataAccess : DataAccessBase<ApplicationRating>
    {
        public ApplicationRatingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}