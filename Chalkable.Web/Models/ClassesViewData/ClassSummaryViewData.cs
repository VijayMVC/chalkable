using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.CalendarsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassSummaryViewData : ClassViewData
    {
        private const int STUDENT_COUNT = 16;

        public RoomViewData Room { get; set; }
        public IList<ShortPersonViewData> Students { get; set; }
        public int ClassSize { get; set; }

        public ClassHoverBoxViewData<ClassAttendanceHoverViewData> ClassAttendanceBox { get; set; }
        //public ClassTitelsViewData<ClassDisciplineHoveViewData> ClassDisciplineListTile { get; set; }
        public ClassHoverBoxViewData<ClassAverageForMpHoverViewData> ClassAverageBox { get; set; }

        public IList<AnnouncementByDateViewData> AnnouncementsByDate { get; set; } 

        protected ClassSummaryViewData(ClassComplex c, Room currentRoom) : base(c)
        {
            if (currentRoom != null)
                Room = RoomViewData.Create(currentRoom);

        }

        public static ClassSummaryViewData Create(ClassComplex c, Room currentRoom, IList<Person> students)
        {
            var res = new ClassSummaryViewData(c, currentRoom) {ClassSize = students.Count};
            students = students.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Take(STUDENT_COUNT).ToList();
            res.Students = ShortPersonViewData.Create(students);
            return res;
        }

        public static ClassSummaryViewData Create(ClassComplex c, Room currentRoom, IList<Person> students,
             IList<AnnouncementByDateViewData> announcementByDates, IList<ClassAttendanceComplex> attendances, int posibleAbsent)
        {
            var res = Create(c, currentRoom, students);
            res.AnnouncementsByDate = announcementByDates;
            res.ClassAttendanceBox = ClassHoverBoxViewData<ClassAttendanceHoverViewData>.Create(attendances, posibleAbsent);
            return res;
        }

    }
}