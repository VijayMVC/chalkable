REQUIRE('chlk.templates.calendar.announcement.BaseCalendarBodyTpl');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.models.calendar.announcement.WeekItem');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.DayCalendarBodyTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/DayCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        'DayCalendarBodyTpl', EXTENDS(chlk.templates.calendar.announcement.BaseCalendarBodyTpl),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items',

            [[ArrayOf(chlk.models.announcement.AnnouncementPeriod)]],
            ArrayOf(ArrayOf(chlk.models.announcement.AnnouncementPeriod)), function groupItems(items) {
                return items.reduce(function (acc, item) {
                    var o = item.getPeriod().getOrder();
                    return (acc[o] = acc[o] || []).push(item), acc;
                }, [0]).slice(1);
            },

            function groupItemsAsTable() {
                var rows = [], that = this;
                this.items.forEach(function (day, index) {
                    var dayItems = that.groupItems(day.getAnnouncementPeriods() || []);
                    dayItems.forEach(function (periodItems, order) {
                        periodItems.day = day;
                        (rows[order] = rows[order] || [[],[],[],[],[],[],[]])[index] = periodItems;
                        rows[order].period = rows[order].period || (periodItems.length ? periodItems[0].getPeriod() : null);
                    });
                });

                return rows;
            }
    ]);
});