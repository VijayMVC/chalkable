REQUIRE('chlk.models.feed.FeedAdmin');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.templates.common.PageWithGrades');


NAMESPACE('chlk.templates.feed', function () {

    /** @class chlk.templates.feed.FeedAdmin*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/feed/Feed.jade')],
        [ria.templates.ModelBind(chlk.models.feed.FeedAdmin)],
        'FeedAdmin', EXTENDS(chlk.templates.common.PageWithGrades), [
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
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'annType',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.FeedSortTypeEnum, 'sortType',

            [ria.templates.ModelPropertyBind],
            String, 'submitType',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevels',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inProfile',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readonly',

            [ria.templates.ModelPropertyBind],
            Boolean, 'staringDisabled',

            function hasFilters(){
                return this.getAnnType() || this.getGradingPeriodId() || this.getStartDate() || this.getEndDate() || this.getSortType() && this.getSortType().valueOf()
            }
        ])
});
