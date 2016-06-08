using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{

    public class FeedItemsSortByTitleHandler : BaseFeedItemHandler<string, string>
    {
        public FeedItemsSortByTitleHandler(bool sortDesc) : base(sortDesc)
        {
        }
        protected override IList<AnnouncementComplex> InternalGetLessonPlans(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, string @from, string to, bool includeFrom, bool includeTo, bool? ownedOnly = null)
        {
            return locator.LessonPlanService.GetLessonPlansSortedByTitle(fromDate, toDate, @from, to, includeFrom, includeTo, classId, complete, start, count, _sortDesc, ownedOnly);
        }
        protected override IList<AnnouncementComplex> InternalGetAdminAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels,
            bool? complete, int start, int count, string @from, string to, bool includeFrom, bool includeTo)
        {
            return locator.AdminAnnouncementService.GetAdminAnnouncementsSortedByTitle(fromDate, toDate, @from, to, includeFrom, includeTo, gradeLevels, complete, start, count, _sortDesc);
        }

        protected override IList<AnnouncementComplex> InternalGetSupplementalAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, string @from, string to, bool includeFrom, bool includeTo,
            bool? ownedOnly = null)
        {
            return locator.SupplementalAnnouncementService.GetSupplementalAnnouncementSortedByTitle(fromDate, toDate, @from, to, includeFrom, includeTo, classId, start, count, _sortDesc, ownedOnly);
        }

        protected override AnnouncementSortOption SortOption
            => !_sortDesc ? AnnouncementSortOption.NameAscending : AnnouncementSortOption.NameDescending;
        protected override Func<AnnouncementComplex, string> SortSelector { get { return x => x.Title; } }
        protected override Func<ClassAnnouncement, string> FilterSelector { get { return x => x.Title; } }

    }
}
