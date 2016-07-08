using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoRoomStorage : BaseDemoIntStorage<Room>
    {
        public DemoRoomStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoRoomService : DemoSchoolServiceBase, IRoomService
    {
        private DemoRoomStorage RoomStorage { get; set; }
        public DemoRoomService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            RoomStorage = new DemoRoomStorage();
        }

        public void AddRooms(IList<Room> rooms)
        {
            RoomStorage.Add(rooms);
        }

        public void EditRooms(IList<Room> rooms)
        {
            RoomStorage.Update(rooms);
        }

        public void DeleteRooms(IList<Room> rooms)
        {
            throw new NotImplementedException();
        }

        public void AddHomerooms(IList<Homeroom> homerooms)
        {
            throw new NotImplementedException();
        }

        public void EditHomerooms(IList<Homeroom> homerooms)
        {
            throw new NotImplementedException();
        }

        public void DeleteHomerooms(IList<Homeroom> homerooms)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            return GetAll(start, count);
        }

        private PaginatedList<Room> GetAll(int start, int count)
        {
            var rooms = RoomStorage.GetData().Select(x => x.Value).ToList().Skip(start).Take(count);
            return new PaginatedList<Room>(rooms, start / count, count, RoomStorage.GetData().Count);
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var c = ServiceLocator.ClassPeriodService.CurrentClassForTeacher(personId, dateTime);
            return c.RoomRef.HasValue ? RoomStorage.GetById(c.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            return RoomStorage.GetById(id);
        }

        public Homeroom GetStudentHomeroomOrNull(int studentId, int schoolYearId)
        {
            throw new NotImplementedException();
        }
    }
}
