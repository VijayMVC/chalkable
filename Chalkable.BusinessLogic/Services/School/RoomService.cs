﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IRoomService
    {
        Room AddRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber);
        void AddRooms(IList<Room> rooms);
        Room EditRoom(int id, string roomNumber, string description, string size, int? capacity, string phoneNumber);
        void EditRooms(IList<Room> rooms); 
        void DeleteRooms(IList<int> ids);
        PaginatedList<Room> GetRooms(int start = 0, int count = int.MaxValue);
        Room WhereIsPerson(int personId, DateTime dateTime);
        Room GetRoomById(int id);
    }

    //TODO: needs tests 

    public class RoomService : SchoolServiceBase, IRoomService
    {
        public RoomService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Room AddRoom(int id, int schoolId, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
                var room = new Room
                    {
                        Id = id,
                        SchoolRef = schoolId,
                        Capacity = capacity,
                        Description = description,
                        PhoneNumber = phoneNumber,
                        RoomNumber = roomNumber,
                        Size = size, 
                    };
                da.Insert(room);
                uow.Commit();
                return room;
            }
        }

        public void AddRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
                da.Insert(rooms);
                uow.Commit();
            }
        }

        public Room EditRoom(int id, string roomNumber, string description, string size, int? capacity, string phoneNumber)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
                var room = da.GetById(id);
                room.RoomNumber = roomNumber;
                room.Description = description;
                room.Capacity = capacity;
                room.Size = size;
                room.PhoneNumber = phoneNumber;
                da.Update(room);
                uow.Commit();
                return room;
            }
        }

        public void EditRooms(IList<Room> rooms)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new RoomDataAccess(uow, Context.SchoolLocalId);
                da.Update(rooms);
                uow.Commit();
            }
        }

        public PaginatedList<Room> GetRooms(int start = 0, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow, Context.SchoolLocalId).GetPage(start, count);
            }
        }

        public Room WhereIsPerson(int personId, DateTime dateTime)
        {
            var c = ServiceLocator.ClassPeriodService.CurrentClassForTeacher(personId, dateTime);
            return c.RoomRef.HasValue ? GetRoomById(c.RoomRef.Value) : null;
        }

        public Room GetRoomById(int id)
        {
            using (var uow = Read())
            {
                return new RoomDataAccess(uow, Context.SchoolLocalId).GetById(id);
            }
        }

        public void DeleteRooms(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new RoomDataAccess(u, Context.SchoolLocalId).Delete(ids));
        }


        
    }
}
