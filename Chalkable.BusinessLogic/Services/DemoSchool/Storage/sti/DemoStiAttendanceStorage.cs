using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
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
                return new SectionAttendance()
                {
                    SectionId = classId,
                    Date = ds,
                    StudentAttendance = new List<StudentSectionAttendance>()
                    {
                        new StudentSectionAttendance()
                        {
                            Date = ds,
                            SectionId = classId,
                            StudentId = DemoSchoolConstants.FirstStudentId
                        },

                        new StudentSectionAttendance()
                        {
                            Date = ds,
                            SectionId = classId,
                            StudentId = DemoSchoolConstants.SecondStudentId
                        },

                        new StudentSectionAttendance()
                        {
                            Date = ds,
                            SectionId = classId,
                            StudentId = DemoSchoolConstants.ThirdStudentId
                        }
                    }
                };
            }
           return data.First(x => x.Value.SectionId == classId && x.Value.Date == ds).Value;
        }

        public override void Setup()
        {
           
        }

        public void SetSectionAttendance(int schoolYearId, DateTime date, int classId, SectionAttendance sa)
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


        public IList<SectionAttendanceSummary> GetSectionAttendanceSummary(List<int> classesIds, DateTime startDate, DateTime endDate)
        {
            var result = new List<SectionAttendanceSummary>();
            foreach (var classId in classesIds)
            {
                var sectionAttendanceSummary = new SectionAttendanceSummary();
                sectionAttendanceSummary.SectionId = classId;

                var days = new List<DailySectionAttendanceSummary>();

                for (var start = startDate; start < endDate; start = start.AddDays(1))
                {
                    var day = new DailySectionAttendanceSummary();
                    var attendance = Storage.StiAttendanceStorage.GetSectionAttendance(start, classId);

                    day.Absences = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Absent");
                    day.Tardies = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Tardy");
                    day.Date = start;
                    day.SectionId = classId;
                    days.Add(day);
                }



                //todo: add students stats
                var students = Storage.PersonStorage.GetPersons(new PersonQuery()
                {
                    TeacherId = Storage.Context.UserLocalId
                });
                sectionAttendanceSummary.Days = days;
                sectionAttendanceSummary.Students = students.Persons.Select(x => new StudentSectionAttendanceSummary
                {
                    SectionId = classId,
                    StudentId = x.Id,
                    Absences = 3,
                    Tardies = 0
                });
                result.Add(sectionAttendanceSummary);
            }
            return result;
        }
    }
}
