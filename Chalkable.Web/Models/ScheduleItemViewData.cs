﻿using System;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ScheduleItemViewData : PeriodViewData
    {
        public DateTime Day { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public int? ClassId { get; set; }
        public int? RoomId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string ClassNumber { get; set; }
        public string RoomNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool? CanCreateItem { get; set; }

        protected ScheduleItemViewData(ScheduleItem scheduleItem)
            : base(scheduleItem.PeriodId, scheduleItem.SchoolYearId, scheduleItem.PeriodOrder)
        {
            Day = scheduleItem.Day;
            StartTime = scheduleItem.StartTime;
            EndTime = scheduleItem.EndTime;
            ClassId = scheduleItem.ClassId;
            ClassDescription = scheduleItem.ClassDescription;
            ClassName = scheduleItem.ClassName;
            ClassNumber = scheduleItem.ClassNumber;
            RoomId = scheduleItem.RoomId;
            RoomNumber = scheduleItem.RoomNumber;
            DepartmentId = scheduleItem.ChalkableDepartmentId;
            CanCreateItem = scheduleItem.CanCreateItem;
        }

        public static ScheduleItemViewData Create(ScheduleItem scheduleItem)
        {
            return new ScheduleItemViewData(scheduleItem);
        }
    }
}