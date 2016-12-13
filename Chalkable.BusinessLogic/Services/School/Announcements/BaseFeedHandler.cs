using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IFeedItemHandler
    {
        IList<AnnouncementComplex> GetAllItems(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetLessonPlansOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetAdminAnnouncementsOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels, bool? complete, int start, int count);
        IList<AnnouncementComplex> GetSupplementalAnnouncementsOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count, bool? ownedOnly = null);
    }
    
    public abstract class BaseFeedItemHandler<TSortBy, TFilterBy> : IFeedItemHandler
    {
        protected bool _sortDesc;
        protected BaseFeedItemHandler(bool sortDesc)
        {
            _sortDesc = sortDesc;
        }

        public virtual IList<AnnouncementComplex> GetLessonPlansOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count, bool? ownedOnly = null)
        {
            return InternalGetLessonPlans(locator, fromDate, toDate, classId, complete, start, count, default(TFilterBy), default(TFilterBy), true, true, ownedOnly);
        }

        public virtual IList<AnnouncementComplex> GetAdminAnnouncementsOnly(IServiceLocatorSchool locator,
            DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels, bool? complete, int start, int count)
        {
            return InternalGetAdminAnns(locator, fromDate, toDate, gradeLevels, complete, start, count, default(TFilterBy), default(TFilterBy), true, true);
        }

        public IList<AnnouncementComplex> GetSupplementalAnnouncementsOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId,
            bool? complete, int start, int count, bool? ownedOnly = null)
        {
            return InternalGetSupplementalAnns(locator, fromDate, toDate, classId, complete, start, count, default(TFilterBy), default(TFilterBy), true, true, ownedOnly);
        }
        
        public virtual IList<AnnouncementComplex> GetAllItems(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, 
            bool? complete, int start, int count, bool? ownedOnly = null)
        {
            var ct = count != int.MaxValue ? count + 1 : count;

            var classAnns = locator.ClassAnnouncementService.GetClassAnnouncementsForFeed(fromDate, toDate, classId, complete, null, start, ct, SortOption);
            
            if (start > 0 && classAnns.Count == 0)
                return classAnns;

            TFilterBy from, to;
            bool includeFrom, includeTo;

            IdentifyFilterInterval(out from, out to, out includeFrom, out includeTo, classAnns, start, count);

            //remove(count + 1) - item
            if (classAnns.Count > count)
                classAnns.RemoveAt(classAnns.Count - 1);

            
            var lps = InternalGetLessonPlans(locator, fromDate, toDate, classId, complete, 0, int.MaxValue, from, to, includeFrom, includeTo, ownedOnly);
            var res = MergeItems(classAnns, lps);

            var supplementalAnns = InternalGetSupplementalAnns(locator, fromDate, toDate, classId, complete, 0, int.MaxValue, from, to, includeFrom, includeFrom, true);
            res = MergeItems(res, supplementalAnns);

            if (locator.Context.Role == CoreRoles.STUDENT_ROLE && !classId.HasValue)
                res = MergeItems(res, InternalGetAdminAnns(locator, fromDate, toDate, null, complete, 0, int.MaxValue, from, to, includeFrom, includeTo));

            return res.ToList();
        }

        protected virtual void IdentifyFilterInterval(out TFilterBy from, out TFilterBy to, out bool includeFrom, out bool includeTo, IList<AnnouncementComplex> anns, int start, int count)
        {
            //Change data range for other feed items

            from = default(TFilterBy);
            to = default(TFilterBy);

            includeFrom = true;
            includeTo = true;

            var classAnns = anns.Where(x=>x.ClassAnnouncementData != null).Select(x => x.ClassAnnouncementData).ToList();
            var isNotFistPage = start > 0 && classAnns.Count > 0;
            var isNotLastPage = classAnns.Count > count;

            if (isNotFistPage && !_sortDesc || isNotLastPage && _sortDesc)
            {
                from = classAnns.Min(FilterSelector);
                includeFrom = _sortDesc;
            }

            if (isNotLastPage && !_sortDesc || isNotFistPage && _sortDesc)
            {
                to = classAnns.Max(FilterSelector);
                includeTo = !_sortDesc;
            }
        }

        protected virtual IList<AnnouncementComplex> MergeItems(IList<AnnouncementComplex> anns1, IList<AnnouncementComplex> anns2)
        {
            return anns1.Merge(anns2, SortSelector, SortComparator, _sortDesc).ToList();
        }

        protected abstract IList<AnnouncementComplex> InternalGetLessonPlans(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId
            , bool? complete, int start, int count, TFilterBy from, TFilterBy to, bool includeFrom, bool includeTo, bool? ownedOnly = null);
        protected abstract IList<AnnouncementComplex> InternalGetAdminAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels
            , bool? complete, int start, int count, TFilterBy from, TFilterBy to, bool includeFrom, bool includeTo);

        protected abstract IList<AnnouncementComplex> InternalGetSupplementalAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId
            , bool? complete, int start, int count, TFilterBy from, TFilterBy to, bool includeFrom, bool includeTo, bool? ownedOnly = null);
        
        protected virtual IComparer<TSortBy> SortComparator => null;
        protected abstract Func<ClassAnnouncement, TFilterBy> FilterSelector { get; }
        protected abstract Func<AnnouncementComplex, TSortBy> SortSelector { get; }
        protected abstract AnnouncementSortOption SortOption { get; }

    }
    
}
