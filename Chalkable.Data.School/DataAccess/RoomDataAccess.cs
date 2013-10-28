using System;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class RoomDataAccess : DataAccessBase<Room, int>
    {
        public RoomDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
