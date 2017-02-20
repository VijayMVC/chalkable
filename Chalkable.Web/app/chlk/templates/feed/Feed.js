REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.templates.common.PageWithClasses');


NAMESPACE('chlk.templates.feed', function () {

    /** @class chlk.templates.feed.Feed*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/feed/Feed.jade')],
        [ria.templates.ModelBind(chlk.models.feed.Feed)],
        'Feed', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            [ria.templates.ModelPropertyBind],
            Boolean, 'importantOnly',

            [ria.templates.ModelPropertyBind],
            Number, 'importantCount',

            [ria.templates.ModelPropertyBind],
            Number, 'newNotificationCount',

            [ria.templates.ModelPropertyBind],
            Number, 'start',

            [ria.templates.ModelPropertyBind],
            Number, 'count',

            [ria.templates.ModelPropertyBind],
            Number, 'adjustedItemsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'adjustCount',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'annType',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChlkDate), 'classScheduledDays',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.FeedSortTypeEnum, 'sortType',

            [ria.templates.ModelPropertyBind],
            String, 'submitType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inProfile',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readonly',

            [ria.templates.ModelPropertyBind],
            Boolean, 'staringDisabled',

            function hasFilters(){
                return this.getAnnType() || this.getGradingPeriodId() || this.getStartDate() || this.getEndDate() || this.getSortType() && this.getSortType().valueOf()
            },

            function getTimeSelectPlaceholder(){
                if(this.getStartDate())
                    return this.getStartDate().format('M d - ') + this.getEndDate().format('M d');

                var gpId = this.getGradingPeriodId()

                if(gpId)
                    return this.getGradingPeriods().filter(function(gp){return gp.getId() == gpId})[0].getName();

                return 'Any Time';
            },

            function getActivitiesTypeSelectPlaceholder(){
                var typesEnum = chlk.models.announcement.AnnouncementTypeEnum;

                switch(this.getAnnType()){
                    case typesEnum.CLASS_ANNOUNCEMENT : return 'Activities Only';
                    case typesEnum.ADMIN : return 'Admin Announcement Only';
                    case typesEnum.LESSON_PLAN : return 'Lesson Plan Only';
                    case typesEnum.SUPPLEMENTAL_ANNOUNCEMENT : return 'Supplemental Only';
                    default : return 'All Types';
                }
            },

            function getSortSelectPlaceholder(){
                if(this.getSortType() == chlk.models.announcement.FeedSortTypeEnum.DUE_DATE_ASCENDING)
                    return 'Due Date: Earliest';

                return 'Due Date: Latest';
            }

        ])
});
