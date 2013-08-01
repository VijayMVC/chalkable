using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentSummaryViewData : PersonSummaryViewData
    {

        public int GradeLevelNumber { get; set; }
        public string CurrentClassName { get; set; }
        public string RoomNumber { get; set; }
        public int? CurrentAttendanceType { get; set; }
        public int MaxPeriodNumber { get; set; }
        public IList<AnnouncementsClassPeriodViewData> PeriodSection { get; set; }
       
        protected StudentSummaryViewData(Person person, Room room) : base(person, room)
        {
            RoomNumber = room.RoomNumber;
        }

        public static StudentSummaryViewData Create(Person person, Room room, IList<AnnouncementsClassPeriodViewData> announcementsClassPeriods)
        {
            var res = new StudentSummaryViewData(person, room) {PeriodSection = announcementsClassPeriods};
            
            return res;
        }


    }

    public class StudentHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        //public static StudentHoverBoxViewData<StudentAttendanceSummaryViewData> Create()
    }

    public class StudentAttendanceSummaryViewData
    {
        public int Type { get; set; }
        public int AttendanceCount { get; set; }

        public static IList<StudentAttendanceSummaryViewData> Create(IDictionary<int, int> attTypeDic)
        {
            return attTypeDic.Select(x => new StudentAttendanceSummaryViewData()
                {
                    AttendanceCount = x.Value,
                    Type = x.Key
                }).ToList();
        }
    }
}