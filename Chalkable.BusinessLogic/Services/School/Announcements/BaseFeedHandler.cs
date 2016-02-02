﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IFeedItemHandler
    {
        IList<AnnouncementComplex> GetAllItems(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count);
        IList<AnnouncementComplex> GetLessonPlansOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count);
        IList<AnnouncementComplex> GetAdminAnnouncementsOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels, bool? complete, int start, int count);

    }
    
    public abstract class BaseFeedItemHandler<TSortBy, TFilterBy> : IFeedItemHandler
    {
        protected bool _sortDesc;
        protected BaseFeedItemHandler(bool sortDesc)
        {
            _sortDesc = sortDesc;
        }

        public virtual IList<AnnouncementComplex> GetLessonPlansOnly(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count)
        {
            return InternalGetLessonPlans(locator, fromDate, toDate, classId, complete, start, count, default(TFilterBy), default(TFilterBy), true, true);
        }

        public virtual IList<AnnouncementComplex> GetAdminAnnouncementsOnly(IServiceLocatorSchool locator,
            DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels, bool? complete, int start, int count)
        {
            return InternalGetAdminAnns(locator, fromDate, toDate, gradeLevels, complete, start, count, default(TFilterBy), default(TFilterBy), true, true);
        }

        public virtual IList<AnnouncementComplex> GetAllItems(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start, int count)
        {
            var ct = count != int.MaxValue ? count + 1 : count;

            var classAnns = locator.ClassAnnouncementService.GetClassAnnouncementsForFeed(fromDate, toDate, classId, complete, null, start, ct, SortOption);

            if (start > 0 && classAnns.Count == 0)
                return classAnns;

            TFilterBy from, to;
            IdentifyFilterInterval(out from, out to, classAnns, start, count);

            //remove(count + 1) - item
            if (classAnns.Count > count)
                classAnns.RemoveAt(classAnns.Count - 1);

            bool includeFrom = _sortDesc, includeTo = !_sortDesc;

            var lps = InternalGetLessonPlans(locator, fromDate, toDate, classId, complete, 0, int.MaxValue, from, to, includeFrom, includeTo);

            var res = MerageItems(classAnns, lps);

            if (locator.Context.Role == CoreRoles.STUDENT_ROLE)
                res = MerageItems(res, InternalGetAdminAnns(locator, fromDate, toDate, null, complete, 0, int.MaxValue, from, to, includeFrom, includeTo));

            return res.ToList();
        }

        protected virtual void IdentifyFilterInterval(out TFilterBy from, out TFilterBy to, IList<AnnouncementComplex> anns, int start, int count)
        {
            //Change data range for other feed items

            from = default(TFilterBy);
            to = default(TFilterBy);

            var classAnns = anns.Where(x=>x.ClassAnnouncementData != null).Select(x => x.ClassAnnouncementData).ToList();
            var isNotFistPage = start > 0 && classAnns.Count > 0;
            var isNotLastPage = classAnns.Count > count;

            if (isNotFistPage || _sortDesc)
                from = classAnns.Min(FilterSelector);

            if (isNotLastPage && !_sortDesc || isNotFistPage && _sortDesc)
                to = classAnns.Max(FilterSelector);
        }

        protected virtual IList<AnnouncementComplex> MerageItems(IList<AnnouncementComplex> anns1, IList<AnnouncementComplex> anns2)
        {
            return anns1.Merge(anns2, SortSelector, SortComparetor, _sortDesc).ToList();
        }

        protected abstract IList<AnnouncementComplex> InternalGetLessonPlans(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, int? classId
            , bool? complete, int start, int count, TFilterBy from, TFilterBy to, bool includeFrom, bool includeTo);
        protected abstract IList<AnnouncementComplex> InternalGetAdminAnns(IServiceLocatorSchool locator, DateTime? fromDate, DateTime? toDate, IList<int> gradeLevels
            , bool? complete, int start, int count, TFilterBy from, TFilterBy to, bool includeFrom, bool includeTo);


        protected virtual IComparer<TSortBy> SortComparetor => null;
        protected abstract Func<ClassAnnouncement, TFilterBy> FilterSelector { get; }
        protected abstract Func<AnnouncementComplex, TSortBy> SortSelector { get; }
        protected abstract AnnouncementSortOption SortOption { get; }

    }
    
}
