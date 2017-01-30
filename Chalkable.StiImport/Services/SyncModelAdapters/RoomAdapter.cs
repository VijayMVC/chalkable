using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class RoomAdapter : SyncModelAdapter<Room>
    {
        private const string UNKNOWN_ROOM_NUMBER = "Unknown number";
        public RoomAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Room Selector(Room x)
        {
            return new Data.School.Model.Room
            {
                Id = x.RoomID,
                SchoolRef = x.SchoolID,
                Capacity = x.StudentCapacity,
                Description = x.Description,
                RoomNumber = x.RoomNumber
            };
        }

        protected override void InsertInternal(IList<Room> entities)
        {
            var rooms = entities.ToList()
                .Select(Selector).ToList();
            ServiceLocatorSchool.RoomService.AddRooms(rooms);
        }

        protected override void UpdateInternal(IList<Room> entities)
        {
            var rooms = entities.Select(Selector).ToList();
            ServiceLocatorSchool.RoomService.EditRooms(rooms);
        }

        protected override void DeleteInternal(IList<Room> entities)
        {
            var ids = entities.Select(x => new Data.School.Model.Room { Id = x.RoomID }).ToList();
            ServiceLocatorSchool.RoomService.DeleteRooms(ids);
        }

        protected override void PrepareToDeleteInternal(IList<Room> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}