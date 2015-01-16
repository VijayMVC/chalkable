REQUIRE('chlk.templates.calendar.announcement.DayCalendarBodyTpl');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.models.calendar.announcement.WeekItem');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.WeekCalendarBodyTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        'WeekCalendarBodyTpl', EXTENDS(chlk.templates.calendar.announcement.DayCalendarBodyTpl),[
            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            function groupAnnouncementsAsTable() {
                var rows = [], that = this;
                this.items.forEach(function (day, index) {
                    var dayItems = that.groupItems(day.getAnnouncementPeriods() || []);
                    dayItems.forEach(function (periodItems, order) {

                        var res = [].concat.apply([], periodItems.map(function (periodItem) {
                            var classId = periodItem.getPeriod().getClassId();

                            return day.getAnnouncements().filter(function (ann) {
                                return ann.getClassId() == classId;
                            })
                        }));

                        res.day = day;

                        (rows[order] = rows[order] || [[],[],[],[],[],[],[]])[index] = res;

                        rows[order].period = rows[order].period || (periodItems.length ? periodItems[0].getPeriod() : null);
                    });
                });

                return rows;
            }
        ]);
});