using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class RoomDataAccess : BaseSchoolDataAccess<Room>
    {
        public RoomDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete<Room>(ids.Select(x=>new Room{Id = x}).ToList());
        }
    }
}
