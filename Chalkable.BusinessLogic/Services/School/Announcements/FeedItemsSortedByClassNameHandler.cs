using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public class FeedItemsSortedByClassNameHandler : BaseFeedItemHandler<string, string>
    {
        public FeedItemsSortedByClassNameHandler(bool sortDesc) : base(sortDesc)
        {
        }

        protected override IList<AnnouncementComplex> InternalGetLessonPlans(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, string @from, string to, bool includeFrom, bool includeTo, bool? ownedOnly = null)
        {
            return locator.LessonPlanService.GetLessonPlansSortedByClassName(fromDate, toDate, @from, to, includeFrom, includeTo, classId, complete, start, count, _sortDesc, ownedOnly);
        }

        protected override IList<AnnouncementComplex> InternalGetAdminAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels,
            bool? complete, int start, int count, string @from, string to, bool includeFrom, bool includeTo)
        {
            throw new NotImplementedException();
        }

        protected override AnnouncementSortOption SortOption
            => !_sortDesc ? AnnouncementSortOption.SectionNameAscending
                        : AnnouncementSortOption.SectionNameDescending;

        protected override Func<AnnouncementComplex, string> SortSelector
        {
            get
            {
                return x => x.ClassAnnouncementData != null ? x.ClassAnnouncementData.ClassName : x.LessonPlanData?.ClassName;
            }
        }
        protected override Func<ClassAnnouncement, string> FilterSelector { get { return x => x.ClassName; } }
    }


}
