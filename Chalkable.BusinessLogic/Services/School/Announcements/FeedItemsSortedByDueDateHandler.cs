using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public class FeedItemsSortedByDueDateHandler : BaseFeedItemHandler<DateTime, DateTime?>
    {
        public FeedItemsSortedByDueDateHandler(bool sortDesc) : base(sortDesc)
        {
        }

        protected override IList<AnnouncementComplex> InternalGetLessonPlans(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, DateTime? @from, DateTime? to, bool includeFrom, bool includeTo, bool? ownedOnly = null)
        {
            @from = @from ?? fromDate;
            @to = to ?? toDate;
            return locator.LessonPlanService.GetLessonPlansSortedByDate(@from, to, includeFrom, includeTo, classId, complete, start, count, _sortDesc, ownedOnly);
        }

        protected override IList<AnnouncementComplex> InternalGetAdminAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels,
            bool? complete, int start, int count, DateTime? @from, DateTime? to, bool includeFrom, bool includeTo)
        {
            @from = @from ?? fromDate;
            @to = to ?? toDate;
            return locator.AdminAnnouncementService.GetAdminAnnouncementsSortedByDate(@from, to, includeFrom, includeTo, gradeLevels, complete, start, count, _sortDesc);
        }

        protected override IList<AnnouncementComplex> InternalGetSupplementalAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, DateTime? from, DateTime? to, bool includeFrom, bool includeTo,
            bool? ownedOnly = null)
        {
            return locator.SupplementalAnnouncementService.GetSupplementalAnnouncementsSortedByDate(from ?? fromDate, to ?? toDate, includeFrom
                , includeTo, classId, start, count, _sortDesc, ownedOnly);
        }

        protected override Func<AnnouncementComplex, DateTime> SortSelector
        {
            get
            {
                return x =>
                {
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.LessonPlanData != null) return (x.LessonPlanData.StartDate ?? x.Created);
                    return x.SupplementalAnnouncementData.Expires;
                };
            }
        }

        protected override Func<ClassAnnouncement, DateTime?> FilterSelector { get { return x => x.Expires; } }

        protected override AnnouncementSortOption SortOption
            => !_sortDesc ? AnnouncementSortOption.DueDateAscending : AnnouncementSortOption.DueDateDescending;
    }
}
