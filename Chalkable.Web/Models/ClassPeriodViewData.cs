using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models
{
    public class ClassPeriodViewData
    {
        public Guid Id { get; set; }
        public PeriodViewData Period { get; set; }
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; }
        public ClassViewData Class { get; set; }
        public int StudentsCount { get; set; }

        public static ClassPeriodViewData Create(ClassPeriod classPeriod, ClassComplex classComplex, Room room)
        {
            return new ClassPeriodViewData
                {
                    Id = classPeriod.Id,
                    Period = PeriodViewData.Create(classPeriod.Period),
                    Class = ClassViewData.Create(classComplex),
                    StudentsCount = classComplex.StudentsCount,
                    RoomId = classPeriod.RoomRef,
                    RoomNumber = room.RoomNumber
                };
        }
    }
}