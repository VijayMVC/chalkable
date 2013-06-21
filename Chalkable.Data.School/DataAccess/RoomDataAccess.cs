using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class RoomDataAccess : DataAccessBase
    {
        public RoomDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Create(Room room)
        {
            SimpleInsert(room);
        }
        public void Update(Room room)
        {
            SimpleUpdate(room);
        }
        public void Delete(Room room)
        {
            SimpleDelete(room);
        }

        public Room GetById(Guid id)
        {
            return SelectOne<Room>(new Dictionary<string, object> {{"Id", id}});
        }

        public PaginatedList<Room> GetRooms(int start, int count)
        {
            return PaginatedSelect<Room>("Id", start, count);
        } 
    }
}
