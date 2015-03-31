using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAutoGradeStorage:BaseDemoIntStorage<AutoGrade>
    {
        public DemoAutoGradeStorage()
            : base(null, true)
        {
        }


        public void SetAutoGrade(AutoGrade autograde)
        {
           var item = data.First(
                x => x.Value.AnnouncementApplicationRef == autograde.AnnouncementApplicationRef && autograde.StudentRef == x.Value.StudentRef);

            data[item.Key] = autograde;
        }

        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            var annApps = StorageLocator.AnnouncementApplicationStorage.GetAnnouncementApplicationsByAnnId(announcementId);
            return GetAll()
                       .Where(x => annApps.Any(y => y.Id == x.AnnouncementApplicationRef))
                       .ToList();
        }

        public AutoGrade GetAutoGrade(int announcementApplicationId, int? recipientId = null)
        {
            return GetAll().FirstOrDefault(x => x.AnnouncementApplicationRef == announcementApplicationId
                                                        && (!recipientId.HasValue || x.StudentRef == recipientId));
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            return GetAll().Where(x => x.AnnouncementApplicationRef == announcementApplicationId).ToList();
        }
    }
}
