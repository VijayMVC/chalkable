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

            function getAnnouncementsOffPeriods() {
                return this.items.map(function (day, index) {
                    var classesTaday = day.getAnnouncementPeriods()
                        .map(function (_) { return _.getPeriod(); })
                        .filter(function (_) { return _.getStartTime() && _.getClassName(); })
                        .map(function (_) { return _.getClassId(); });

                    var result = day.getAnnouncements()
                        .filter(function (_) { return classesTaday.indexOf(_.getClassId()) < 0 });

                    result.day = day;

                    return result;
                });
            },

            function groupAnnouncementsAsTable() {
                var rows = [], that = this;
                this.items.forEach(function (day, index) {
                    var dayItems = that.groupItems(day.getAnnouncementPeriods() || []);
                    dayItems.forEach(function (periodItems, order) {

                        var res = [].concat.apply([], periodItems.map(function (periodItem) {
                            var classId = periodItem.getPeriod().getClassId();

                            if (!periodItem.getPeriod().getStartTime() || !periodItem.getPeriod().getClassName())
                                return [];

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