using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            StudentId = 1196
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
    }
}
