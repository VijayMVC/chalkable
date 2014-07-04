using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
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
        public ClassHoverBoxViewData<ClassDisciplineHoveViewData> ClassDisciplineBox { get; set; }
        public ClassHoverBoxViewData<ClassAverageForMpHoverViewData> ClassAverageBox { get; set; }

        public IList<AnnouncementByDateViewData> AnnouncementsByDate { get; set; } 

        protected ClassSummaryViewData(ClassDetails c, Room currentRoom) : base(c)
        {
            if (currentRoom != null)
                Room = RoomViewData.Create(currentRoom);

        }

        public static ClassSummaryViewData Create(ClassDetails c, Room currentRoom, IList<Person> students)
        {
            var res = new ClassSummaryViewData(c, currentRoom) {ClassSize = students.Count};
            students = students.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Take(STUDENT_COUNT).ToList();
            res.Students = ShortPersonViewData.Create(students);
            return res;
        }

        public static ClassSummaryViewData Create(ClassDetails c, Room currentRoom, IList<Person> students,
             IList<AnnouncementByDateViewData> announcementByDates, IList<ClassAttendanceDetails> attendances
            , int posibleAbsent, IList<ClassDisciplineDetails> disciplines, IList<Infraction> disciplineTypes
            , IList<MarkingPeriodClassGradeAvg> classGradeStatsPerMp)
        {
            var res = Create(c, currentRoom, students);
            res.AnnouncementsByDate = announcementByDates;
            res.ClassAttendanceBox = ClassHoverBoxViewData<ClassAttendanceHoverViewData>.Create(attendances, posibleAbsent);
            res.ClassDisciplineBox = ClassHoverBoxViewData<ClassDisciplineHoveViewData>.Create(disciplineTypes, disciplines);
            res.ClassAverageBox = ClassHoverBoxViewData<ClassAverageForMpHoverViewData>.Create(classGradeStatsPerMp);
            return res;
        }

    }
}