using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IRoomService
    {
        void AddRooms(IList<Room> rooms);
        void EditRooms(IList<Room> rooms);
        void DeleteRooms(IList<Room> rooms);

        void AddHomerooms(IList<Homeroom> homerooms);
        void EditHomerooms(IList<Homeroom> homerooms);
        void DeleteHomerooms(IList<Homeroom> homerooms);

        Room WhereIsPerson(int personId, DateTime dateTime);
        Room GetRoomById(int id);
        Homeroom GetStudentHomeroomOrNull(int studentId, int schoolYearId);
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

        public Homeroom GetStudentHomeroomOrNull(int studentId, int schoolYearId)
        {
            using (var uow = Read())
            {
                var studentSchoolYear = new SchoolYearDataAccess(uow).GetStudentSchoolYear(studentId, schoolYearId);
                if (studentSchoolYear?.HomeroomRef == null)
                    return null;

                var homeroom = new DataAccessBase<Homeroom, int>(uow).GetById(studentSchoolYear.HomeroomRef.Value);
                if (homeroom?.TeacherRef == null)
                    return homeroom;

                homeroom.Teacher = new PersonDataAccess(uow).GetById(homeroom.TeacherRef.Value);

                return homeroom;
            }
        }

        public void DeleteRooms(IList<Room> rooms)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Room>(u).Delete(rooms));
        }

        public void AddHomerooms(IList<Homeroom> homerooms)
        {
            DoUpdate(u => new DataAccessBase<Homeroom>(u).Insert(homerooms));
        }

        public void EditHomerooms(IList<Homeroom> homerooms)
        {
            DoUpdate(u => new DataAccessBase<Homeroom>(u).Update(homerooms));
        }

        public void DeleteHomerooms(IList<Homeroom> homerooms)
        {
            DoUpdate(u => new DataAccessBase<Homeroom>(u).Delete(homerooms));
        }
    }
}
