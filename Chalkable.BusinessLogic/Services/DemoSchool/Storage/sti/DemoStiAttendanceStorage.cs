using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiAttendanceStorage:BaseDemoIntStorage<SectionAttendance>
    {
        public DemoStiAttendanceStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int classId)
        {
            var ds = date.ToString("yyyy-MM-dd");
            if (data.Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                GenerateSectionAttendanceForClass(classId, date, date);
            }
            return data.First(x => x.Value.SectionId == classId && x.Value.Date == ds).Value;
        }

        public void SetSectionAttendance(DateTime date, int classId, SectionAttendance sa)
        {
            var ds = date.ToString("yyyy-MM-dd");
            if (data.Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                data.Add(GetNextFreeId(), sa);
            }
            var item = data.First(x => x.Value.SectionId == classId && x.Value.Date == ds).Key;
            sa.IsPosted = true;
            data[item] = sa;
        }


        public void GenerateSectionAttendanceForClass(int classId, DateTime startDate, DateTime endDate)
        {
            var classRoomLevels = new string[] {"Absent", "Missing", "Tardy", "Present"};
            var random = new Random();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                var ds = start.ToString("yyyy-MM-dd");
                var sa = new SectionAttendance()
                {
                    SectionId = classId,
                    Date = ds,
                    StudentAttendance = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery()
                    {
                        ClassId = classId
                    }).Select(x => x.PersonRef).Distinct().Select(x => new StudentSectionAttendance()
                    {
                        Date = ds,
                        SectionId = classId,
                        StudentId = x,
                        ClassroomLevel = classRoomLevels[random.Next(0, 4)]
                    }).ToList()
                };
                data.Add(GetNextFreeId(), sa);
            }
        }


        private SectionAttendanceSummary GetSaSummary(int classId, DateTime startDate, DateTime endDate)
        {
            var sectionAttendanceSummary = new SectionAttendanceSummary();
            sectionAttendanceSummary.SectionId = classId;

            var days = new List<DailySectionAttendanceSummary>();

            var absenceCount = new Dictionary<int, int>();
            var tardiesCount = new Dictionary<int, int>();

            var studentIds = Storage.PersonStorage.GetPersons(new PersonQuery()
            {
                TeacherId = Storage.Context.UserLocalId
            }).Persons.Select(x => x.Id);

            foreach (var studentId in studentIds)
            {
                absenceCount.Add(studentId, 0);
                tardiesCount.Add(studentId, 0);
            }

            for (var start = startDate; start < endDate; start = start.AddDays(1))
            {
                var day = new DailySectionAttendanceSummary();
                var attendance = Storage.StiAttendanceStorage.GetSectionAttendance(start, classId);

                day.Absences = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Absent");
                day.Tardies = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Tardy");
                day.Date = start;
                day.SectionId = classId;
                days.Add(day);

                foreach (var studentId in studentIds)
                {
                    absenceCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Absent" && x.StudentId == studentId);
                    tardiesCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Tardy" && x.StudentId == studentId);
                }
            }
            sectionAttendanceSummary.Days = days;
            sectionAttendanceSummary.Students = studentIds.Select(x => new StudentSectionAttendanceSummary
            {
                SectionId = classId,
                StudentId = x,
                Absences = absenceCount[x],
                Tardies = tardiesCount[x]
            });

            return sectionAttendanceSummary;
        }

        public IList<SectionAttendanceSummary> GetSectionAttendanceSummary(List<int> classesIds, DateTime startDate, DateTime endDate)
        {
            return classesIds.Select(classId => GetSaSummary(classId, startDate, endDate)).ToList();
        }
    }
}
