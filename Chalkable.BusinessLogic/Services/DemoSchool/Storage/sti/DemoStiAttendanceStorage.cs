using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiAttendanceStorage:BaseDemoStorage<int, SectionAttendance>
    {
        public DemoStiAttendanceStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int classId)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public void SetSectionAttendance(int id, DateTime date, int classId, SectionAttendance sa)
        {
            throw new NotImplementedException();
        }
    }
}
