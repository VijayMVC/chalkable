using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    
    public class DemoRoomService : DemoSchoolServiceBase, IRoomService
    {
        public DemoRoomService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void AddRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.AddRooms(rooms);
        }

        public void EditRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.Update(rooms);
        }

        public void DeleteRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.RoomStorage.Delete(rooms);
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            return Storage.RoomStorage.GetAll(start, count);
            
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var c = ServiceLocator.ClassPeriodService.CurrentClassForTeacher(personId, dateTime);
            return c.RoomRef.HasValue ? GetRoomById(c.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            return Storage.RoomStorage.GetById(id);
        }
    }
}
