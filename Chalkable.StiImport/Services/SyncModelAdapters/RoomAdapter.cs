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

        protected override void InsertInternal(IList<Room> entities)
        {
            var rooms = entities.ToList()
                .Select(x => new Data.School.Model.Room
                {
                    Id = x.RoomID,
                    SchoolRef = x.SchoolID,
                    Capacity = x.StudentCapacity,
                    Description = x.Description,
                    PhoneNumber = null,
                    RoomNumber = x.RoomNumber ?? UNKNOWN_ROOM_NUMBER,
                    Size = null,
                }).ToList();
            SchoolLocator.RoomService.AddRooms(rooms);
        }

        protected override void UpdateInternal(IList<Room> entities)
        {
            var rooms = entities.Select(x => new Data.School.Model.Room
            {
                Capacity = x.StudentCapacity,
                Description = x.Description,
                Id = x.RoomID,
                RoomNumber = x.RoomNumber,
                SchoolRef = x.SchoolID
            }).ToList();
            SchoolLocator.RoomService.EditRooms(rooms);
        }

        protected override void DeleteInternal(IList<Room> entities)
        {
            var ids = entities.Select(x => new Data.School.Model.Room { Id = x.RoomID }).ToList();
            SchoolLocator.RoomService.DeleteRooms(ids);
        }
    }
}