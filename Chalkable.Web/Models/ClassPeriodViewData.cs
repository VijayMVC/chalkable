using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models
{
    public class ClassPeriodShortViewData
    {
        public int Id { get; set; }
        public PeriodViewData Period { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int ClassId { get; set; }
        public int DateTypeId { get; set; }

        protected ClassPeriodShortViewData(ClassPeriod classPeriod, Room room)
        {
            Id = classPeriod.Id;
            Period = PeriodViewData.Create(classPeriod.Period);
            RoomId = classPeriod.RoomRef;
            ClassId = classPeriod.ClassRef;
            DateTypeId = classPeriod.DateTypeRef;
            if (room != null)
                RoomNumber = room.RoomNumber;
        }
        public static ClassPeriodShortViewData Create(ClassPeriod classPeriod, Room room)
        {
            return new ClassPeriodShortViewData(classPeriod, room);
        }
    }

    public class ClassPeriodViewData : ClassPeriodShortViewData
    {
        public ClassViewData Class { get; set; }
        public int StudentsCount { get; set; }

        protected ClassPeriodViewData(ClassPeriod classPeriod, Room room, ClassDetails classComplex)
            : base(classPeriod, room)
        {
            if (classComplex != null)
            {
                Class = ClassViewData.Create(classComplex);
                StudentsCount = classComplex.StudentsCount;
            }
        }
        public static ClassPeriodViewData Create(ClassPeriod classPeriod, ClassDetails classComplex, Room room)
        {
            return new ClassPeriodViewData(classPeriod, room, classComplex);
        }
    }
}