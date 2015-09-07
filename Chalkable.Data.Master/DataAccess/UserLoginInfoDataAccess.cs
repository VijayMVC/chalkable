using System;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class UserLoginInfoDataAccess : DataAccessBase<UserLoginInfo, Guid>
    {
        public UserLoginInfoDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}