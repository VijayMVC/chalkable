using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IRoomService
    {
        void AddRooms(IList<Room> rooms);
        void EditRooms(IList<Room> rooms);
        void DeleteRooms(IList<Room> rooms);
        Room WhereIsPerson(int personId, DateTime dateTime);
        Room GetRoomById(int id);
    }

    //TODO: needs tests 

    public class RoomService : SchoolServiceBase, IRoomService
    {
        public RoomService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public void AddRooms(IList<Room> rooms)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<Room>(u).Insert(rooms));
        }

        public void EditRooms(IList<Room> rooms)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Room>(u).Update(rooms));
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var c = ServiceLocator.ClassPeriodService.CurrentClassForTeacher(personId, dateTime);
            return (c != null && c.RoomRef.HasValue) ? GetRoomById(c.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            return DoRead(u => new DataAccessBase<Room, int>(u).GetById(id));
        }

        public void DeleteRooms(IList<Room> rooms)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Room>(u).Delete(rooms));
        }


        
    }
}
