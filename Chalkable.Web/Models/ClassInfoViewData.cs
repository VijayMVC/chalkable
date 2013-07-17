﻿using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ClassInfoViewData : ClassViewData
    {
        public RoomViewData Room { get; set; }
        public ChalkableDepartmentViewData Department { get; set; }

        protected ClassInfoViewData(ClassComplex cClass, Room room, ChalkableDepartment department)
            : base(cClass)
        {
            if (room != null) 
                Room = RoomViewData.Create(room);
            if (department != null) 
                Department = ChalkableDepartmentViewData.Create(department);
        }
        public static ClassInfoViewData Create(ClassComplex cClass, Room roomInfo, ChalkableDepartment department)
        {
            return new ClassInfoViewData(cClass, roomInfo, department);
        }
    }
}