using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiActivityStorage:BaseDemoStorage<int, Activity>
    {
        public DemoStiActivityStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public Activity CreateActivity(int classId, Activity activity)
        {
            activity.SectionId = classId;
            activity.Id = GetNextFreeId();
            data.Add(activity.Id, activity);
            return activity;
        }
    }
}
