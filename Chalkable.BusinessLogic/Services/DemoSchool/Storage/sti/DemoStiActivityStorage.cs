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

        public void CopyActivity(int sisActivityId, IList<int> classIds)
        {
            var activity = GetById(sisActivityId);

            var classIdsFiltered = classIds.Where(x => x != activity.SectionId).ToList();

            foreach (var classId in classIdsFiltered)
            {
                activity.Id = GetNextFreeId();
                activity.SectionId = classId;

                if (activity.Attachments == null)
                    activity.Attachments = new List<ActivityAttachment>();

                foreach (var attachment in activity.Attachments)
                {
                    attachment.ActivityId = activity.Id;
                }
                data.Add(activity.Id, activity);
            }
        }
    }
}
