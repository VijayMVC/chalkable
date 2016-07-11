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

            function getClassesForSelect(){
                var items = this.getClassesByYears(), res = [];
                items.forEach(function(item){
                    var classes = item.getClasses();
                    if(classes.length){
                        res.push({
                            name: item.getSchoolYear().getName(),
                            values: classes.map(function(clazz){
                                return {
                                    name: clazz.getFullClassName(),
                                    id: clazz.getId().valueOf()
                                }
                            })
                        })
                    }
                });
                return res;
            }

        ])
});
